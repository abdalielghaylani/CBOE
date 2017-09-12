import { RegStructureQueryFormItem } from './structure-query-form-item.component';
import { TestBed, async, inject } from '@angular/core/testing';
import { TestModule } from '../../../../test/test.module';
import { RegStructureQueryOptions } from './structure-query-form-options.component';
import { DevExtremeModule } from 'devextreme-angular';
import { CommandButton } from '../../../../common/tool/command-button.component';
import { CommandDropdown } from '../../../../common/tool/command-dropdown.component';

describe('Component : Structure Query Form Item', () => {

  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [ TestModule, DevExtremeModule ],
        declarations : [ RegStructureQueryFormItem, RegStructureQueryOptions,
          CommandButton, CommandDropdown ],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegStructureQueryFormItem);
      fixture.detectChanges();
      done();
    });
  });

  it('Structure Query Form Item component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

});
