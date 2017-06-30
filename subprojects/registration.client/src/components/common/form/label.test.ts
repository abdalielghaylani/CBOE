import {
  async,
  inject,
  TestBed,
} from '@angular/core/testing';
import { Component } from '@angular/core';
import { By } from '@angular/platform-browser';
import { RegLabel } from './label';
import { RegCommonComponentModule } from '../common-component.module';
import { configureTests } from '../../../tests.configure';

describe('Component: Label', () => {
  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [RegCommonComponentModule],
        declarations: [
          RegLabelTestController
        ],
      });
    };

    configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegLabelTestController);
      fixture.detectChanges();
      done();
    });
  });

  it('should create the component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let query = fixture.debugElement
        .query(By.directive(RegLabel));
      expect(query).toBeTruthy();
      expect(query.componentInstance).toBeTruthy();
    });
  })));

  it('should set the id to qaid value', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let query = fixture.debugElement
        .query(By.directive(RegLabel));
      expect(query.nativeElement.querySelector('label')
        .getAttribute('id')).toBe('test-1');
    });
  })));
});

@Component({
  selector: 'test',
  template: `
    <reg-label
      qaid="test-1">
    </reg-label>
  `
})
class RegLabelTestController { }

