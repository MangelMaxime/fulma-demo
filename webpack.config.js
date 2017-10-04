var path = require("path");
var webpack = require("webpack");
var fableUtils = require("fable-utils");
var HtmlWebpackPlugin = require('html-webpack-plugin');

function resolve(filePath) {
    return path.join(__dirname, filePath)
}

var babelOptions = fableUtils.resolveBabelOptions({
    presets: [
        ["env", {
            "targets": {
                "browsers": ["last 2 versions"]
            },
            "modules": false
        }]
    ]
});

var isProduction = process.argv.indexOf("-p") >= 0;
console.log("Bundling for " + (isProduction ? "production" : "development") + "...");

var commonPlugins = [
    new HtmlWebpackPlugin({
        filename: resolve('./output/index.html'),
        template: resolve('./src/index.html'),
        minify: isProduction ? {} : false
    })
];

module.exports = {
    devtool: false,
    entry: {
        app: [
            "babel-polyfill",
            resolve('./src/Demo.fsproj')
        ],
        style: [
            resolve('./src/sass/main.sass')
        ]
    },
    output: {
        path: resolve('./output'),
        filename: isProduction ? '[name].[hash].js' : '[name].js'
    },
    plugins: isProduction ?
        commonPlugins
        : commonPlugins.concat([
            new webpack.HotModuleReplacementPlugin(),
            new webpack.NamedModulesPlugin()
        ]),
    resolve: {
        alias: {
            "react": "preact-compat",
            "react-dom": "preact-compat"
        },
        modules: [
            "node_modules/",
            resolve("./node_modules/")
        ]
    },
    devServer: {
        contentBase: resolve('./public/'),
        publicPath: "/",
        port: 8080,
        hot: true,
        inline: true
    },
    module: {
        rules: [
            {
                test: /\.fs(x|proj)?$/,
                use: {
                    loader: "fable-loader",
                    options: {
                        babel: babelOptions,
                        define: isProduction ? [] : ["DEBUG"],
                        extra: { optimizeWatch: true }
                    }
                }
            },
            {
                test: /\.js$/,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader',
                    options: babelOptions
                },
            },
            {
                test: /\.sass$/,
                use: ["style-loader", "css-loader", "sass-loader"]
            },
            {
                test: /\.css$/,
                use: ['style-loader', 'css-loader']
            },
            {
                test: /\.(png|jpg|jpeg|gif|svg|woff|woff2|ttf|eot)(\?.*$|$)/,
                use: ["file-loader"]
            }
        ]
    }
};
