import { Component } from '@angular/core';

@Component({
  selector: 'reg-logo',
  styles: [require('./logo.css')],
  template: `<div><a href="http://www.perkinelmer.com/">
        <span><img
          class="logo"
          [src]="LogoImage"
          alt="PerkinElmer"
        /></span>
      </a></div>
      <div></div>`
})
export class RegLogo {
  public get LogoImage() {
    return RegLogo.Image;
  }
  public static Image = require('./registration.svg');
};
