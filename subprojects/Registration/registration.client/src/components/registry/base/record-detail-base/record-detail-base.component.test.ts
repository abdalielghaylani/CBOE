import { NgRedux } from '@angular-redux/store';
import { IAppState, RecordDetailActions } from '../../../../redux/index';
import { TestBed, async, inject } from '@angular/core/testing';
import { TestModule } from '../../../../test/test.module';
import { RegistryModule } from '../../..';
import { RegRecordDetailBase } from './record-detail-base.component';
import { recordDetailBaseTestData } from './record-detail-base.component.data.test';
import { RegFormGroupItemView } from '../form-group-item-view/form-group-item-view.component';
import { CommandButton } from '../../../../common/tool/command-button.component';
import { DevExtremeModule } from 'devextreme-angular';
import { Subscription } from 'rxjs';
import * as registryUtils from '../../registry.utils';

// Mock out the NgRedux class with just enough to test what we want.
class MockRedux extends NgRedux<IAppState> {
  constructor(private state: IAppState) {
    super();
  }
  dispatch = () => undefined;
  getState = () => this.state;
  configureStore = () => undefined;
  configureSubStore = () => undefined;
  provideStore = () => undefined;
  replaceReducer = () => undefined;
  select = () => undefined;
  subscribe = () => undefined;
}

describe('Component : Record Detail Base', () => {

  let fixture;
  let mockRedux: NgRedux<IAppState>;
  let mockState: IAppState = {
    session: recordDetailBaseTestData.sessionData
  };
  let mockActions: RecordDetailActions;

  beforeEach(done => {
    mockRedux = new MockRedux(mockState);
    mockActions = new RecordDetailActions(mockRedux);
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [TestModule, DevExtremeModule, RegistryModule],
        providers: [
          { provide: NgRedux, useValue: mockRedux },
          { provide: RecordDetailActions, useValue: mockActions }
        ]
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegRecordDetailBase);
      fixture.detectChanges();
      done();
    });

  });

  it('should instantiate Record Detail Base Component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

  // isNewRecord()
  it('should check if it is a new record', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      fixture.componentInstance.template = false;
      fixture.componentInstance.id = 96;
      expect(fixture.componentInstance.isNewRecord).toBeFalsy();
      fixture.componentInstance.template = true;
      fixture.componentInstance.id = 96;
      expect(fixture.componentInstance.isNewRecord).toBeTruthy();
    });
  })));

  // clearDataSubscription
  it('should clear the subscription', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      fixture.componentInstance.dataSubscription = new Subscription();
      fixture.componentInstance.clearDataSubscription();
      expect(fixture.componentInstance.dataSubscription).toBeUndefined();
    });
  })));

  // ngOnDestroy
  it('should check vales on destroy', async(inject([], () => {
    fixture.whenStable().then(() => {
      spyOn(fixture.componentInstance, 'clearDataSubscription');
      fixture.autoDetectChanges();
      fixture.componentInstance.ngOnDestroy();
      expect(fixture.componentInstance.clearDataSubscription).toHaveBeenCalled();
    });
  })));


  // ngOnChanges
  it('should check vales on changes', async(inject([], () => {
    fixture.whenStable().then(() => {
      spyOn(fixture.componentInstance, 'update');
      fixture.autoDetectChanges();
      fixture.componentInstance.ngOnChanges();
      expect(fixture.componentInstance.update).toHaveBeenCalled();
    });
  })));

  // update
  it('should check vales on update method call', async(inject([], () => {
    fixture.whenStable().then(() => {
      spyOn(fixture.componentInstance, 'updateEditMode').and.returnValue({ subscribe: () => {} });
      fixture.componentInstance.id = 101;
      fixture.componentInstance.revision = undefined;
      fixture.componentInstance.viewId = undefined;
      fixture.componentInstance.dataSubscription = undefined;
      fixture.autoDetectChanges();
      fixture.componentInstance.update();
      expect(fixture.componentInstance.updateEditMode).toHaveBeenCalled();
      expect(fixture.componentInstance.viewId).toEqual('formGroupView_101');
    });
  })));

  // onValueUpdated
  it('should check vales on update', async(inject([], () => {
    fixture.whenStable().then(() => {
      let expectedVal = { data: 'Test Event Data' };
      fixture.autoDetectChanges();
      fixture.componentInstance.onValueUpdated();
      fixture.componentInstance.valueUpdated.subscribe(e => expect(e).toEqual(expectedVal));
    });
  })));

  // updateEditMode
  it('should check vales on edit mode updated', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      fixture.componentInstance.displayMode = 'edit';
      fixture.componentInstance.updateEditMode();
      expect(fixture.componentInstance.editMode).toBeTruthy();
      fixture.componentInstance.displayMode = 'view';
      fixture.componentInstance.updateEditMode();
      expect(fixture.componentInstance.editMode).toBeFalsy();
    });
  })));

  // isDuplicateResolutionEnabled
  it('should check if the duplicate resolution has been enabled', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let testData = false;
      fixture.componentInstance.id = 101;
      fixture.componentInstance.template = false;
      fixture.componentInstance.temporary = true;
      expect(fixture.componentInstance.isDuplicateResolutionEnabled(testData)).toBeFalsy();
    });
  })));

  // get statusId()
  it('should check the retrieved status id', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      fixture.componentInstance.recordDoc = new Document();
      fixture.componentInstance.getElementValue = function () { return '1'; };
      expect(fixture.componentInstance.statusId).toEqual(1);
    });
  })));


  // To do 
  // getElementValue
  // createViewGroupContainers
  // loadRecordData
  // x2jsTool
  // getUpdatedRecord
  // save
  // saveRecordData
  // register
  // prepareRegistryRecord
  // set statusId
  // recordId
  // clear
});
