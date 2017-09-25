import { RegSettingValueFormItem } from './setting-value-form-item.component';
import { TestModule } from '../../../test/test.module';
import { TestBed, async, inject } from '@angular/core/testing';
import { DevExtremeModule } from 'devextreme-angular';
import { Component } from '@angular/core';
import { RegBaseColumnItem } from '../../index';

describe('Component : Setting Value Form Item Component', () => {

  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [TestModule,  DevExtremeModule ],
        declarations: [ RegSettingValueFormItem, TestSettingValue ],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(TestSettingValue);
      fixture.detectChanges();
      done();
    });
  });

  it('should create Form view Component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

  it('should check values on update method call', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let expectedOutput = ['True', 'False'];
      let testComponent = new RegSettingValueFormItem();
      testComponent.viewModel = { 'key' : { 'controlType' : 'PICKLIST', 'allowedValues' : 'True|False' },
        'column' : { 'editorOptions' : { 'smallSize' : 'true'} },
        'value' : 'True' };
      testComponent['update']();
      expect(testComponent['value']).toEqual('True');
      expect(testComponent['items']).toEqual(expectedOutput);
    });
  })));


  // To do Unit Test for
  // onValueChanged : updated the grid value

});

@Component({
  selector: 'test-setting-value',
  template: '<reg-setting-value-form-item-template [viewModel]="testVModel"></reg-setting-value-form-item-template>'
})
export class TestSettingValue {
  testVModel = { 'key' : { 'controlType' : 'LIST'}, 'column' : { 'editorOptions' : { 'smallSize' : 'true'} } };
}
