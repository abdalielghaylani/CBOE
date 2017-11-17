import { TestBed, async, inject } from '@angular/core/testing';
import { DevExtremeModule } from 'devextreme-angular';
import { TestModule } from '../../../test/test.module';
import { RegCheckBoxFormItem } from './check-box-form-item.component';

describe('Component : Text Form Item', () => {

  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [ TestModule, DevExtremeModule ],
        declarations : [ RegCheckBoxFormItem ],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegCheckBoxFormItem);
      fixture.detectChanges();
      done();
    });
  });

  it('should create Text From Item Component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));
  
});
