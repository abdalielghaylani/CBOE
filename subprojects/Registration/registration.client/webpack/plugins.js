'use strict';

const webpack = require('webpack');
const CleanWebpackPlugin = require('clean-webpack-plugin');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const StyleLintPlugin = require('stylelint-webpack-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');

const postcss = require('./postcss');

const path = require('path');
const FilterWarningsPlugin = require('webpack-filter-warnings-plugin');

const sourceMap = process.env.TEST
  ? [new webpack.SourceMapDevToolPlugin({ filename: null, test: /\.ts$/ })]
  : [ ];

const basePlugins = [
  new CleanWebpackPlugin(['dist']),
  new webpack.HashedModuleIdsPlugin(),
  new webpack.ProvidePlugin({
    $: 'jquery',
    jQuery: 'jquery',
  }),
  new webpack.DefinePlugin({
    __DEV__: process.env.NODE_ENV !== 'production',
    __PRODUCTION__: process.env.NODE_ENV === 'production',
    __TEST__: JSON.stringify(process.env.TEST || false),
    'process.env.NODE_ENV': JSON.stringify(process.env.NODE_ENV),
  }),
  new HtmlWebpackPlugin({
    template: './src/index.html',
    inject: 'body',
    minify: false,
    chunksSortMode: 'dependency',
  }),
  new CopyWebpackPlugin([
    { from: 'src/assets', to: 'assets' },
  ]),
  new webpack.LoaderOptionsPlugin({
    test: /\.css$/,
    options: {
      postcss,
    },
  }),
  new webpack.ContextReplacementPlugin(
    /angular[\/\\]core[\/\\](esm\/src|src)[\/\\]linker/, __dirname),
  new webpack.ContextReplacementPlugin(
    /\@angular(\\|\/)core(\\|\/)fesm5/, path.join(__dirname, './src')),
  new FilterWarningsPlugin({
    exclude: /System.import/,
  }),
  new MiniCssExtractPlugin({
    // Options similar to the same options in webpackOptions.output
    // both options are optional
    filename: process.env.NODE_ENV === 'production' ?  '[name].[hash].css' : '[name].css',
    chunkFilename: process.env.NODE_ENV === 'production' ?  '[id].[hash].css' : '[id].css',
  }),
].concat(sourceMap);

const devPlugins = [
  new StyleLintPlugin({
    configFile: './.stylelintrc',
    files: 'src/**/*.css',
    failOnError: false,
  }),
];

const prodPlugins = [
];

module.exports = basePlugins
  .concat(process.env.NODE_ENV === 'production' ? prodPlugins : [])
  .concat(process.env.NODE_ENV === 'development' ? devPlugins : []);
