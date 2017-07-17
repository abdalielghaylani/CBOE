import { Component, Inject, ApplicationRef } from '@angular/core';
import { RegContainer, RegConfigTables } from '../components';
import { ConfigurationActions, IConfiguration } from '../redux';

@Component({
  selector: 'config-settings-page',
  providers: [ ConfigurationActions ],
  template: `
    <reg-container testid="config-settings">
      <reg-config-settings>
      </reg-config-settings>
    </reg-container>
  `
})
export class RegConfigSettingsPage {
  constructor() {}
}
