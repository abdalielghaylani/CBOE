import { RegDataGridFormItem } from './data-grid-form-item.component';
import { TestModule } from '../../../test/test.module';
import { TestBed, async, inject } from '@angular/core/testing';
import { DevExtremeModule } from 'devextreme-angular';
import { RegDropDownColumnItem, RegStructureColumnItem } from '../common-component.module';

describe('Component : Data Grid Form Item', () => {

  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [ TestModule, DevExtremeModule ],
        declarations : [ RegDataGridFormItem, RegDropDownColumnItem, 
                          RegStructureColumnItem ],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegDataGridFormItem);
      fixture.detectChanges();
      done();
    });
  });

  it('should create Data Grid Form Item Component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

  it('should serialize value', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let testData = [{'__KEY__': '15d3dde8-4613-ce90-ea8e-855429cf84c4', 'id': 3, 'inputText': 'Test identifier'}];
      let expectedData = [{'__KEY__': '15d3dde8-4613-ce90-ea8e-855429cf84c4', 'id': 3, 'inputText': 'Test identifier'}];
      expect(fixture.componentInstance.serializeValue(testData)).toEqual(expectedData);
    });
  })));

  it('should deserialize value', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let testData = [{'__KEY__': '15d3dde8-4613-ce90-ea8e-855429cf84c4', 'id': 3, 'inputText': 'Test identifier'}];
      let expectedData = [{'__KEY__': '15d3dde8-4613-ce90-ea8e-855429cf84c4', 'id': 3, 'inputText': 'Test identifier'}];
      expect(fixture.componentInstance.deserializeValue(testData)).toEqual(expectedData);
    });
  })));

  it('should check method call on Row Inserted', async(inject([], () => {
    fixture.whenStable().then(() => {
      spyOn(fixture.componentInstance, 'onGridChanged');
      fixture.autoDetectChanges();
      let event = { 'testData' : 'event' };
      let data = { 'component' : 'DataGridComponent' };
      fixture.componentInstance.onRowInserted(event, data);
      expect(fixture.componentInstance.onGridChanged).toHaveBeenCalled();
    });
  })));

  it('should check method call on Row Updated', async(inject([], () => {
    fixture.whenStable().then(() => {
      spyOn(fixture.componentInstance, 'onGridChanged');
      fixture.autoDetectChanges();
      let event = { 'testData' : 'event' };
      let data = { 'component' : 'DataGridComponent' };
      fixture.componentInstance.onRowUpdated(event, data);
      expect(fixture.componentInstance.onGridChanged).toHaveBeenCalled();
    });
  })));

  it('should check method call on Row Removed', async(inject([], () => {
    fixture.whenStable().then(() => {
      spyOn(fixture.componentInstance, 'onGridChanged');
      fixture.autoDetectChanges();
      let event = { 'testData' : 'event' };
      let data = { 'component' : 'DataGridComponent' };
      fixture.componentInstance.onRowRemoved(event, data);
      expect(fixture.componentInstance.onGridChanged).toHaveBeenCalled();
    });
  })));

  
  // To do Unit Test preparation for :
  // checkCommandColumn
  // update
  // onContentReady
  // addRow
  // edit
  // delete
  // save
  // cancel
  // onGridChanged
  // onDropDownValueUpdated

});