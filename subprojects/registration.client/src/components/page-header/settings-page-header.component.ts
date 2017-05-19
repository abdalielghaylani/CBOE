import { Component, Input, Output, EventEmitter } from '@angular/core';
import { NgReduxRouter } from '@angular-redux/router';

@Component({
  selector: 'reg-settings-page-header',
  template: require('./settings-page-header.component.html')
})
export class RegSettingsPageHeader {
  @Input() testid: string;
  @Input() id: string;
};
