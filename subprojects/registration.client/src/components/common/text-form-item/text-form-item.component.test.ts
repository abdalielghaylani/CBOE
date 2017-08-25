import { TestBed, async, inject } from '@angular/core/testing';
import { DevExtremeModule } from 'devextreme-angular';
import { TestModule } from '../../../test/test.module';
import { RegTextFormItem } from './text-form-item.component';

describe('Component : Text Form Item', () => {

  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [ TestModule, DevExtremeModule ],
        declarations : [ RegTextFormItem ],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegTextFormItem);
      fixture.detectChanges();
      done();
    });
  });

  it('should create Text From Item Component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

  it('should deserialize text value', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let testData = 'NOTEBOOKREF123';
      let expectedDate = 'NOTEBOOKREF123';
      expect(fixture.componentInstance.deserializeValue(testData))
        .toEqual(expectedDate);
    });
  })));

  it('should serialize text value', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let testData = 'NOTEBOOKREF123 Test';
      let expectedDate = 'NOTEBOOKREF123 Test';
      expect(fixture.componentInstance.serializeValue(testData))
        .toEqual(expectedDate);
    });
  })));

  it('should update values to component\'s value and readonly attributes', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      fixture.componentInstance.viewModel.editorOptions = { 'value' : 'NOTEBOOKREF123', 'readOnly' : true, 
      'inputAttr': {'id': 'dx_dx-834fed21-18ee-c187-de10-b11038eaba57_NOTEBOOK_TEXTProperty'}, 'name': 'NOTEBOOK_TEXTProperty'};
      fixture.componentInstance.editMode = false;
      fixture.componentInstance.update();
      expect(fixture.componentInstance.value).toEqual('NOTEBOOKREF123');
      expect(fixture.componentInstance.readOnly).toBeTruthy();
    });
  })));

});
