import { TestModule } from '../../../test/test.module';
import { TestBed, async, inject } from '@angular/core/testing';
import { RegBaseColumnItem } from './base-column-item.component';

describe('Component : Base Column Item', () => {

  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [ TestModule ],
        declarations : [ RegBaseColumnItem ],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegBaseColumnItem);
      fixture.detectChanges();
      done();
    });
  });

  it('should create Base Column Item Component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

  it('should deserialise values', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let testVal = { 'Data' : 'TestData' };
      let expectedVal = { 'Data' : 'TestData' };
      expect(fixture.componentInstance.deserializeValue(testVal)).toEqual(expectedVal);
    });
  })));

  it('should check values in On Changes method call', async(inject([], () => {
    fixture.whenStable().then(() => {
      spyOn(fixture.componentInstance, 'update');
      fixture.autoDetectChanges();
      fixture.componentInstance.viewModel = { column : { editorOptions : { 'smallImage' : true } } };
      fixture.componentInstance.ngOnChanges();
      expect(fixture.componentInstance.viewConfig).toBe(fixture.componentInstance.viewModel.column.editorOptions);
      expect(fixture.componentInstance.update).toHaveBeenCalled();
    });
  })));

  it('should check values in Update method call', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let testVal = { value : 'Test Value', column : { editorOptions : { 'smallImage' : true } } };
      fixture.componentInstance.viewModel = testVal;
      fixture.componentInstance.viewConfig = testVal;
      fixture.componentInstance.ngOnChanges();
      expect(fixture.componentInstance.value).toBe(testVal.value);
    });
  })));

  // Complete test cover
});
