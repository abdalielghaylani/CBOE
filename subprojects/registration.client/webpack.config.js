'use strict';

const path = require('path');
const webpack = require('webpack');
const HtmlWebpackPlugin = require('html-webpack-plugin');

const baseConfig = {
    entry: {
        'polyfills': './src/polyfills.ts',
        'vendor': './src/vendor.ts',
        'index': './src/index.ts'
    },
    resolve: {
        root: [path.join(__dirname, 'src')],
        extensions: ['', '.webpack.js', '.web.js', '.ts', '.js']
    },
    plugins: [
        new webpack.optimize.OccurenceOrderPlugin(true),
        new webpack.optimize.CommonsChunkPlugin({
            name: ['index', 'vendor', 'polyfills'],
            minChunks: Infinity
        }),
        new webpack.ProvidePlugin({
            $: "jquery",
            jQuery: "jquery"
        }),
        new HtmlWebpackPlugin({
            template: './src/index.html',
            inject: 'body',
            minify: false,
            chunksSortMode: 'dependency'
        }),
        new webpack.DefinePlugin({
            __DEV__: process.env.NODE_ENV !== 'production',
            __PRODUCTION__: process.env.NODE_ENV === 'production',
            __TEST__: JSON.stringify(process.env.TEST || false),
            'process.env.NODE_ENV': JSON.stringify(process.env.NODE_ENV)
        })       
    ],
    devServer: {
        historyApiFallback: { index: '/' },
        watchOptions: { aggregateTimeout: 300, poll: 1000 },
        port: 9000,
        proxy: {
            '/api': {
                target: 'http://localhost:18088'
            }
        }
    },
    module: {
        loaders: [
            // .ts files for TypeScript
            { test: /\.ts$/, loaders: ['awesome-typescript-loader', 'angular2-template-loader'] },
            { test: /\.css$/, loader: 'style-loader!css-loader' },
            { test: /\.(png|jpe?g|gif|svg|woff|woff2|ttf|eot|ico)$/, loader: "file-loader?name=/[name].[ext]" },
            { test: /\.html$/, loader: 'raw-loader' },
            { test: /\.scss$/, exclude: /node_modules/, loaders: ['raw-loader', 'sass-loader'] }
        ],
        noParse: [/zone\.js\/dist\/.+/, /angular2\/bundles\/.+/]
    },
    node: {
        global: 1,
        crypto: 'empty',
        module: 0,
        Buffer: 0,
        clearImmediate: 0,
        setImmediate: 0
    }
};

const devConfig = {
    cache: true,
    debug: true,
    output: {
        path: path.resolve(__dirname, 'dist'),
        filename: '[name].js',
        sourceMapFilename: '[name].js.map',
        chunkFilename: '[id].chunk.js',
    },
    devtool: 'inline-source-map'
};

const prodConfig = {
    cache: false,
    debug: false,
    output: {
        path: path.resolve(__dirname, 'dist'),
        filename: '[name].[chunkhash].js',
        sourceMapFilename: '[name].[chunkhash].js.map',
        chunkFilename: '[id].chunk.js',
    },
    devtool: 'source-map'
};

module.exports = Object.assign(
  {},
  process.env.NODE_ENV === 'production' ? prodConfig : devConfig,
  baseConfig
);
