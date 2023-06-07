import { Bean, ColDef, Column, ColumnState, Grid as AgGrid, GridOptions, ICellRendererParams, ITextFilterParams, PaginationProxy, RowContainerName, SortDirection, TextFormatter } from "ag-grid-community";
import { Button, Tooltip } from "bootstrap";
import { post } from "jquery";
import moment from "moment";

export interface Game {
    internalName: string
    title: string
    metacriticLink: string
    dealID: string
    storeID: string
    gameID: string
    salePrice: string
    normalPrice: string
    isOnSale: string
    savings: string
    metacriticScore: string
    steamRatingText: string
    steamRatingPercent: string
    steamRatingCount: string
    steamAppID: string
    releaseDate: number
    lastChange: number
    dealRating: string
    thumb: string
}
interface Store {
    storeID: string
    storeName: string
    isActive: number,
    images: StoreImages
}
interface StoreImages {
    banner: string
    logo: string
    icon: string
}

export interface HomePageModel {
    games: Game[]
    maxPages: number,
    stores: Store[],
    isLoggedIn: boolean,
    userFavorites: string[],
    serverSide: boolean
}
export class HomePageGrid {

    private grid: AgGrid;
    private gridOptions: GridOptions<Game>;
    private oldTitleFilter: string;
    private storesById;
    private model: HomePageModel;

    constructor(element, data: HomePageModel) {
        this.model = data;
        this.storesById = {};
        data.stores?.forEach(store => {
            this.storesById[store.storeID] = store;
        });
        this.SetupGrid(element, data);
        this.SetupPager(data);
    }

    SetupPager(data: HomePageModel) {
        if (!this.model.serverSide)
            return;
        var pager = $("#pager");
        pager.css("height", "60px");
        var url = new URL(window.location.href);
        var currentPage = parseInt(url.searchParams.get("Page") ?? "1");
        var maxPages = data.maxPages;
        var pageSize = 60;
        pager.addClass("flex justify-end items-center p-8")
        var info = $("<div></div>").appendTo(pager);
        info.addClass("flex justify-center items-center gap-1");
        info.html(`
        Showing <span class="font-bold">${pageSize * (currentPage - 1) + 1}</span> to 
        <span class="font-bold">${pageSize * (currentPage - 1) + data.games.length}</span> 
        of <span class="font-bold">${maxPages == 1 ? data.games.length : maxPages * pageSize}</span>`)

        var buttons = $("<div></div>").appendTo(pager);
        buttons.addClass("flex gap-2 justify-center items-center ml-8");

        buttons.html(`
            <i id="backward-fast" class="fa-sharp cursor-pointer fa-solid fa-backward-fast ${currentPage != 1 ? "" : "text-[#212529]/25"}"></i>
            <i id="backward-step" class="fa-sharp cursor-pointer fa-solid fa-backward-step ${currentPage != 1 ? "" : "text-[#212529]/25"}"></i>
            <span class="ag-paging-description" role="status">
                <span>Page</span>
                <span id="start-page-number" ref="lbCurrent" class="font-bold">1</span>
                <span>of</span>
                <span id="last-page-number" ref="lbTotal" class="font-bold">1</span>
            </span>
            <i id="forward-step" class="fa-sharp cursor-pointer fa-solid fa-forward-step ${currentPage != maxPages ? "" : "text-[#212529]/25"}"></i>
            <i id="forward-fast" class="fa-sharp cursor-pointer fa-solid fa-forward-fast ${currentPage != maxPages ? "" : "text-[#212529]/25"}"></i>
        `)
        const navigate = (page) => {
            url.searchParams.set("Page", page);
            window.location.href = url.href;
        }

        buttons.find("#start-page-number").text(currentPage);
        buttons.find("#last-page-number").text(maxPages);

        if (currentPage != 1) {
            buttons.find("#backward-fast").on('click', (e) => { navigate(1); });
            buttons.find("#backward-step").on('click', (e) => { navigate(currentPage - 1) });
        }
        if (currentPage != maxPages) {
            buttons.find("#forward-step").on('click', (e) => { navigate(currentPage + 1); });
            buttons.find("#forward-fast").on('click', (e) => { navigate(maxPages); });
        }

    }

    first(elements, selector) {
        for (var e of elements) {
            if (selector(e)) {
                return e;
            }
        }
    }

    sort(field?: string, desc?) {
        if (!this.model.serverSide)
            return;
        var url = new URL(window.location.href);
        if (field) {
            field = field.charAt(0).toUpperCase() + field.slice(1)
            url.searchParams.set("Sort", field);
            url.searchParams.set("Desc", desc == "desc" ? "1" : "0");
        } else {
            url.searchParams.delete("Sort");
            url.searchParams.delete("Desc");
        }
        
        window.location.href = url.href;
    }

