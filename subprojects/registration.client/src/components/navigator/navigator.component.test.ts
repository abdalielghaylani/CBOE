import {
  async,
  inject,
  TestBed,
} from '@angular/core/testing';
import {Component} from '@angular/core';
import {By} from '@angular/platform-browser';
import {RegNavigator} from './navigator.component';
import {RegFormModule} from '../form/form.module';
import {RegNavigatorModule} from './navigator.module';
import {configureTests} from '../../tests.configure';

describe('Component: Navigator', () => {
  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [
          RegNavigatorModule,
        ],
        declarations: [
          RegNavigatorTestController
        ],
      });
    };

    configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegNavigatorTestController);
      fixture.detectChanges();
      done();
    });
  });

  it('should create the component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let query = fixture.debugElement
        .query(By.directive(RegNavigator));
      expect(query).toBeTruthy();
      expect(query.componentInstance).toBeTruthy();
    });
  })));
});

@Component({
  selector: 'test',
  template: `
    <reg-navigator></reg-navigator>
  `
})
class RegNavigatorTestController { }

