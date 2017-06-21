import { Component, Inject, ApplicationRef } from '@angular/core';
import { ConfigurationActions } from '../actions';
import { RegContainer, RegConfigTables } from '../components';
import { IConfiguration } from '../store/configuration';

@Component({
  selector: 'config-tables-page',
  providers: [ ConfigurationActions ],
  template: `
    <reg-container testid="config-tables">
      <reg-config-tables>
      </reg-config-tables>
    </reg-container>
  `
})
export class RegConfigTablesPage {
  constructor() {}
}
