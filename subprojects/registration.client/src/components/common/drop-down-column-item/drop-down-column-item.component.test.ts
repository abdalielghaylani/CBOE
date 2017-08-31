import { RegDropDownColumnItem } from './drop-down-column-item.component';
import { TestBed, async, inject } from '@angular/core/testing';
import { TestModule } from '../../../test/test.module';
import { DevExtremeModule } from 'devextreme-angular';
import { RegStructureColumnItem } from '../common-component.module';
import { DDColumnItemViewConfig } from './test-data';

describe('Component : Drop Down Column Item', () => {

  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [ TestModule, DevExtremeModule ],
        declarations : [ RegDropDownColumnItem, RegStructureColumnItem ],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegDropDownColumnItem);
      fixture.detectChanges();
      done();
    });
  });

  it('should create Drop Down Column Item Component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

  it('should deserialize value useNumeric false', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      fixture.componentInstance.useNumericValue = false;
      let testValue = '57';
      let expectedValue = '57';
      expect(fixture.componentInstance.deserializeValue(testValue))
        .toEqual(expectedValue);
    });
  })));

  it('should deserialize value useNumeric true', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      fixture.componentInstance.useNumericValue = true;
      let testValue = '57'; let expectedValue = 57;
      expect(fixture.componentInstance.deserializeValue(testValue)).toEqual(expectedValue);
      let testValueNum = 57; expectedValue = 57;
      expect(fixture.componentInstance.deserializeValue(testValueNum)).toEqual(expectedValue);
    });
  })));

  it('should serialize value useNumeric false', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      fixture.componentInstance.useNumericValue = false;
      let testValue = '87'; let expectedValue = '87';
      expect(fixture.componentInstance.serializeValue(testValue)).toBe(expectedValue);
    });
  })));

  it('should serialize value useNumeric true', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      fixture.componentInstance.useNumericValue = true;
      let testValue = 54; let expectedValue = '54';
      expect(fixture.componentInstance.serializeValue(testValue)).toBe(expectedValue);
    });
  })));

  it('should update values to component attribute values', async(inject([], () => {
    fixture.whenStable().then(() => {
      spyOn(fixture.componentInstance, 'setDropDownWidth');
      fixture.autoDetectChanges();
      fixture.componentInstance.viewModel = {'value' : '60'};
      fixture.componentInstance.viewConfig = DDColumnItemViewConfig;
      fixture.componentInstance.update();
      expect(fixture.componentInstance.setDropDownWidth).toHaveBeenCalled();
      expect(fixture.componentInstance.dataSource).toBe(DDColumnItemViewConfig.dataSource);
      expect(fixture.componentInstance.valueExpr).toBe(DDColumnItemViewConfig.valueExpr);
      expect(fixture.componentInstance.displayExpr).toBe(DDColumnItemViewConfig.displayExpr);
      expect(fixture.componentInstance.columns).toBe(DDColumnItemViewConfig.columns);
      expect(fixture.componentInstance.showClearButton).toBeFalsy();
      expect(fixture.componentInstance.value).toBe(fixture.componentInstance.viewModel.value);
    });
  })));

  it('should check values on changes method call', async(inject([], () => {
    fixture.whenStable().then(() => {
      spyOn(fixture.componentInstance, 'update');
      fixture.autoDetectChanges();
      fixture.componentInstance.viewModel = { column : { editorOptions : { displayExpr : 'CODE', dropDownWidth : 600, valueExpr : 'CODE'} } };
      fixture.componentInstance.ngOnChanges();
      expect(fixture.componentInstance.viewConfig).toBe(fixture.componentInstance.viewModel.column.editorOptions);
      expect(fixture.componentInstance.update).toHaveBeenCalled();
    });
  })));

  it('should check values after view initialize method call', async(inject([], () => {
    fixture.whenStable().then(() => {
      spyOn(fixture.componentInstance, 'setDropDownWidth');
      fixture.autoDetectChanges();
      fixture.componentInstance.ngAfterViewInit();
      expect(fixture.componentInstance.setDropDownWidth).toHaveBeenCalled();
    });
  })));

  
  // To do Unit Test preparation for :
  // setDropDownWidth()
  // onGridRowClick()

});
