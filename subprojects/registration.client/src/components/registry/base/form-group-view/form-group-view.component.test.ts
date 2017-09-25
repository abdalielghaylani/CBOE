import { RegFormGroupView } from './form-group-view.component';
import { TestModule } from '../../../../test/test.module';
import { TestBed, async, inject } from '@angular/core/testing';
import { RegFormGroupItemView } from '../form-group-item-view/form-group-item-view.component';
import { RegFormView, RegProjectsFormItem, RegFragmentsFormItem, RegIdListFormItem, RegStructureFormItem } from '../registry-base.module';
import { DevExtremeModule } from 'devextreme-angular';
import {
  RegDateFormItem, RegDropDownFormItem, RegStructureImageFormItem, RegTextFormItem,
  RegDropDownColumnItem, RegStructureColumnItem
} from '../../../index';
import { NgReduxModule } from '@angular-redux/store/lib';
import { CommandButton } from '../../../../common/tool/command-button.component';

describe('Component : Form Group View', () => {

  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [TestModule, DevExtremeModule, NgReduxModule],
        declarations: [RegFormGroupView, RegFormGroupItemView, RegFormView, RegDateFormItem,
          RegProjectsFormItem, RegFragmentsFormItem, RegIdListFormItem, RegDropDownFormItem,
          RegStructureFormItem, RegStructureImageFormItem, RegTextFormItem, RegDropDownColumnItem,
          RegStructureColumnItem, CommandButton],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegFormGroupView);
      fixture.detectChanges();
      done();
    });
  });

  it('should create Form group view Component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

  it('should check values on ngOnChanges method call', async(inject([], () => {
    fixture.whenStable().then(() => {
      spyOn(fixture.componentInstance, 'update');
      fixture.autoDetectChanges();
      fixture.componentInstance.ngOnChanges();
      expect(fixture.componentInstance.update).toHaveBeenCalled();
    });
  })));

  it('should check value emitted on value updated', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      fixture.componentInstance.valueUpdated.subscribe(e => expect(e).toEqual(fixture.componentInstance));
    });
  })));

  // To do Unit Test for
  // validate()
});
