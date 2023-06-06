import esbuild from 'esbuild';
import { existsSync, readdirSync, statSync, rmSync } from "fs";
import { join, relative } from "path";

function scanDir(dir, org) {
    return readdirSync(dir).reduce((files, file) => {
        const absolute = join(dir, file);
        return [...files, ...(statSync(absolute).isDirectory()
            ? scanDir(absolute, org || dir)
            : [relative(org || dir, absolute)])]
    }, []);
}


function cleanPlugin() {
    return {
        name: 'clean',
        setup(build) {
            build.onEnd(result => {
                try {
                    const { outputs } = result.metafile ?? {};
                    if (!outputs || !existsSync(build.initialOptions.outdir))
                        return;

                    const outputFiles = new Set(Object.keys(outputs));
                    scanDir(build.initialOptions.outdir).forEach(file => {
                        if (!file.endsWith('.js') && !file.endsWith('.js.map'))
                            return;
                        if (!outputFiles.has(join(build.initialOptions.outdir, file).replace(/\\/g, '/'))) {
                            console.log('esbuild clean: deleting extra file ' + file);
                            rmSync(join(build.initialOptions.outdir, file));
                        }
                    });
                } catch (e) {
                    console.error(`esbuild clean: ${e}`);
                }
            });
        }
    }
}


let config = {
    entryPoints: ['Views/Home/HomePage.ts'],
    bundle: true,
    outdir: 'wwwroot/Scripts',
    metafile: true,
    sourcemap: true,
    target: 'es6',
    format: 'esm',
    splitting: true,
    chunkNames: 'chunks/[name]-[hash]',
    mainFields: ['module', 'main'],
    plugins: [cleanPlugin()],
};


if (process.argv[2] == "--watch") {
    let ctx = await esbuild.context(config)
    console.log("watching...")
    await ctx.watch();
}
else {
    await esbuild.build(config)
}