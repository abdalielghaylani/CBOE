import { Component, Input } from '@angular/core';

@Component({
  selector: 'reg-footer',
  styles: [require('./footer.css')],
  template: `<div id="footer">
  <div class="container-fluid">
    <div class="row">
      <div class="col-md-8 hidden-sm hidden-xs">
        <p id="copyright">
          <a target="_blank" href="http://www.perkinelmer.com/" class="text-muted hover-gray">©2017 PerkinElmer</a>
          |
          <a target="_blank" href="http://www.perkinelmer.com/corporate/policies/privacy-policy.html" class="text-muted hover-gray">
            <u><small>Privacy Policy</small></u>
          </a>
          |
          <a href="/terms" class="text-muted hover-gray"><u><small>Terms of Service</small></u></a>
          |
          <a href="/dashboard" class="text-muted hover-gray"><u><small>Registration</small></u></a>
        </p>

      </div>
    </div>
  </div>
</div>`
})
export class RegFooter {
};
