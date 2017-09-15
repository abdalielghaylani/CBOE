import { RegSearchFormView } from './search-form-view.component';
import { TestBed, async, inject } from '@angular/core/testing';
import { TestModule } from '../../../../test/test.module';
import { DevExtremeModule } from 'devextreme-angular';
import { RegDateFormItem, RegDropDownFormItem, RegTextFormItem, RegDropDownColumnItem, RegStructureColumnItem } from '../../../index';
import { RegProjectsFormItem, RegFragmentsFormItem, RegIdListFormItem } from '../registry-base.module';
import { RegStructureQueryFormItem } from '../structure-query-form-item/structure-query-form-item.component';
import { CommandDropdown } from '../../../../common/tool/command-dropdown.component';
import { RegStructureQueryOptions } from '../structure-query-form-item/structure-query-form-options.component';
import { CommandButton } from '../../../../common/tool/command-button.component';

describe('Component : Search Form View', () => {

  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [ TestModule, DevExtremeModule ],
        declarations : [ RegSearchFormView, RegDateFormItem, 
          RegProjectsFormItem, RegFragmentsFormItem, RegIdListFormItem,
          RegDropDownFormItem, RegStructureQueryFormItem, RegTextFormItem,
          RegDropDownColumnItem, CommandDropdown, RegStructureQueryOptions, 
          CommandButton, RegStructureColumnItem ],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegSearchFormView);
      fixture.detectChanges();
      done();
    });
  });

  it('Check Component is created for Search Form View', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

  it('should check if values updated on Field data changed event call', async(inject([], () => {
    fixture.whenStable().then(() => {
      spyOn(fixture.componentInstance, 'onValueUpdated');
      fixture.autoDetectChanges();
      let testEvent = { 'data' : 'Test Data', 'value' : { 'structureCriteriaOptions' : { 'option' : 'TestOption 1' } } };
      fixture.componentInstance.onFieldDataChanged(testEvent);
      expect(fixture.componentInstance.onValueUpdated).toHaveBeenCalled();
    });
  })));

  it('should check event is emitted on value updated method call', async(inject([], () => {
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
