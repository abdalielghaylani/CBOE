import { Component, Input } from '@angular/core';

@Component({
  selector: 'reg-footer',
  styles: [require('./footer.css')],
  template: require('./footer.component.html')
})
export class RegFooter {
  @Input() cssClass: string;
}
