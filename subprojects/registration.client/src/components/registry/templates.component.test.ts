import { TestBed, async, inject } from '@angular/core/testing';
import { TestModule } from '../../test/test.module';
import { DevExtremeModule } from 'devextreme-angular';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpModule, XHRBackend, RequestOptions } from '@angular/http';
import { RegTemplates } from './templates.component';
import { CommandButton } from '../../common/tool/command-button.component';
import { RegHeader, RegStructureImage } from '../index';
import { NgReduxModule, NgRedux } from '@angular-redux/store/lib';
import { MockBackend } from '@angular/http/testing/mock_backend';
import { HttpService } from '../../services/http.service';
import { IAppState } from '../../redux/index';


// Mock out the NgRedux class with just enough to test what we want.
class MockRedux extends NgRedux<IAppState> {
  constructor(private state: IAppState) {
    super(null);
  }
  dispatch = () => undefined;
  getState = () => this.state;
}


describe('Component : Templates', () => {

  let fixture;
  let mockRedux: NgRedux<IAppState>;
  let mockState : IAppState = {
    session : { token: '', user: { fullName : 'Test User Name' }, hasError: null, isLoading: null, lookups: null }
  };

  beforeEach(done => {
    mockRedux = new MockRedux(mockState);

    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [ TestModule, DevExtremeModule, 
            RouterTestingModule, HttpModule ],
        declarations : [ RegTemplates, CommandButton, RegHeader, 
            RegStructureImage ],
        providers: [
          { provide: XHRBackend, useClass: MockBackend },
          {
            provide: HttpService,
            useFactory: (backend: XHRBackend, options: RequestOptions, redux: NgRedux<IAppState> ) => {
              return new HttpService(backend, options, mockRedux );
            },
            deps: [XHRBackend, RequestOptions]
          },
          { provide: NgRedux, useValue : mockRedux }
        ]
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegTemplates);
      fixture.detectChanges();
      done();
    });

  });

  it('should instantiate Templates Component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

  it('should update vales on initialization', async(inject([], () => {
    fixture.whenStable().then(() => {
      let testInput = {username : 'Test User Name'};
      let expectedOutput = 'My Templates';
      let testInput2 = {username : 'Any other User Name'};
      let expectedOutput2 = 'Shared Templates';
      fixture.autoDetectChanges();
      fixture.componentInstance.ngOnInit();
      expect(fixture.componentInstance.columns[0].calculateCellValue(testInput)).toEqual(expectedOutput);
      expect(fixture.componentInstance.columns[0].calculateCellValue(testInput2)).toEqual(expectedOutput2);
    });
  })));

  it('should grid height', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      fixture.componentInstance.elementRef = {'nativeElement' : {'parentElement' : {'clientHeight' : 130} } };
      let expectedGridHeight = fixture.componentInstance.elementRef.nativeElement.parentElement.clientHeight - 100;
      expect(fixture.componentInstance.getGridHeight()).toEqual(expectedGridHeight.toString());
      expect(typeof fixture.componentInstance.getGridHeight()).toEqual('string');
    });
  })));

  it('should check component values on resize', async(inject([], () => {
    fixture.whenStable().then(() => {
      spyOn(fixture.componentInstance.grid.instance, 'repaint');
      fixture.autoDetectChanges();
      let expectedGridHeight = -100;      
      fixture.componentInstance.elementRef = {'nativeElement' : {'parentElement' : {'clientHeight' : 0} } };
      fixture.componentInstance.onResize({data: 'testEvent'});
      expect(fixture.componentInstance.gridHeight).toEqual(expectedGridHeight.toString());
      expect(typeof fixture.componentInstance.gridHeight).toEqual('string');
      expect(fixture.componentInstance.grid.height).toEqual(expectedGridHeight.toString());
      expect(fixture.componentInstance.grid.instance.repaint).toHaveBeenCalled();
    });
  })));

  it('should check component values on document click method call', async(inject([], () => {
    fixture.whenStable().then(() => {
      spyOn(fixture.componentInstance.grid.instance, 'repaint');
      fixture.autoDetectChanges();
      let testEvent = {srcElement : {title : 'Full Screen', className : 'fa fa-compress fa-stack-1x white'}};
      let expectedGridHeight = -10;
      fixture.componentInstance.elementRef = {'nativeElement' : {'parentElement' : {'clientHeight' : 0} } };
      fixture.componentInstance.onDocumentClick(testEvent);
      expect(fixture.componentInstance.gridHeight).toEqual(expectedGridHeight.toString());
      expect(typeof fixture.componentInstance.gridHeight).toEqual('string');
      expect(fixture.componentInstance.grid.height).toEqual(expectedGridHeight.toString());
      expect(fixture.componentInstance.grid.instance.repaint).toHaveBeenCalled();
    });
  })));

  it('should emit event on cancel', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let testEvent = { event : 'Test Event' };
      fixture.componentInstance.cancel(testEvent);
      fixture.componentInstance.onClose.subscribe(e => expect(e).toEqual(testEvent));
    });
  })));

  // To do 
  // ngOnDestroy
  // loadData
  // onContentReady
  // onCellPrepared
  // createCustomStore
  // loadTemplate
});
