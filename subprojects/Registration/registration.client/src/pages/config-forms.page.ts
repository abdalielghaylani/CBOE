import { Component, Inject, ApplicationRef } from '@angular/core';
import { ConfigurationActions, IConfiguration } from '../redux';
import { RegContainer, RegConfigTables } from '../components';

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
