'use strict';

process.env.TEST = true;

module.exports = config => {
    config.set({
        autoWatch: true,
        browsers: ['Chrome'],
        files: [
            'karma.entry.js'
        ],
        frameworks: ['jasmine'],
        logLevel: config.LOG_INFO,
        phantomJsLauncher: {
            exitOnResourceError: true
        },
        port: 9876,
        preprocessors: {
            'karma.entry.js': ['webpack', 'sourcemap']
        },
        reporters: ['dots'],
        singleRun: true,
        webpack: require('../webpack.config.js'),
        webpackServer: {
            noInfo: true
        }
    });
};
