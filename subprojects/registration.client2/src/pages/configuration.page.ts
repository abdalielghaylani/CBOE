import { Component, Inject, ApplicationRef } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { Observable } from 'rxjs/Observable';
import { select } from 'ng2-redux';
import { ConfigurationActions } from '../actions';
import { RegContainer, RegConfiguration } from '../components';
import { ICounter } from '../store';

@Component({
  selector: 'configuration-page',
  providers: [ ConfigurationActions ],
  template: `
    <reg-container testid="configuration" [size]=2 [center]=true>
      <h2 data-testid="configuration-heading" id="qa-configuration-heading"
        class="center caps">
        Configuration
      </h2>

      <reg-configuration
        (create)="actions.create()"
        (edit)="actions.edit()">
      </reg-configuration>
    </reg-container>
  `
})
export class RegConfigurationPage {
  @select() private counter$: Observable<ICounter>;
  constructor(private actions: ConfigurationActions) {}
}