    filterTitle(text: string) {
        if (!this.model.serverSide)
            return;
        var url = new URL(window.location.href);
        if (text) {
            url.searchParams.set("FilterTitle", text);
        } else {
            if (url.searchParams.get("FilterTitle")) {
                url.searchParams.delete("FilterTitle");
            }
            else {
                return;
            }
        }
        window.location.href = url.href;
    }

    private SortMap = {
        "salePrice" : "Price"
    }

    private SortMapR = {
        "Price": "salePrice"
    }
    SetupGrid(element, data: HomePageModel) {
        var url = new URL(window.location.href);
        this.gridOptions = {
            rowData: data.games,
            rowHeight: 100,
            onSortChanged: (e) => {
                var column: Column = this.first(e.columnApi.getColumns(), (e: Column) => e.getSort());
                if (column) {
                    this.sort(this.SortMap[column.getColId()] ?? column.getColId(), column.getSort());
                }
                else {
                    this.sort(null, null);
                }
            },
            defaultColDef: {
                resizable: true,
            },
            suppressMaintainUnsortedOrder: true,
            columnDefs: [
                { field: "thumb", cellClass: "flex justify-center items-center", headerName: "Image", cellRenderer: (params) => this.ThumbRenderer(params), width: 100 },
                {
                    field: "title",
                    sortable: true,
                    filter: true,
                    floatingFilter: true,
                    filterParams: {
                        maxNumConditions: 1,
                        trimInput: false,
                        filterOptions: ["contains"],
                        textMatcher: !this.model.serverSide ? undefined : (params) => {
                            if (this.oldTitleFilter == params.filterText)
                                return;

                            if (params.filterText) {
                                this.filterTitle(params.filterText);
                            }
                            this.filterTitle("");

                            this.oldTitleFilter = params.filterText;
                        },

                        buttons: [
                            "apply",
                            "clear"
                        ],
                        
                    } as ITextFilterParams 
                },
                { headerName: "Meta Critic", cellClass: "flex justify-center items-center", field: "metacriticLink", cellRenderer: (params) => this.MetaCriticRenderer(params), width: 125 },
                {
                    field: "salePrice",
                    comparator: (valueA, valueB, nodeA, nodeB, isDescending) => parseFloat(valueA) - parseFloat(valueB),
                    sortable: true,
                    cellRenderer: (params) => this.PriceRenderer(params)
                },
                { field: "savings", sortable: true, valueFormatter: (params) => parseFloat(params.value).toFixed(2) + "%" },
                { headerName: "Steam Rating", sortable: true, field: "steamRatingText", cellRenderer: (params) => this.SteamRatingRenderer(params) },
                { headerName: "Store", cellRenderer: (params) => this.StoreImageRenderer(params) },
                { field: "releaseDate", valueFormatter: (params) => params.value ? moment(params.value * 1000).format("DD-MM-yyyy") : "" },
            ]
        };
        var sort = url.searchParams.get("Sort");
        if (sort) {
            sort = this.SortMapR[sort] ?? sort;
            sort = sort.charAt(0).toLowerCase() + sort.slice(1)
            var column : ColDef = this.first(this.gridOptions.columnDefs, (e: ColDef) => e.field == sort);
            var sort = url.searchParams.get("Desc") == "0" ? 'asc' : 'desc';
            column.sort = sort as SortDirection;
        }

        if (data.isLoggedIn) {
            this.gridOptions.columnDefs.unshift(
                { cellRenderer: (params) => this.FavoriteRenderer(params), width: 65 },
            );
        }

        this.grid = new AgGrid(element, this.gridOptions);
        this.gridOptions.api!.getFilterInstance('title').onAnyFilterChanged
        if (url.searchParams.get("FilterTitle")) {
            this.gridOptions.api!.getFilterInstance('title')!.setModel({
                filter: url.searchParams.get("FilterTitle"),
                type: "contains"
            });
        }

        $("body").on('click', (e) => {
            var target = $(e.target);
            if (target.attr("type") == 'button' && target.attr("ref") == 'clearFilterButton') {
                this.filterTitle("");
            }
        });
    }

    StoreImageRenderer(params: ICellRendererParams<Game>) {
        var store = this.storesById[params.data.storeID] as Store;
        var img = $("<img/>")
        img.attr("src", "https://www.cheapshark.com/" + store.images.logo);
        img.attr("data-bs-toggle", "tooltip");
        img.attr("data-bs-title", `${store.storeName}`);
        img.width(50);
        img.height(50);
        new Tooltip(img[0]);

        return img[0];
    }

