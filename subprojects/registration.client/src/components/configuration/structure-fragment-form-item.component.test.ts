import { RegStructureFragmentFormItem } from './structure-fragment-form-item.component';
import { TestBed, async, inject } from '@angular/core/testing';
import { TestModule } from '../../test/test.module';
import { ChemDrawWeb } from '../index';

describe('Component : Structure Fragment Form Item', () => {

  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [ TestModule ],
        declarations : [ RegStructureFragmentFormItem ],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegStructureFragmentFormItem);
      fixture.detectChanges();
      done();
    });
  });

  it('should instantiate Structure Fragment Form Item component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

  it('should update values on update method call', async(inject([], () => {
    fixture.whenStable().then(() => {
      spyOn(fixture.componentInstance, 'setValue');
      fixture.autoDetectChanges();
      fixture.componentInstance.update();
      expect(fixture.componentInstance.setValue).toHaveBeenCalled();
    });
  })));

  // To-do unit test for 
  // onContentChanged();

});
