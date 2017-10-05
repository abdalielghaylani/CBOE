import { NgRedux } from '@angular-redux/store/lib';
import { IAppState } from '../../../../redux/index';
import { TestBed, async, inject } from '@angular/core/testing';
import { TestModule } from '../../../../test/test.module';
import { RegIdListFormItem } from './id-list-form-item.component';
import { DevExtremeModule } from 'devextreme-angular';
import { RegDropDownColumnItem, RegStructureColumnItem } from '../../../index';
import { idListTestData } from './id-list-form-item.component.data.test';

// Mock out the NgRedux class with just enough to test what we want.
class MockRedux extends NgRedux<IAppState> {
  constructor(private state: IAppState) {
    super(null);
  }
  dispatch = () => undefined;
  getState = () => this.state;
}


describe('Component : ID List Form Item', () => {

  let fixture;
  let mockRedux: NgRedux<IAppState>;
  let mockState : IAppState = {
    session : idListTestData.sessionData
  };

  beforeEach(done => {
    mockRedux = new MockRedux(mockState);
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [ TestModule, DevExtremeModule ],
        declarations : [ RegIdListFormItem, RegDropDownColumnItem, RegStructureColumnItem ],
        providers: [
          { provide: NgRedux, useValue : mockRedux }
        ]
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegIdListFormItem);
      fixture.detectChanges();
      done();
    });

  });

  it('should instantiate ID List Form Item Component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

  it('should check values on deserializeValue method call', async(inject([], () => {
    fixture.whenStable().then(() => {
      let testInputVal = idListTestData.deSerializeInputVal;
      let expectedOutputVal = idListTestData.deSerializedOutputVal;
      fixture.autoDetectChanges();
      expect(fixture.componentInstance.deserializeValue(testInputVal)).toEqual(expectedOutputVal);
      let testInput2 = ''; 
      let expectedOutput2 = [];
      expect(fixture.componentInstance.deserializeValue(testInput2)).toEqual(expectedOutput2);
    });
  })));

  it('should check values on update method call', async(inject([], () => {
    fixture.whenStable().then(() => {
      spyOn(fixture.componentInstance, 'checkCommandColumn');
      fixture.componentInstance.viewModel = idListTestData.viewModel;
      fixture.autoDetectChanges();
      fixture.componentInstance.update();
      expect(fixture.componentInstance.checkCommandColumn).toHaveBeenCalled();
      expect(fixture.componentInstance.editingMode).toBe(idListTestData.expectedEditingMode);
      expect(fixture.componentInstance.allowUpdating).toBeTruthy();
      expect(fixture.componentInstance.allowDeleting).toBeTruthy();
      expect(fixture.componentInstance.allowAdding).toBeTruthy();
    });
  })));

  // To do 
  // Serialize method
});
