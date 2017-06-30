import {
  async,
  inject,
} from '@angular/core/testing';
import { Component } from '@angular/core';
import { By } from '@angular/platform-browser';
import { RegForm } from './form';
import {
  FormGroup,
  FormControl,
  FormBuilder,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { TestBed } from '@angular/core/testing';
import { RegCommonModule } from '../common.module';
import { configureTests } from '../../../tests.configure';

describe('Component: Form', () => {
  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [
          FormsModule,
          ReactiveFormsModule,
          RegCommonModule,
        ],
        declarations: [
          RegFormTestController,
        ]
      });
    };

    configureTests(configure).then(testBed => {
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

