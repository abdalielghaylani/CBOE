import { RegStructureQueryFormItem } from './structure-query-form-item.component';
import { DevExtremeModule } from 'devextreme-angular';
import { CommandDropdown } from '../../../../common/tool/command-dropdown.component';
import { CommandButton } from '../../../../common/tool/command-button.component';
import { RegStructureQueryOptions } from './structure-query-form-options.component';
import { TestBed, async, inject } from '@angular/core/testing';
import { TestModule } from '../../../../test/test.module';
import { StructQueryTestData } from './structure-query-form-item.component.data.test';

describe('Component : Structure Query Form Item', () => {

  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [ TestModule, DevExtremeModule ],
        declarations : [ RegStructureQueryFormItem, CommandDropdown,
          CommandButton, RegStructureQueryOptions ],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegStructureQueryFormItem);
      fixture.detectChanges();
      done();
    });
  });

  it('should instantiate Structure Query Form Item component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

  it('should check index on viewOptions', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let testEvent = { 'data' : 'Test Data' };
      fixture.componentInstance.viewOptions(testEvent);
      expect(fixture.componentInstance.index).toEqual(testEvent);
    });
  })));

  it('should check values on onValueUpdated method call', async(inject([], () => {
    fixture.whenStable().then(() => {
      spyOn(fixture.componentInstance, 'setStructureSearchOptions');
      fixture.autoDetectChanges();
      let testEvent = { 'data' : 'Test Data' };
      fixture.componentInstance.viewModel = undefined;
      fixture.componentInstance.onValueUpdated(testEvent);
      expect(fixture.componentInstance.setStructureSearchOptions).toHaveBeenCalled();
      fixture.componentInstance.valueUpdated.subscribe(e => expect(e).toEqual(fixture.componentInstance));
    });
  })));

  it('should check values set using setStructureSearchOptions method', async(inject([], () => {
    fixture.whenStable().then(() => {
      // spyOn(fixture.componentInstance, 'setStructureSearchOptions');
      fixture.autoDetectChanges();
      fixture.componentInstance.queryModel = StructQueryTestData.QueryModelTestData1;
      fixture.componentInstance.setStructureSearchOptions();
      expect(fixture.componentInstance.structureCriteriaOptions._hitAnyChargeHetero).toEqual(StructQueryTestData.structCriteriaOptionsYes);
      expect(fixture.componentInstance.structureCriteriaOptions._reactionCenter).toEqual(StructQueryTestData.structCriteriaOptionsYes);
      expect(fixture.componentInstance.structureCriteriaOptions._hitAnyChargeCarbon).toEqual(StructQueryTestData.structCriteriaOptionsYes);
      expect(fixture.componentInstance.structureCriteriaOptions._permitExtraneousFragments).toEqual(StructQueryTestData.structCriteriaOptionsNo);
      expect(fixture.componentInstance.structureCriteriaOptions._permitExtraneousFragmentsIfRXN).toEqual(StructQueryTestData.structCriteriaOptionsNo);
      expect(fixture.componentInstance.structureCriteriaOptions._fragmentsOverlap).toEqual(StructQueryTestData.structCriteriaOptionsNo);
      expect(fixture.componentInstance.structureCriteriaOptions._tautometer).toEqual(StructQueryTestData.structCriteriaOptionsNo);
      expect(typeof fixture.componentInstance.structureCriteriaOptions._simThreshold).toEqual('string');
      expect(fixture.componentInstance.structureCriteriaOptions._simThreshold).toEqual(StructQueryTestData.QueryModelTestData1.simThreshold.toString());
      expect(fixture.componentInstance.structureCriteriaOptions._fullSearch).toEqual(StructQueryTestData.structCriteriaOptionsNo);
      expect(fixture.componentInstance.structureCriteriaOptions._identity).toEqual(StructQueryTestData.structCriteriaOptionsNo);
      expect(fixture.componentInstance.structureCriteriaOptions._similar).toEqual(StructQueryTestData.structCriteriaOptionsNo);
      expect(fixture.componentInstance.structureCriteriaOptions._tetrahedralStereo).toEqual(StructQueryTestData.QueryModelTestData1.tetrahedralStereo);
      expect(fixture.componentInstance.structureCriteriaOptions._relativeTetStereo).toEqual(StructQueryTestData.structCriteriaOptionsNo);
      expect(fixture.componentInstance.structureCriteriaOptions._doubleBondStereo).toEqual(StructQueryTestData.structCriteriaOptionsYes);
    });
  })));

});
