import { Component } from '@angular/core';

@Component({
  selector: 'reg-login-logo',
  styles: [require('./logo.css')],
  template: `<div><a href="http://www.perkinelmer.com/">
        <span><img
          class="loginlogo"
          [src]="LogoImage"
          alt="PerkinElmer"
        /></span>
      </a></div>
      <div>
    <span>Welcome to <a>ChemBioOffice Enterprise</a></span></div>`
})
export class RegLoginLogo {
  private LogoImage = require('../../assets/pki-logo.png');
};
