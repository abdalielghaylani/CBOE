import { Component, Inject, ApplicationRef } from '@angular/core';
import { ConfigurationActions, IConfiguration } from '../redux';
import { RegContainer, RegConfigTables } from '../components';

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
