import { RegTagBoxFormItem } from './tag-box-form-item.component';
import { TestBed, async, inject } from '@angular/core/testing';
import { TestModule } from '../../../test/test.module';
import { DevExtremeModule } from 'devextreme-angular';

describe('Component : Tag Box Form Item', () => {

  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [ TestModule, DevExtremeModule ],
        declarations : [ RegTagBoxFormItem ],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegTagBoxFormItem);
      fixture.detectChanges();
      done();
    });
  });

  it('should create Tag Box Form Item Component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

  it('should serialize value', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance.serializeValue('Test Data')).toEqual('Test Data');
    });
  })));

  it('should deserialize value', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance.deserializeValue('Test Data')).toEqual('Test Data');
    });
  })));

  
  // To do Unit Test preparation for :
  // update

});
