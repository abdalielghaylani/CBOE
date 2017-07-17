import { Component, Inject, ApplicationRef } from '@angular/core';
import { ConfigurationActions, IConfiguration } from '../redux';
import { RegContainer, RegConfigTables } from '../components';

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
