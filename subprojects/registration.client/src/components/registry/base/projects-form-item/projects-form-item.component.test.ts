

// Mock out the NgRedux class with just enough to test what we want.
import { RegProjectsFormItem } from './projects-form-item.component';
import { NgRedux } from '@angular-redux/store/lib';
import { IAppState } from '../../../../redux/index';
import { TestBed, async, inject } from '@angular/core/testing';
import { TestModule } from '../../../../test/test.module';
import { DevExtremeModule } from 'devextreme-angular';
import { projectsFormItemTestData } from './projects-form-item.component.data.test';

class MockRedux extends NgRedux<IAppState> {
  constructor(private state: IAppState) {
    super(null);
  }
  dispatch = () => undefined;
  getState = () => this.state;
}

class mockDxForm extends DevExtremeModule {
  option = function (field, value) { return true; };
}


describe('Component : Project Form Item', () => {

  let fixture;
  let mockRedux: NgRedux<IAppState>;
  let mockState: IAppState = { session: projectsFormItemTestData.sessionData };

  beforeEach(done => {
    mockRedux = new MockRedux(mockState);
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [TestModule, DevExtremeModule],
        declarations: [RegProjectsFormItem],
        providers: [
          { provide: NgRedux, useValue: mockRedux }
        ]
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegProjectsFormItem);
      fixture.detectChanges();
      done();
    });

  });

  it('should instantiate Project Form Item Component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

  it('should check de serialized value', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(Array.isArray(fixture.componentInstance.deserializeValue(projectsFormItemTestData.deSerializeInputVal))).toBeTruthy();
      expect(fixture.componentInstance.deserializeValue(projectsFormItemTestData.deSerializeInputVal)).toEqual(projectsFormItemTestData.deSerializedOutputVal);
    });
  })));

  it('should check serialized value', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.componentInstance.savedValue = projectsFormItemTestData.serializeInputSavedValue;
      fixture.autoDetectChanges();
      let serializeValReturnVal = fixture.componentInstance.serializeValue(projectsFormItemTestData.serializeInputParamValue);
      expect(fixture.componentInstance.savedValue).toEqual(projectsFormItemTestData.serializeOutputSavedValue);
      expect(serializeValReturnVal).toEqual(projectsFormItemTestData.serializeExpectedOutputParamValue);
    });
  })));

  it('should check value on update method call', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.componentInstance.viewModel = projectsFormItemTestData.viewModel;
      fixture.autoDetectChanges();
      fixture.componentInstance.update();
      expect(fixture.componentInstance.dataSource).toEqual(projectsFormItemTestData.updatedDataSource);
      expect(fixture.componentInstance.savedValue).toEqual(projectsFormItemTestData.updatedSavedValue);
      expect(fixture.componentInstance.value).toEqual(projectsFormItemTestData.deSerializedOutputVal);
      expect(Array.isArray(fixture.componentInstance.value)).toBeTruthy();
      expect(fixture.componentInstance.displayExpr).toEqual('NAME');
      expect(fixture.componentInstance.valueExpr).toEqual('PROJECTID');
    });
  })));

  it('should check attributes on values changed method call', async(inject([], () => {
    fixture.whenStable().then(() => {
      let dxComponent = new mockDxForm();
      fixture.componentInstance.savedValue = projectsFormItemTestData.serializeInputSavedValue;
      spyOn(fixture.componentInstance, 'onValueUpdated');
      fixture.autoDetectChanges();
      fixture.componentInstance.onValueChanged(projectsFormItemTestData.testEventData, { 'component': dxComponent });
      expect(fixture.componentInstance.onValueUpdated).toHaveBeenCalled();
    });
  })));

  // To do
  // All methods covered
});
