import { TestBed, async, inject } from '@angular/core/testing';
import { TestModule } from '../../../../test/test.module';
import { RegBulkRegisterPopup } from './bulk-register-popup.component';
import { DevExtremeModule } from 'devextreme-angular';
import { Component } from '@angular/core';


describe('Component : Bulk Register Popup', () => {

  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [TestModule, DevExtremeModule],
        declarations: [RegBulkRegisterPopup, TestBulkRegisterComponent],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(TestBulkRegisterComponent);
      fixture.detectChanges();
      done();
    });

  });

  it('should instantiate Bulk Register Popup Component', async(inject([], () => {
    fixture.whenStable().then(() => {
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

  it('should check values emitted on register method call', async(inject([], () => {
    fixture.whenStable().then(() => {
      let testComponent: any = new RegBulkRegisterPopup(null);
      let testEvent = { 'data': 'Test Event' };
      testComponent.register(testEvent);
      testComponent.registerMarked.subscribe(e => expect(e).toEqual(testComponent.viewModel));
    });
  })));

  it('should check values on cancel register mark method call', async(inject([], () => {
    fixture.whenStable().then(() => {
      let testComponent: any = new RegBulkRegisterPopup(null);
      testComponent.viewModel = { 'description': 'Test Description', 'option': 'Test Option', 'isVisible': false };
      testComponent.cancelRegisterMarked();
      expect(testComponent.viewModel.isVisible).toBeFalsy();
    });
  })));

  // To do 
});


@Component({
  selector: 'test-bulk-reg',
  template: '<reg-bulk-register-popup [viewModel]="viewModel"></reg-bulk-register-popup>'
})

export class TestBulkRegisterComponent {
  viewModel = { isVisible: true };
}
