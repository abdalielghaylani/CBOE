import {
  TestBed,
  inject
} from '@angular/core/testing';
import { Component } from '@angular/core';
import { By } from '@angular/platform-browser';
import { TestModule } from '../../../test';
import { RegCommonComponentModule } from '../common-component.module';
import { RegModalContent } from './modal-content.component';

describe('Component: Modal Content', () => {
  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [
          TestModule,
          RegCommonComponentModule
        ],
        declarations: [
          RegModalContentTestController
        ],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegModalContentTestController);
      fixture.detectChanges();
      done();
    });
  });

  it('should create the component', inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let query = fixture.debugElement
        .query(By.directive(RegModalContent));
      expect(query).toBeTruthy();
      expect(query.componentInstance).toBeTruthy();
    });
  }));
});

@Component({
  selector: 'test',
  template: `
    <reg-modal-content></reg-modal-content>
  `
})
class RegModalContentTestController { }

