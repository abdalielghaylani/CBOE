// The browser platform with a compiler
import {platformBrowserDynamic} from '@angular/platform-browser-dynamic';

// The app module
import {RegAppModule} from './app/reg-app.module';
import {enableProdMode} from '@angular/core';

import {production, test} from './configuration';

import './extensions';

if (production) {
  enableProdMode();
} else {
  require('zone.js/dist/long-stack-trace-zone');
}

if (!test) {
  // Compile and launch the module
  platformBrowserDynamic().bootstrapModule(RegAppModule);
}