import { Component, Inject, ApplicationRef } from '@angular/core';
import { ConfigurationActions } from '../actions';
import { RegContainer, RegConfigTables } from '../components';
import { IConfiguration } from '../store/configuration';

@Component({
  selector: 'config-properties-page',
  providers: [ ConfigurationActions ],
  template: `
    <reg-container testid="config-properties">
      <reg-config-properties>
      </reg-config-properties>
    </reg-container>
  `
})
export class RegConfigPropertiesPage {
  constructor() {}
}
