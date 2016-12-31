import { Component } from '@angular/core';

@Component({
  selector: 'reg-logo',
  styles: [require('./logo.css')],
  template: `
    <div class="col-sm-12">
      <a class="navbar-brand" href="">
        <span><img
          class="logo"
          [src]="LogoImage"
          alt="PerkinElmer"
        /></span> CBOE: Registration
      </a>
    </div>
  `
})
export class RegLogo {
  private LogoImage = require('../../assets/pki-logo.png');
};
