import { TestBed, async, inject } from '@angular/core/testing';
import { TestModule } from '../../../../test/test.module';
import { RegFormGroupItemBase } from './form-group-item-base.component';
import { groupItemData } from './form-group-item-base-test-data.test';
import { CViewGroupContainer } from '../registry-base.types';

describe('Component : Form Group Item Base', () => {

  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [ TestModule  ],
        declarations : [ RegFormGroupItemBase ],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegFormGroupItemBase);
      fixture.detectChanges();
      done();
    });
  });

  it('should create Form group item base Component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

  it('should call update method on changes', async(inject([], () => {
    fixture.whenStable().then(() => {
      spyOn(fixture.componentInstance, 'update');
      fixture.autoDetectChanges();
      fixture.componentInstance.ngOnChanges();
      expect(fixture.componentInstance.update).toHaveBeenCalled();
    });
  })));

  it('should call update method on update method call', async(inject([], () => {
    fixture.whenStable().then(() => {
      spyOn(fixture.componentInstance, 'readVM');
      fixture.componentInstance.viewConfig = new CViewGroupContainer('temporary_information', 
        'Temporary Information');
      fixture.componentInstance.displayMode = 'query';
      fixture.autoDetectChanges();
      fixture.componentInstance.update();
      expect(fixture.componentInstance.readVM).toHaveBeenCalled();
    });
  })));

  it('should update values on values updated method call', async(inject([], () => {
    fixture.whenStable().then(() => {
      spyOn(fixture.componentInstance, 'writeVM');
      fixture.autoDetectChanges();
      let testEvent = { 'data' : 'Test Data' };
      fixture.componentInstance.onValueUpdated(testEvent);
      fixture.componentInstance.valueUpdated.subscribe(e => expect(e).toEqual(fixture.componentInstance));
      expect(fixture.componentInstance.writeVM).toHaveBeenCalled();
    });
  })));

  it('should check valid items returned', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.componentInstance.items = groupItemData.NonGroupTestDataInput;
      fixture.autoDetectChanges();
      expect(fixture.componentInstance.getValidItems()).toEqual(groupItemData.NonGroupTestDataOutput);
    });
  })));

  it('should check valid items returned from Groups', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.componentInstance.items = groupItemData.GroupTestDataInput;
      fixture.autoDetectChanges();
      expect(fixture.componentInstance.getValidItems()).toEqual(groupItemData.GroupTestDataOutput);
    });
  })));

  it('should return valid items from items input', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(RegFormGroupItemBase.getValidItems(groupItemData.NonGroupTestDataInput)).toEqual(groupItemData.NonGroupTestDataOutput);
    });
  })));

  it('should check valid items returned from Groups input', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(RegFormGroupItemBase.getValidItems(groupItemData.GroupTestDataInput)).toEqual(groupItemData.GroupTestDataOutput);
    });
  })));

  // To do 
  // update method unit case to test this.items and this.colCount
  // togglePanel
});
