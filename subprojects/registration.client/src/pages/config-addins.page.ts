import { Component, Inject, ApplicationRef } from '@angular/core';
import { ConfigurationActions } from '../actions';
import { RegContainer, RegConfigTables } from '../components';
import { IConfiguration } from '../store/configuration';

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
