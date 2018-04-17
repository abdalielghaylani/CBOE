import { Component } from '@angular/core';
import { RegContainer } from '../components';

@Component({
  selector: 'unauthorized-page',
  template: `
    <reg-container>     
      <div style="margin-top:40px"  class="caps text-center red">      
       <p> You are not authorized to access Registration system.</p>
      </div>
    </reg-container>
  `
})
export class UnAuthorizedPage {
  constructor() { }
}
