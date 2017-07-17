import { Component } from '@angular/core';
import { SessionActions } from '../redux';
import { RegContainer } from '../components';

@Component({
  selector: 'reg-login-page',
  template: `
    <reg-container></reg-container>
  `
})
export class RegLoginPage {
  constructor(private sessionActions: SessionActions) {
    this.checkLogin();
  }

  checkLogin() {
    // See if auth token cookie is available.
    let token = document.cookie.split('; ').map(c => c.split('=')).find(c => c[0] === 'CS%5FSEC%5FUserName');
    // If token is available, check if cookie is valid.
    this.sessionActions.checkLogin(token ? token[1] : undefined);
  }
}
