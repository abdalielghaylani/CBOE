'use strict';

const basePath = process.env.NODE_ENV === 'production' ? '/Registration.Server/' : '/';
const path = require('path');
const proxy = require('./server/webpack-dev-proxy');
const loaders = require('./webpack/loaders');
const plugins = require('./webpack/plugins');
const UglifyJSPlugin = require('uglifyjs-webpack-plugin');
const OptimizeCSSAssetsPlugin = require('optimize-css-assets-webpack-plugin');

/*
 * Dev Config
 * ==========
 *
 * For dev, create an app entry point and a polyfills entry point
 * (need to because it lives in a file that is never imported by the app -
 * we bring it in later via CommonsChunkPlugin).
 *
 * Also, save compilation time by not adding [chunkhash]es to the filenames
 */
const devConfig = {
  mode: 'development',
  entry: {
    app: './src/index.ts',
    // polyfills: './src/polyfills.ts',
  },
  output: {
    path: path.resolve(__dirname, 'dist'),
    filename: '[name].js',
    publicPath: basePath,
    sourceMapFilename: '[name].js.map',
    chunkFilename: '[id].chunk.js',
    jsonpFunction: 'jsonpFunction',
  },
  devtool: 'inline-source-map',
};

/*
 * Prod Config
 * ===========
 *
 * For prod, create an app entry point and a polyfills entry point
 * (need to because it lives in a file that is never imported by the app - we
 * bring it in later via CommonsChunkPlugin) as well as a vendor bundle.
 *
 * The idea is that eventually, once chunk-manifest-webpack-plugin is
 * working with wp2, each chunk suffixed with its chunkhash can be
 * cached on a user's browser, ideally requiring them to only redownload
 * the vendor bundle if the vendor libraries change. Otherwise, the
 * smallest bundle, app.ts, would be the only thing they'd have to get a
 * new version of, since it's the most likely to change.
 *
 * https://medium.com/@okonetchnikov/long-term-caching-of-static-assets
 * -with-webpack-1ecb139adb95
 */
const prodConfig = {
  mode: 'production',
  entry: {
    app: './src/index.ts',
    // keep polyfills
    // polyfills: './src/polyfills.ts',
    // and vendor files separate
    vendor: [
      '@angular/core',
      '@angular/common',
      '@angular/forms',
      '@angular/http',
      '@angular/platform-browser',
      '@angular/platform-browser-dynamic',
      '@angular/router',
      'rxjs',
      '@angular-redux/store',
      '@angular-redux/router',
      'redux',
      'immutable',
      'redux-localstorage',
      'redux-observable',
      'redux-logger',
      'typed-immutable-record',
    ],
  },
  output: {
    path: path.resolve(__dirname, 'dist'),
    filename: '[name].[contenthash].js',
    publicPath: basePath,
    sourceMapFilename: '[name].[chunkhash].js.map',
    chunkFilename: '[id].chunk.js',
    jsonpFunction: 'jsonpFunction',
  },
  devtool: 'source-map',
};

const serverHost = 'http://localhost';
// const serverHost = 'http://165.88.34.121';
// const serverHost = 'http://165.88.210.101'; // QA system
// const serverHost = 'http://10.230.22.149'; // Demo system

const proxySettings = {};
proxySettings[basePath + 'api'] = {
  target: serverHost + '/Registration.Server/api',
  pathRewrite: {
    '^/api': '',
  },
};
proxySettings[basePath + 'swagger'] = {
  target: serverHost + '/Registration.Server/swagger',
  pathRewrite: {
    '^/swagger': '',
  },
};
proxySettings['/COEManager'] = serverHost;
proxySettings['/COERegistration/Webservices'] = serverHost;
proxySettings[basePath + 'COECommonResources'] = serverHost;
for (const targetUrl in proxySettings) {
  proxySettings[targetUrl.toLowerCase()] = proxySettings[targetUrl];
}

const baseConfig = {
  resolve: {
    extensions: ['.webpack.js', '.web.js', '.ts', '.js'],
  },

  plugins: plugins,
  optimization: {
    runtimeChunk: 'single',
    splitChunks: {
      chunks: 'all',
      maxInitialRequests: Infinity,
      minSize: 0,
      cacheGroups: {
        vendor: {
          test: /[\\/]node_modules[\\/](devextreme|devextreme-angular)[\\/]/,
          name: 'vendor',
          chunks: 'all',
        },
        /* vendor: {
          test: /[\\/]node_modules[\\/]/,
          name(module) {
            // get the name. E.g. node_modules/packageName/not/this/part.js
            // or node_modules/packageName
            const packageName = module.context.match(/[\\/]node_modules[\\/](.*?)([\\/]|$)/)[1];

            // npm package names are URL-safe, but some servers don't like @ symbols
            return `npm.${packageName.replace('@', '')}`;
          },
        }, */
      },
    },
    minimizer: [
      new UglifyJSPlugin({
        sourceMap: true,
        cache: true,
        parallel: true,
        uglifyOptions: {
          mangle: { keep_fnames: true },
          compress: {
            warnings: false,
          },
        },
      }),
      new OptimizeCSSAssetsPlugin({}),
    ],
  },
  devServer: {
    historyApiFallback: {
      index: basePath,
    },
    proxy: Object.assign({}, proxy(), proxySettings),
    port: process.env.PORT || 8080,
  },

  module: {
    rules: [
      loaders.tslint,
      loaders.angular,
      loaders.ts,
      loaders.html,
      loaders.rawcss,
      loaders.css,
      loaders.gif,
      loaders.png,
      loaders.svg,
      loaders.eot,
      loaders.woff,
      loaders.woff2,
      loaders.ttf,
      loaders.json,
    ],
    noParse: [ /zone\.js\/dist\/.+/, /angular2\/bundles\/.+/ ],
  },
};

module.exports = Object.assign(
  {},
  process.env.NODE_ENV === 'production' ? prodConfig : devConfig,
  baseConfig
);
