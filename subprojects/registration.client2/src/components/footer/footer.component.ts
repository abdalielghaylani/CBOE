import { Component, Input } from '@angular/core';

@Component({
  selector: 'reg-footer',
  template: `<div class="navbar navbar-default navbar-fixed-bottom">
          <div class="container">
            <p class="navbar-text pull-center">© Copyright 2017  
                <a href="http://www.perkinelmer.com/">PerkinElmer Informatics, Inc.</a>
            </p>
          </div>
        </div>`
})
export class RegFooter {
  // @Input() testid: string;
};
