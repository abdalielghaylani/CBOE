import { Component } from '@angular/core';
import { RegContainer } from '../components';

@Component({
  selector: 'reg-about-page',
  template: `
    <reg-container [size]=4 [center]=true>
      <h2 class="caps">About Us</h2>
      <p>
        Rangle.io is a next-generation HTML5 design and development firm
        dedicated to modern, responsive web and mobile applications.
      </p>
    </reg-container>
  `
})
export class RegAboutPage {}
