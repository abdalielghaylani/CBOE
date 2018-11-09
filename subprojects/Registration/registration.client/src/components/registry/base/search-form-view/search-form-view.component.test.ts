import { TestBed, async, inject } from '@angular/core/testing';
import { TestModule } from '../../../../test/test.module';
import { DevExtremeModule } from 'devextreme-angular';
import { RegBaseComponentModule, RegSearchFormView } from '..';
import { CdjsService } from '../../../../services';
import { HttpModule } from '@angular/http';
import { IAppState } from '../../../../redux/index';
import { NgRedux } from '@angular-redux/store/lib';

class MockRedux extends NgRedux<IAppState> {
  constructor(private state: IAppState) {
    super(null);
  }
  dispatch = () => undefined;
  getState = () => this.state;
}

describe('Component : Search Form View', () => {
  let mockRedux: NgRedux<IAppState>;
  let mockState: IAppState = {
    session: {
      'token': '',
      'user': { 'fullName': 'Test User Name' },
      'hasError': null,
      'isLoading': null,
      'lookups': {
        'systemInformation': {
          'CDJSUrl': 'http://localhost'
        }
      }
    }
  };
  let fixture;

  beforeEach(done => {
    mockRedux = new MockRedux(mockState);
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [ TestModule, DevExtremeModule, RegBaseComponentModule, HttpModule ],
        providers: [ CdjsService, { provide: NgRedux, useValue: mockRedux } ]
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegSearchFormView);
      fixture.detectChanges();
      done();
    });
  });

  it('Check Component is created for Search Form View', async(inject([CdjsService], (service) => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

  it('should check if values updated on Field data changed event call', async(inject([CdjsService], (service) => {
    fixture.whenStable().then(() => {
      spyOn(fixture.componentInstance, 'onValueUpdated');
      fixture.autoDetectChanges();
      const value1 = {
        structureCriteriaOptions: {
          option: 'TestOption 1'
        }
      };
      const value2 = {
        structureCriteriaOptions: {
          option: 'TestOption 2'
        }
      };
      fixture.componentInstance.viewModelCopy = { value: value1 };
      const testEvent = {
        data: 'Test Data',
        dataField: 'value',
        value: value2
      };
      fixture.componentInstance.onFieldDataChanged(testEvent);
      expect(fixture.componentInstance.onValueUpdated).toHaveBeenCalled();
    });
  })));

  it('should check event is emitted on value updated method call', async(inject([CdjsService], (service) => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let testEvent = { 'data' : 'Test Data' };
      fixture.componentInstance.onValueUpdated(testEvent);
      fixture.componentInstance.valueUpdated.subscribe( e => expect(e).toEqual(testEvent));
    });
  })));

  

  // Need to implement test cases for:
  // 1) on value updated method if condition workflow
});
