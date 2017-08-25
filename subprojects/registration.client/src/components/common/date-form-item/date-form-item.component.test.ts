import { Component } from '@angular/core';
import { TestBed, async, inject } from '@angular/core/testing';
import { TestModule } from '../../../test/test.module';
import { By } from '@angular/platform-browser';
import { RegDateFormItem } from './date-form-item.component';
import { DevExtremeModule } from 'devextreme-angular';

describe('Component : Date Form Item', () => {

  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [ TestModule, DevExtremeModule ],
        declarations : [ RegDateFormItem ],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegDateFormItem);
      fixture.detectChanges();
      done();
    });
  });

  it('should create Date Form Item Component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

  it('should deserialize date value', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let testDate = '2017-07-20 09:19:10 AM';
      let expectedDate = new Date('Thu Jul 20 2017 14:49:10 GMT+0530 (India Standard Time)');
      expect(fixture.componentInstance.deserializeValue(testDate))
        .toEqual(expectedDate);
    });
  })));

  it('should serialize date value', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      fixture.componentInstance.type = 'date';
      fixture.componentInstance.value = new Date('Wed Aug 23 2017 18:57:53 GMT+0530 (India Standard Time)');
      expect(fixture.componentInstance.serializeValue(fixture.componentInstance.value))
        .toBe('2017-08-23 12:27:53 PM');
    });
  })));

  it('should update values to component\'s value and readonly attributes', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      fixture.componentInstance.viewModel.editorOptions = { 'value': '2017-07-20 09:19:10 AM',
      'readOnly': true, 'inputAttr': { 'id': 'dx_dx-4054c099-dcb6-8203-0250-62ef1a923d76_DateCreatedTextBox'},
      'name': 'DateCreatedTextBox'};
      fixture.componentInstance.editMode = false;
      let expectedDate = new Date('Thu Jul 20 2017 14:49:10 GMT+0530 (India Standard Time)');
      fixture.componentInstance.update();
      expect(fixture.componentInstance.value).toEqual(expectedDate);
      expect(fixture.componentInstance.readOnly).toBeTruthy();
    });
  })));

});
