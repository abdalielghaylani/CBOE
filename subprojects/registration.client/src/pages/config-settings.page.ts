import { Component, Inject, ApplicationRef } from '@angular/core';
import { ConfigurationActions } from '../actions';
import { RegContainer, RegConfigTables } from '../components';
import { IConfiguration } from '../store/configuration';

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
