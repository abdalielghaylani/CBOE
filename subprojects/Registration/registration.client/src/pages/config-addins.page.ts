import { Component, Inject, ApplicationRef } from '@angular/core';
import { RegContainer, RegConfigTables } from '../components';
import { ConfigurationActions, IConfiguration } from '../redux';

@Component({
  selector: 'config-addins-page',
  providers: [ ConfigurationActions ],
  template: `
    <reg-container testid="config-addins">
      <reg-config-addins>
      </reg-config-addins>
    </reg-container>
  `
})
export class RegConfigAddinsPage {
  constructor() {}
}
