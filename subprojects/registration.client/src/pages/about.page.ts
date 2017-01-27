import { Component } from '@angular/core';
import { RegContainer } from '../components';

@Component({
  selector: 'reg-about-page',
  template: `
    <reg-container>
      <h2 class="caps">About CBOE:Registration Client</h2>
      <p>
        CBOE:Registration is a web-based client application for PerkinElmer Chemical Registration system.
      </p>
    </reg-container>
  `
})
export class RegAboutPage {}
