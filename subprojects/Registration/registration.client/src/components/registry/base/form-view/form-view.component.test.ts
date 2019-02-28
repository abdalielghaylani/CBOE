import { RegFormView } from './form-view.component';
import { TestBed, async, inject } from '@angular/core/testing';
import { TestModule } from '../../../../test/test.module';
import { DevExtremeModule } from 'devextreme-angular';
import { RegistryModule } from '../..';
import {
  RegDropDownFormItem, RegDateFormItem, RegStructureImageFormItem, RegTextFormItem, 
  RegDropDownColumnItem, RegStructureImageColumnItem
} from '../../../index';

describe('Component : Form View Component', () => {

  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [TestModule, DevExtremeModule, RegistryModule],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegFormView);
      fixture.detectChanges();
      done();
    });
  });

  it('should create Form view Component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

  it('should create copy of view model on changes method call', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.componentInstance.viewModel = { data : 'Test Data' };
      fixture.autoDetectChanges();
      fixture.componentInstance.ngOnChanges();
      expect(fixture.componentInstance.viewModelCopy).toEqual(fixture.componentInstance.viewModel);
    });
  })));

  it('should emit value on value updated', async(inject([], () => {
    fixture.whenStable().then(() => {
      let testEvent = { data : 'Test Event' };
      fixture.autoDetectChanges();
      fixture.componentInstance.onValueUpdated(testEvent);
      fixture.componentInstance.valueUpdated.subscribe(e => expect(e).toEqual(testEvent));
      expect(fixture.componentInstance.viewModelCopy).toEqual(fixture.componentInstance.viewModel);
    });
  })));

  it('should check values on field data changed method call', async(inject([], () => {
    fixture.whenStable().then(() => {
      spyOn(fixture.componentInstance, 'onValueUpdated');
      let testEvent = { dataField : 'APPEARANCEProperty', value : 'Green' };
      fixture.componentInstance.viewModelCopy = { APPEARANCEProperty : undefined };
      fixture.autoDetectChanges();
      fixture.componentInstance.onFieldDataChanged(testEvent);
      expect(fixture.componentInstance.viewModelCopy[testEvent.dataField]).toEqual(testEvent.value);
      expect(fixture.componentInstance.onValueUpdated).toHaveBeenCalled();
    });
  })));

  // To do Unit Test for : none

});
