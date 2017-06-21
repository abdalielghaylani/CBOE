import { Component, Inject, ApplicationRef } from '@angular/core';
import { ConfigurationActions } from '../actions';
import { RegContainer, RegConfigTables } from '../components';
import { IConfiguration } from '../store/configuration';

@Component({
  selector: 'config-forms-page',
  providers: [ ConfigurationActions ],
  template: `
    <reg-container testid="config-forms">
      <reg-config-forms>
      </reg-config-forms>
    </reg-container>
  `
})
export class RegConfigFormsPage {
  constructor() {}
}
