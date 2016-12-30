import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { AppModule } from './app/app.module';
import { enableProdMode } from '@angular/core';

import { production, test } from './configuration';

if (production) {
  enableProdMode();
} else {
  require('zone.js/dist/long-stack-trace-zone');
}
if (!test) {
  platformBrowserDynamic().bootstrapModule(AppModule);
}