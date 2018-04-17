import { IAppState } from '../../../../redux/index';
import { NgRedux } from '@angular-redux/store/lib';
import { TestBed, async, inject } from '@angular/core/testing';
import { TestModule } from '../../../../test/test.module';
import { RegBatchSelector } from './batch-selector.component';
import { DevExtremeModule } from 'devextreme-angular';
import { CommandButton } from '../../../../common/tool/command-button.component';

// Mock out the NgRedux class with just enough to test what we want.
class MockRedux extends NgRedux<IAppState> {
  constructor(private state: IAppState) {
    super(null);
  }
  dispatch = () => undefined;
  getState = () => this.state;
}

describe('Component : Batch Selector', () => {

  let fixture;
  let mockRedux: NgRedux<IAppState>;
  let mockState: IAppState = {
    session: {
      'token': '',
      'user': { 'fullName': 'Test User Name' },
      'hasError': null,
      'isLoading': null,
      'lookups': {
        'users': [
          { 'PERSONID': 67, 'USERID': 'T4_85', 'SITEID': 0, 'ACTIVE': 'T' },
          { 'PERSONID': 68, 'USERID': 'T5_85', 'SITEID': 0, 'ACTIVE': 'T' }
        ]
      }
    }
  };

  beforeEach(done => {
    mockRedux = new MockRedux(mockState);
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [TestModule, DevExtremeModule],
        declarations: [RegBatchSelector, CommandButton],
        providers: [
          { provide: NgRedux, useValue: mockRedux }
        ]
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegBatchSelector);
      fixture.detectChanges();
      done();
    });

  });

  it('should instantiate Batch Selector Component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

  it('should show Selector', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      fixture.componentInstance.showForm({ 'data': 'Test Event' });
      expect(fixture.componentInstance.formVisible).toBeTruthy();
    });
  })));

  it('should update values on Select Batch method', async(inject([], () => {
    fixture.whenStable().then(() => {
      let testEvent = {
        columns: [{ dataField: 'BatchID' }], values: ['3301', '1',
          'AB-003111/01', '2017-09-05T18:30:00.000Z', '2017-09-12T18:30:00.000Z',
          { '_displayName': 'T5_85', '__text': '68' }]
      };
      let expectedTestValue = '3301';
      fixture.autoDetectChanges();
      fixture.componentInstance.selectBatch(testEvent);
      expect(fixture.componentInstance.formVisible).toBeFalsy();
      fixture.componentInstance.onSelected.subscribe(e => expect(e).toEqual(expectedTestValue));
    });
  })));

  // To do
});

