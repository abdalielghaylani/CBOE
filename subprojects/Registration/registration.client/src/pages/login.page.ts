import { Component } from '@angular/core';
import { SessionActions } from '../redux';
import { RegContainer } from '../components';
import { Router } from '@angular/router';

@Component({
  selector: 'reg-login-page',
  template: `
    <reg-container></reg-container>
  `
})
export class RegLoginPage {
  constructor(private router: Router, private sessionActions: SessionActions) {
    let urlSegments = router.url.split('/');
    if (!!urlSegments.find(s => s === 'logout')) {
      window.location.href = location.protocol + '//' + location.host + `/COEManager/logoff.aspx`;
    } else {
      this.checkLogin();
    }
  }

  checkLogin() {
    // See if auth token cookie is available.
    let token = document.cookie.split('; ').map(c => c.split('=')).find(c => c[0] === 'CS%5FSEC%5FUserName');
    // If token is available, check if cookie is valid.
    this.sessionActions.checkLogin(token ? token[1] : undefined);
  }
}
