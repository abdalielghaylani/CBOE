import { RegFragmentsFormItem } from './fragments-form-item.component';
import { TestBed, async, inject } from '@angular/core/testing';
import { TestModule } from '../../../../test/test.module';
import { DevExtremeModule } from 'devextreme-angular';
import { RegDropDownColumnItem, RegStructureColumnItem } from '../../../index';
import { NgRedux } from '@angular-redux/store';
import { IAppState } from '../../../../redux/index';
import { HttpService } from '../../../../services/http.service';
import { XHRBackend, RequestOptions } from '@angular/http';
import { fragmentsTestData } from './fragments-form-item.component.data.test';

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


describe('Component : Fragments Form Item', () => {

  let fixture;
  let mockRedux: NgRedux<IAppState>;
  let mockState : IAppState = {
    session : fragmentsTestData.sessionData,
  };

  beforeEach(done => {
    mockRedux = new MockRedux(mockState);
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [ TestModule, DevExtremeModule  ],
        declarations : [ RegFragmentsFormItem, RegDropDownColumnItem, RegStructureColumnItem ],
        providers: [
          { provide: NgRedux, useValue : mockRedux }
        ]
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegFragmentsFormItem);
      fixture.detectChanges();
      done();
    });

  });

  it('should instantiate Fragments Form Item Component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

  it('should check values on update method call', async(inject([], () => {
    fixture.whenStable().then(() => {
      spyOn(fixture.componentInstance, 'checkCommandColumn');
      fixture.componentInstance.viewModel = { editorOptions : { 'value' : '', 'readOnly' : false,
        'inputAttr' : { 'id' : 'dx_dx-63118ce9-24e6-04b9-deb2-97e3614fff6d_View_FragmentsControl' },
        'name' : 'View_FragmentsControl' } };
      fixture.autoDetectChanges();
      fixture.componentInstance.update();
      let expectedOutput = [];
      expect(fixture.componentInstance.dataSource).toEqual(expectedOutput);
      expect(fixture.componentInstance.checkCommandColumn).toHaveBeenCalled();
      expect(fixture.componentInstance.editingMode).toBe('row');
      expect(typeof fixture.componentInstance.editingMode).toBe('string');
      expect(fixture.componentInstance.allowUpdating).toBeTruthy();
      expect(fixture.componentInstance.allowDeleting).toBeTruthy();
      expect(fixture.componentInstance.allowAdding).toBeTruthy();
    });
  })));

  // To do 
  // serialize
  // deserialize
});
