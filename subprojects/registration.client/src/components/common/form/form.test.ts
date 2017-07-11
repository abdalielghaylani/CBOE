import {
  async,
  inject,
} from '@angular/core/testing';
import { Component } from '@angular/core';
import { By } from '@angular/platform-browser';
import {
  FormGroup,
  FormControl,
  FormBuilder,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { TestBed } from '@angular/core/testing';
import { TestModule } from '../../../test';
import { RegCommonComponentModule } from '../common-component.module';
import { RegForm } from './form';

describe('Component: Form', () => {
  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [
          TestModule,
          FormsModule,
          ReactiveFormsModule,
          RegCommonComponentModule
        ],
        declarations: [
          RegFormTestController,
        ]
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegFormTestController);
      fixture.detectChanges();
      done();
    });
  });

  it('should create the component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let query = fixture.debugElement
        .query(By.directive(RegForm));
      expect(query).toBeTruthy();
      expect(query.componentInstance).toBeTruthy();
      expect(query.componentInstance.onSubmit).toBeTruthy();
      expect(query.componentInstance.group).toBeTruthy();
    });
  })));

  it('should emit event when onSubmit is invoked', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let query = fixture.debugElement
        .query(By.directive(RegForm));
      query.componentInstance.onSubmit.subscribe(c => {
        expect(c).toBeDefined();
      });
      query.nativeElement.querySelector('button').click();
    });
  })));

});

@Component({
  selector: 'test',
  template: `
    <reg-form
      [group]="group">
      <input
        [formControl]="field1">
      <button type="submit">submit</button>
    </reg-form>
  `
})
class RegFormTestController {
  private group: FormGroup;
  private field1: FormControl;
  constructor(private builder: FormBuilder) {
    this.field1 = new FormControl('test value');
    this.group = this.builder.group({
      field1: this.field1,
    });
  }
}