    SteamRatingRenderer(params: ICellRendererParams<Game>) {
        if (!params.value)
            return "";

        var link = $("<a></a>");
        link.text(params.value);
        link.attr("data-bs-toggle", "tooltip");
        var percent = parseInt(params.data.steamRatingPercent);
        link.attr("data-bs-title", `${percent}% of the ${params.data.steamRatingCount} user reviews for this game are positive.`);
        if (percent > 49) {
            link.addClass("text-[#66C0F4]")
        }
        else {
            link.addClass("text-[#A34C25]")
        }
        link.addClass("no-underline")
        new Tooltip(link[0]);
        return link[0];
    }

    PriceRenderer(params: ICellRendererParams<Game>) {
        var container = $("<div/>");
        container.addClass("flex justify-center items-center flex-col");
        if (parseFloat(params.data.salePrice) != parseFloat(params.data.normalPrice)) {
            var old = $("<span/>").appendTo(container);;
            old.text("$" + params.data.normalPrice);
            old.addClass("line-through text-red");
        }
        var sale = $("<span/>");
        sale.text(parseInt(params.data.salePrice) == 0 ? "FREE!" : "$" + params.data.salePrice);
        sale.appendTo(container);
        return container[0];
    }

    ThumbRenderer(params: ICellRendererParams<Game>) {
        var link = $("<div></div>");
        link.addClass("w-full h-full bg-contain bg-no-repeat bg-center");
        link.css("background-image", `url(${params.value})`)
        return link[0];
    }

    FavoriteRenderer(params: ICellRendererParams<Game>) {
        var link = $("<div></div>");
        link.addClass("w-full h-full flex justify-center items-center no-underline");
        var icon = $("<i></i>");
        icon.addClass("fa-regular cursor-pointer fa-heart text-red-500 fa-xl");
        link.append(icon);
        link.data("row-id", params.data.gameID)
        if (this.model.userFavorites) {
            if (this.model.userFavorites.indexOf(params.data.gameID) >= 0) {
                icon.addClass("fas");
            }
        }
        if (this.model.isLoggedIn) {
            link.on('click', (e) => {
                if (this.model.userFavorites && this.model.userFavorites.indexOf($(e.currentTarget).data("row-id")) == -1) {
                    $.ajax("/AddToFavorites", {
                        method: "post",
                        data: {
                            "GameId": $(e.currentTarget).data("row-id")
                        },
                        success: () => {
                            var gameId = $(e.currentTarget).data("row-id");
                            if (this.model.userFavorites) {
                                if (this.model.userFavorites.indexOf(gameId) == -1) {
                                    this.model.userFavorites.push(gameId);
                                }
                            }
                            var changedRows = [];
                            var rows = this.gridOptions.api.forEachNode(x => {
                                if (x.data.gameID == gameId) {
                                    changedRows.push(x);
                                }
                            })
                            this.gridOptions.api.redrawRows({
                                rowNodes: changedRows
                            });
                        }
                    })
                }
                else {
                    $.ajax("/RemoveFromFavorites", {
                        method: "post",
                        data: {
                            "GameId": $(e.currentTarget).data("row-id")
                        },
                        success: () => {
                            var gameId = $(e.currentTarget).data("row-id");
                            if (this.model.userFavorites) {
                                if (this.model.userFavorites.indexOf(gameId) >= -1) {
                                    this.model.userFavorites.splice(this.model.userFavorites.indexOf(gameId), 1);
                                }
                            }
                            var changedRows = [];
                            var rows = this.gridOptions.api.forEachNode(x => {
                                if (x.data.gameID == gameId) {
                                    changedRows.push(x);
                                }
                            })
                            this.gridOptions.api.redrawRows({
                                rowNodes: changedRows
                            });
                        }
                    })
                }
            })
        }
        return link[0];
    }

    MetaCriticRenderer(params: ICellRendererParams<Game>) {
        if (!params.value)
            return "";

        var link = $("<a></a>");
        link.addClass("w-full h-full");
        var circle = $('<div/>')

        circle.addClass("rounded-circle font-bold flex justify-center items-center p-2 w-9 h-9 text-white text-center align-top")
        var score = parseInt(params.data.metacriticScore);
        if (score > 74) {
            circle.addClass("bg-success");
        }
        else if (score > 49) {
            circle.addClass("bg-warning");
        }
        else if (score > 0) {
            circle.addClass("bg-danger");
        }
        else {
            circle.addClass("bg-secondary");
        }
        circle.text(score);
        link.append(circle);
        link.addClass("no-underline flex justify-center items-center");
        link.attr("href", "https://www.metacritic.com/" + params.value);
        link.attr("target", "_blank");
        return link[0];
    }
}


export default function initPage(args) {
    $(() => {
        new HomePageGrid($('#myGrid')[0], args)
    });
}