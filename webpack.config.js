const path = require("path");
const webpack = require("webpack");
const HtmlWebpackPlugin = require('html-webpack-plugin');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const CopyWebpackPlugin = require('copy-webpack-plugin');

// Update these values when your project's structure changes.
var CONFIG = {
    indexHtmlTemplate: './src/index.html',
    fsharpEntry: './src/Demo.fsproj',
    cssEntry: './src/scss/main.scss',
    outputDir: './output',
    assetsDir: './public',
    devServerPort: 8080
}

// The HtmlWebpackPlugin allows us to use a template for the index.html page
// and automatically injects <script> or <link> tags for generated bundles.
var commonPlugins = [
    new HtmlWebpackPlugin({
        filename: 'index.html',
        template: CONFIG.indexHtmlTemplate
    })
];

module.exports = function (evn, argv) {
  var mode = argv.mode || "development";
  var isProduction = mode === "production";
  console.log("Webpack mode: " + mode);

  return {
    // In development we put code and styles (CSS) in same bundle
    // to allow hot reloading (HMR) for styles too. But in production
    // mode we create two different bundles.
    // NOTE we use a polyfill for old browser not compatible with latest native APIs.
    entry: isProduction ?
        {
            app: ['fable-loader/polyfill', CONFIG.fsharpEntry, CONFIG.cssEntry]
        } : {
            app: ['fable-loader/polyfill', CONFIG.fsharpEntry],
            style: [CONFIG.cssEntry]
        },
    // NOTE we add a hash to the output file name in production
    // to prevent browser caching if code changes
    output: {
        path: path.join(__dirname, CONFIG.outputDir),
        filename: isProduction ? '[name].[hash].js' : '[name].js'
    },
    mode: mode,
    devtool: isProduction ? false : "eval-source-map",
    // This splits the code coming from npm packages into a different file.
    // This is because 3rd party dependencies usually change less often than
    // our own code, so putting them in a different file increases the chances
    // that the browser caches it.
    optimization : {
        splitChunks: {
            cacheGroups: {
                commons: {
                    test: /node_modules/,
                    name: "vendors",
                    chunks: "all"
                }
            }
        },
    },
    // Besides the HtmlPlugin, we used the following plugins:
    // PRODUCTION
    //      - MiniCssExtractPlugin: Extracts CSS from bundle to a different file
    //      - CopyWebpackPlugin: Copies static assets to output directory
    // DEVELOPMENT
    //      - HotModuleReplacementPlugin: Enables hot reloading when code changes without refreshing
    //      - NamedModulesPlugin: Shows relative path of modules when HMR is enabled
    plugins: isProduction ?
        commonPlugins.concat([
            new MiniCssExtractPlugin({ filename: 'style.css' }),
            new CopyWebpackPlugin([ { from: CONFIG.assetsDir } ])
        ])
        : commonPlugins.concat([
            new webpack.HotModuleReplacementPlugin(),
            new webpack.NamedModulesPlugin()
        ]),
    // Configuration for webpack-dev-server
    devServer: {
        publicPath: "/",
        contentBase: CONFIG.assetsDir,
        port: CONFIG.devServerPort,
        hot: true,
        inline: true
    },
    // These are the loaders
    // - fable-loader: transforms F# into JS
    // - babel-loader: transforms JS to old syntax (compatible with old browsers)
    // - sass-loaders: transforms SASS/SCSS into JS
    // - file-loader: Moves files referenced in the code (fonts, images)
    //   into output folder
    module: {
        rules: [
            {
                test: /\.fs(proj)?$/,
                use: { loader: "fable-loader" }
            },
            {
                test: /\.js$/,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader',
                    options: { presets: [
                        ["@babel/preset-env", { "modules": false }]
                    ] }
                },
            },
            {
                test: /\.(sass|scss|css)$/,
                use: [
                    isProduction ? MiniCssExtractPlugin.loader : 'style-loader',
                    'css-loader',
                    'sass-loader',
                ],
            },
            {
                test: /\.(png|jpg|jpeg|gif|svg|woff|woff2|ttf|eot)(\?.*)?$/,
                use: ["file-loader"]
            }
        ]
    }
  };
}
