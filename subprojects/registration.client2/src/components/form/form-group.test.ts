import {
  async,
  inject,
  TestBed,
} from '@angular/core/testing';
import {Component} from '@angular/core';
import {By} from '@angular/platform-browser';
import {RegFormGroup} from './form-group';
import {RegFormModule} from './form.module';
import {RegAppModule} from '../../app/reg-app.module';
import {configureTests} from '../../tests.configure';

describe('Component: Form Group', () => {
  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [RegFormModule],
        declarations: [RegFormGroupTestController],
      });
    };

    configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegFormGroupTestController);
      fixture.detectChanges();
      done();
    });
  });

  it('should create the component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let query = fixture.debugElement
        .query(By.directive(RegFormGroup));
      expect(query).toBeTruthy();
      expect(query.componentInstance).toBeTruthy();
    });
  })));
});

@Component({
  selector: 'test',
  template: `
    <reg-form-group
      qaid="test-1">
    </reg-form-group>
  `
})
class RegFormGroupTestController { }

