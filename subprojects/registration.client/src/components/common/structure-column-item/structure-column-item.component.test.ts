import { TestBed, async, inject } from '@angular/core/testing';
import { TestModule } from '../../../test/test.module';
import { RegStructureColumnItem } from './structure-column-item.component';

describe('Component : Structure Column Item', () => {

  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [ TestModule ],
        declarations : [ RegStructureColumnItem ],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegStructureColumnItem);
      fixture.detectChanges();
      done();
    });
  });

  it('should create Structure Column Item Component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

  it('should update values in component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      fixture.componentInstance.smallImage = false;
      fixture.componentInstance.viewConfig = { 'smallImage' : true };
      fixture.componentInstance.viewModel = { 'value' : 'fragment/48?20090630020232' };
      fixture.componentInstance.value = '';
      let expectedVal = '/api/v1/StructureImage/fragment/48/30/60?20090630020232';
      fixture.componentInstance.update();
      expect(fixture.componentInstance.value).toEqual(expectedVal);
      expect(fixture.componentInstance.smallImage).toBeTruthy();
    });
  })));

  it('should deserialize values', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      fixture.componentInstance.smallImage = true;
      let testVal = { value : 'fragment/48?20090630020232' };
      let expectedVal = '/api/v1/StructureImage/fragment/48/30/60?20090630020232';
      expect(fixture.componentInstance.deserializeValue(testVal.value)).toEqual(expectedVal);
      fixture.componentInstance.smallImage = false;
      let expectedVal2 = '/api/v1/StructureImage/fragment/48/50/100?20090630020232';
      expect(fixture.componentInstance.deserializeValue(testVal.value)).toEqual(expectedVal2);
    });
  })));

// Completed Unit Test cover

});
