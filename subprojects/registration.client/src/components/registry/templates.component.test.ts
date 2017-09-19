import { RegTemplates } from './templates.component';
import { TestBed, async, inject } from '@angular/core/testing';
import { TestModule } from '../../test/test.module';
import { RegHeader, RegStructureImage } from '../index';
import { DevExtremeModule } from 'devextreme-angular';
import { CommandButton } from '../../common/tool/command-button.component';
import { NgRedux, NgReduxModule } from '@angular-redux/store/lib';
import { IAppState } from '../../redux/index';
import { Router } from '@angular/router';
import { HttpService } from '../../services/http.service';
import { ChangeDetectorRef, ElementRef, Component } from '@angular/core';
import { RouterTestingModule } from '@angular/router/testing';
import { XHRBackend, RequestOptions, HttpModule } from '@angular/http';
import { MockBackend } from '@angular/http/testing/mock_backend';

describe('Component : Templates', () => {

  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [ TestModule, DevExtremeModule, NgReduxModule, 
            RouterTestingModule, HttpModule ],
        declarations : [ RegTemplates, CommandButton, RegHeader, 
            RegStructureImage ],
        providers: [
          { provide: XHRBackend, useClass: MockBackend },
          {
            provide: HttpService,
            useFactory: (backend: XHRBackend, options: RequestOptions, redux: NgRedux<IAppState>) => {
              return new HttpService(backend, options, redux);
            },
            deps: [XHRBackend, RequestOptions, NgRedux]
          },
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

  // To do 
});
