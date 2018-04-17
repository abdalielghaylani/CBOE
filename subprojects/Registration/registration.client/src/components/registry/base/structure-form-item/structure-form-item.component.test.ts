import { TestBed, async, inject } from '@angular/core/testing';
import { TestModule } from '../../../../test';
import { RegBaseComponentModule } from '../..';
import { DevExtremeModule } from 'devextreme-angular';
import { RegStructureFormItem } from './structure-form-item.component';

describe('Component : Structure Form Item', () => {

  let fixture;
  let structureData;
  let testSerializeInputData;
  let testStructureText;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [TestModule, DevExtremeModule, RegBaseComponentModule]
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegStructureFormItem);
      fixture.detectChanges();
      structureData = {
        'Structure': { '__text': 'Test CDXML Data here' },
        'NormalizedStructure': 'Test CDXML Data here', 'DrawingType': '0'
      };
      testSerializeInputData = { 'mode': 0, 'structureData': {} };
      testStructureText = 'Test CDXML Data here';
      done();
    });
  });

  it('should create', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

  it('should check extract mode', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      let testNode = structureData.DrawingType;
      let expectedVal = parseInt(structureData.DrawingType, 10);
      expect(fixture.componentInstance.extractMode(testNode)).toEqual(expectedVal);
    });
  })));

  it('should deserialize value', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.componentInstance.structureData = { 'DrawingType': '0' };
      spyOn(fixture.componentInstance, 'updateMode');
      spyOn(fixture.componentInstance, 'extractMode');
      fixture.autoDetectChanges();
      let returnDeserializedVal = fixture.componentInstance.deserializeValue(structureData);
      expect(fixture.componentInstance.structureData).toEqual(structureData);
      expect(typeof returnDeserializedVal).toEqual('string');
      expect(returnDeserializedVal).toEqual(testStructureText);
      expect(fixture.componentInstance.updateMode).toHaveBeenCalled();
      expect(fixture.componentInstance.extractMode).toHaveBeenCalled();
    });
  })));

  it('should serialize value', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.componentInstance.structureData = structureData;
      fixture.autoDetectChanges();
      fixture.componentInstance.serializeValue(testSerializeInputData);
      expect(fixture.componentInstance.structureData.Structure.__text).toEqual(testSerializeInputData);
      expect(fixture.componentInstance.structureData.Structure._update).toEqual('yes');
      expect(fixture.componentInstance.structureData.Structure.NormalizedStructure).toBeUndefined();
      expect(fixture.componentInstance.structureData.DrawingType).toEqual('0');
      expect(fixture.componentInstance.serializeValue(testSerializeInputData)).toEqual(fixture.componentInstance.structureData);
    });
  })));

  it('should update the mode value', async(inject([], () => {
    fixture.whenStable().then(() => {
      spyOn(fixture.componentInstance.changeDetector, 'markForCheck');
      fixture.autoDetectChanges();
      let modeTestData = 0; // 0 : for chemical
      let forceRefreshTestData = false;
      fixture.componentInstance.updateMode(modeTestData, forceRefreshTestData);
      expect(fixture.componentInstance.mode).toEqual(modeTestData);
      forceRefreshTestData = true;
      fixture.componentInstance.updateMode(modeTestData, forceRefreshTestData);
      expect(fixture.componentInstance.changeDetector.markForCheck).toHaveBeenCalled();
    });
  })));


});
