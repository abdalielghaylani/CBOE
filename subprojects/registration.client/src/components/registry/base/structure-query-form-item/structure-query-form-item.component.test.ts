import { RegStructureQueryFormItem } from './structure-query-form-item.component';
import { DevExtremeModule } from 'devextreme-angular';
import { CommandDropdown } from '../../../../common/tool/command-dropdown.component';
import { CommandButton } from '../../../../common/tool/command-button.component';
import { RegStructureQueryOptions } from './structure-query-form-options.component';
import { TestBed, async, inject } from '@angular/core/testing';
import { TestModule } from '../../../../test/test.module';

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

});
