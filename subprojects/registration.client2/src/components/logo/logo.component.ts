import { Component } from '@angular/core';

@Component({
  selector: 'reg-logo',
  styles: [require('./logo.css')],
  template: `
    <div className="flex items-center">
      <img
        class="logo"
        [src]="LogoImage"
        alt="Rangle.io"
      />
    </div>
  `
})
export class RegLogo {
  private LogoImage = require('../../assets/rangleio-logo.svg');
};
