import { RegStructureBaseFormItem } from './structure-base-form-item.component';
import { TestBed, async, inject } from '@angular/core/testing';
import { TestModule } from '../../../test/test.module';
import { ChemDrawWeb } from '../common-component.module';
import { ElementRef } from '@angular/core';

describe('Component : Structure Base Form Item', () => {

  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [ TestModule ],
        declarations : [ RegStructureBaseFormItem ],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegStructureBaseFormItem);
      fixture.detectChanges();
      done();
    });
  });

  it('should create Structure Base Form Item Component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

  it('should deserialise value', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let testVal1 = 'Test Value';
      let expectedVal1 = 'Test Value';
      let testVal2 = { 'viewModel' : {'Attribute' : 'TestValue'} };
      expect(fixture.componentInstance.deserializeValue(testVal1)).toEqual(expectedVal1);
      expect(fixture.componentInstance.deserializeValue(testVal2)).toEqual(testVal2.toString());
    });
  })));

  it('should serialise value', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let testVal = {'data' : 'testdata'};
      let expectedVal = {'data' : 'testdata'};
      expect(fixture.componentInstance.serializeValue(testVal)).toEqual(expectedVal);
    });
  })));

  it('should check value on Initialize Validator', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let event = { 'component' : { 'peer' : '' } };
      let data = 'Test Data';
      fixture.componentInstance.onValidatorInitialized(event, data);
      expect(fixture.componentInstance.serializeValue(event.component.peer)).toEqual(data);
    });
  })));

  it('should Validate', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let options = { 'options' : { 'allowEditing' : false } };
      fixture.componentInstance.validate(options);
      expect(fixture.componentInstance.validate(options)).toBeTruthy();
    });
  })));

  // To do Unit Test preparation for :
  // update
  // onContentChanged

});
