
import { TestBed, async, inject } from '@angular/core/testing';
import { TestModule } from '../../../test/test.module';
import { RegValidationFormItem } from './validation-type-form-item.component';
import { DevExtremeModule } from 'devextreme-angular';
import { NgReduxModule } from '@angular-redux/store/lib';

describe('Component : Validation Type Form Item', () => {

  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [ TestModule, DevExtremeModule, NgReduxModule  ],
        declarations : [ RegValidationFormItem ],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegValidationFormItem);
      fixture.detectChanges();
      done();
    });

  });

  it('should instantiate Validation Type Form Item Component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

  it('should check values on de serialize method call', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let inputItemValue = 'textLength';
      let expectedItemValue = 'textLength';
      expect(fixture.componentInstance.deserializeValue(inputItemValue)).toEqual(expectedItemValue);
    });
  })));

  it('should check values on update method call', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let expectedItemValue = ['requiredField', 'textLength', 'wordListEnumeration',
        'notEmptyStructure', 'notEmptyStructureAndNoText', 'custom'];
      fixture.componentInstance.viewConfig = { 'type' : 'TEXT' };
      fixture.componentInstance.viewModel = { 'editorOptions' : { 'value' : 'textLength', 'inputAttr' :  { 'id' : 'test_id' }, 'name' : 'name' } };
      fixture.componentInstance.update();
      expect(fixture.componentInstance.items).toEqual(expectedItemValue);
      expect(fixture.componentInstance.value).toEqual(fixture.componentInstance.viewModel.editorOptions.value);
    });
  })));

  // To do 
});
