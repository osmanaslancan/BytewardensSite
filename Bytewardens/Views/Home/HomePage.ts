import { Grid, GridOptions, ICellRendererParams, RowContainerName } from "ag-grid-community";
import { param } from "jquery";

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

export class HomePageGrid {

    private grid: Grid;

    constructor(element, data) {
        
        this.grid = new Grid(element, {
            rowData: data,
            rowHeight: 100,
            columnDefs: [
                { cellRenderer: (params) => this.FavoriteRenderer(params), width: 65 },
                { field: "thumb", cellClass: "flex justify-center items-center", headerName: "Image", cellRenderer: (params) => this.ThumbRenderer(params), width: 100 },
                { field: "title" },
                { headerName: "Meta Critic", cellClass: "flex justify-center items-center", field: "metacriticLink", cellRenderer: (params) => this.MetaCriticRenderer(params), width: 125 },
                { field: "salePrice", cellRenderer: (params) => this.PriceRenderer(params) },
                { field: "savings" },
                { field: "steamRatingText" },
                { field: "steamRatingPercent" },
                { field: "steamRatingCount" },
                { field: "steamAppID" },
                { field: "releaseDate" },
                { field: "lastChange" },
            ]
        } as GridOptions<Game> );
    }

    PriceRenderer(params: ICellRendererParams<Game>) {
        var container = $("<div/>");
        container.addClass("flex justify-center items-center flex-col");
        var old = $("<span/>");
        old.text(params.data.normalPrice);
        old.addClass("line-through text-red");
        old.appendTo(container);
        var sale = $("<span/>");
        sale.text(parseInt(params.data.salePrice) == 0 ? "FREE!" : params.data.salePrice);
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
        var link = $("<a></a>");
        link.addClass("w-full h-full flex justify-center items-center no-underline");
        var icon = $("<i></i>");
        icon.addClass("fa-regular cursor-pointer fa-heart text-red-500 fa-xl");
        link.append(icon);
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
        new HomePageGrid(document.querySelector('#myGrid'), args)
    });
}