'use strict';
process.env.TEST = true;
// puppeteer uses Chromium package to run tests in CI environment in headless mode
process.env.CHROMIUM_BIN = require('puppeteer').executablePath();
const loaders = require('./webpack/loaders');
const plugins = require('./webpack/plugins');

module.exports = (config) => {
  const coverage = config.singleRun ? ['coverage'] : [];

  config.set({
    mime: { 'text/x-typescript': ['ts', 'tsx'] },

    frameworks: [
      'jasmine',
    ],

    browserNoActivityTimeout: 50000,
    browserDisconnectTolerance: 2,

    plugins: [
      'karma-jasmine',
      'karma-sourcemap-writer',
      'karma-sourcemap-loader',
      'karma-webpack',
      'karma-coverage',
      'karma-remap-istanbul',
      'karma-spec-reporter',
      'karma-chrome-launcher',
      'karma-junit-reporter',
    ],

    files: [
      './src/tests.entry.ts',
      {
        pattern: '**/*.map',
        served: true,
        included: false,
        watched: true,
      },
    ],

    customLaunchers: {
      ChromeHeadless: {
        base: 'Chrome',
        flags: [
          '--headless',
          '--disable-gpu',
          '--no-sandbox',
          // Without a remote debugging port, Google Chrome exits immediately.
          '--remote-debugging-port=9222',
        ],
      },
    },

    preprocessors: {
      './src/tests.entry.ts': [
        'webpack',
        'sourcemap',
      ],
      './src/**/!(*.test|tests.*).(ts|js)': [
        'sourcemap',
      ],
    },

    webpack: {
      plugins,
      entry: './src/tests.entry.ts',
      devtool: 'inline-source-map',
      resolve: {
        extensions: ['.webpack.js', '.web.js', '.ts', '.js'],
      },
      module: {
        rules:
          combinedLoaders().concat(
            config.singleRun
              ? [ loaders.istanbulInstrumenter ]
              : [ ]),
      },
      stats: { colors: true, reasons: true },
    },

    webpackServer: {
      noInfo: true, // prevent console spamming when running in Karma!
    },

    reporters: ['spec']
      .concat(coverage)
      .concat(coverage.length > 0 ? ['karma-remap-istanbul', 'junit'] : []),

    coverageReporter: {
      reporters: [
        { type: 'html' },
      ],
      dir: './test-coverage/',
      subdir: (browser) => {
        return browser.toLowerCase().split(/[ /-]/)[0]; // returns 'chrome'
      },
    },

    junitReporter: {
      outputDir: require('path').join(__dirname, './test-output'),
      outputFile: 'test-results.xml',
      suite: 'Registration Web Client Tests',
      useBrowserName: false,
    },

    port: 9999,
    colors: true,
    logLevel: config.LOG_INFO,
    autoWatch: true,
    browsers: ['Chrome', 'ChromiumHeadless'],
    captureTimeout: 6000,
  });
};

function combinedLoaders() {
  return Object.keys(loaders).reduce(function reduce(aggregate, k) {
    switch (k) {
    case 'istanbulInstrumenter':
    case 'tslint':
      return aggregate;
    case 'ts':
    case 'tsTest':
      return aggregate.concat([ // force inline source maps
        Object.assign(loaders[k],
          { query: { babelOptions: { sourceMaps: 'both' } } })]);
    default:
      return aggregate.concat([loaders[k]]);
    }
  },
  []);
}
