const path = require("path");
const webpack = require("webpack");
const HtmlWebpackPlugin = require('html-webpack-plugin');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const CopyWebpackPlugin = require('copy-webpack-plugin');

var commonPlugins = [
    new HtmlWebpackPlugin({
        filename: 'index.html',
        template: './src/index.html'
    })
];

module.exports = function (evn, argv) {
  var isProduction = argv.mode === "production";
  console.log("Webpack mode: " + argv.mode);

  return {
    // We don't use the same entry for dev and production,
    // to make HMR over style quicker for dev env
    entry: isProduction ?
        {
            demo: [
                "babel-polyfill",
                './src/Demo.fsproj',
                './src/scss/main.scss'
            ]
        } : {
            app: [
                "babel-polyfill",
                './src/Demo.fsproj'
            ],
            style: [
                './src/scss/main.scss'
            ]
        },
    output: {
        path: path.join(__dirname, './output'),
        filename: isProduction ? '[name].[hash].js' : '[name].js'
    },
    devtool: isProduction ? false : "eval-source-map",
    optimization : {
        splitChunks: {
            cacheGroups: {
                commons: {
                    test: /node_modules/,
                    name: "vendors",
                    chunks: "all"
                },
                fable: {
                    test: /fable-core/,
                    name: "fable",
                    chunks: "all"
                }
            }
        },
    },
    plugins: isProduction ?
        commonPlugins.concat([
            // Extracts CSS from bundle to a different file
            new MiniCssExtractPlugin({ filename: 'style.css' }),
            // Copies files to output directory
            new CopyWebpackPlugin([ { from: './public' } ])
        ])
        : commonPlugins.concat([
            new webpack.HotModuleReplacementPlugin(),
            new webpack.NamedModulesPlugin()
        ]),
    devServer: {
        contentBase: './public/',
        publicPath: "/",
        port: 8080,
        hot: true,
        inline: true
    },
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
