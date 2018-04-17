import { DevExtremeModule } from 'devextreme-angular';
import { TestBed, async, inject } from '@angular/core/testing';
import { TestModule } from '../../../../test/test.module';
import { StructQueryTestData } from './structure-query-form-item.component.data.test';
import { RegistryModule, RegStructureQueryOptions, RegStructureQueryFormItem } from '../..';

describe('Component : Structure Query Form Item', () => {

  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [TestModule, DevExtremeModule, RegistryModule]
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
      let testEvent = { 'data': 'Test Data' };
      fixture.componentInstance.viewOptions(testEvent);
      expect(fixture.componentInstance.index).toEqual(testEvent);
    });
  })));

  it('should check values on onOptionUpdated method call', async(inject([], () => {
    fixture.whenStable().then(() => {
      spyOn(fixture.componentInstance, 'serializeValue');
      fixture.autoDetectChanges();
      let testEvent = { 'data': 'Test Data' };
      fixture.componentInstance.viewModel = { component: { option: (name, value) => { } } };
      fixture.componentInstance.onOptionUpdated(testEvent);
      expect(fixture.componentInstance.serializeValue).toHaveBeenCalled();
      fixture.componentInstance.valueUpdated.subscribe(e => expect(e).toEqual(fixture.componentInstance));
    });
  })));
});
