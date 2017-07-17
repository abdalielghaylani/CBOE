import { Component, Inject, ApplicationRef } from '@angular/core';
import { RegContainer, RegConfigTables } from '../components';
import { ConfigurationActions, IConfiguration } from '../redux';

@Component({
  selector: 'config-xml-forms-page',
  providers: [ ConfigurationActions ],
  template: `
    <reg-container testid="config-xml-forms">
      <reg-config-xml-forms>
      </reg-config-xml-forms>
    </reg-container>
  `
})
export class RegConfigXmlFormsPage {
  constructor() {}
}
