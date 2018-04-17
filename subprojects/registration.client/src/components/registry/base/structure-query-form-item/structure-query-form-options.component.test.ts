import { TestBed, async, inject } from '@angular/core/testing';
import { TestModule } from '../../../../test/test.module';
import { DevExtremeModule } from 'devextreme-angular';
import { RegStructureQueryOptions } from './structure-query-form-options.component';

describe('Component : Structure Query Form Otions', () => {

  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [TestModule, DevExtremeModule],
        declarations: [RegStructureQueryOptions],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegStructureQueryOptions);
      fixture.detectChanges();
      done();
    });
  });

  it('should create Structure Query Form Otions', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

  it('should emit event on feild data changed', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let testEvent = { data: 'Test Data' };
      fixture.componentInstance.onFieldDataChanged(testEvent);
      fixture.componentInstance.optionUpdated.subscribe(e => expect(e).toEqual(testEvent));
    });
  })));

});
