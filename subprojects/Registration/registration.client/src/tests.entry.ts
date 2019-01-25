import 'zone.js/dist/zone';
import 'zone.js/dist/zone-testing';
import 'babel-polyfill';
import 'core-js/es6';
import 'core-js/es7/reflect';
import 'ts-helpers';
import 'reflect-metadata';

const testContext = (<{ context?: Function }>require)
  .context('./', true, /^(.(?!tests\.entry))*\.ts$/);

testContext('./index.ts');

testContext.keys().forEach(
  key => {
    if (/\.test\.ts$/.test(key)) {
      testContext(key);
    }
  });
