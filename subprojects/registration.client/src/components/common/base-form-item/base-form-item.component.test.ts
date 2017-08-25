import { Component } from '@angular/core';
import { TestBed, async, inject } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { TestModule } from '../../../test/test.module';
import { RegBaseFormItem } from './base-form-item.component';

describe('Component : Base Form Item Component', () => {

  let fixture;
  let queryByDir;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [ TestModule ],
        declarations : [ RegBaseFormItem  ],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegBaseFormItem);
      fixture.detectChanges();
      done();
    });
  });

  it('should create Base Form Item Component, serialize value', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance).toBeDefined();
      expect(fixture.componentInstance.serializeValue('Test Serialize Text')).toBe('Test Serialize Text');
    });
  })));

  it('should deserialize passed value', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(fixture.componentInstance.deserializeValue('Test Deserialize Text')).toBe('Test Deserialize Text');
    });
  })));

  it('should update values to component\'s value and readonly attributes', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      fixture.componentInstance.viewModel.editorOptions = {'value': 'C6H6', 'readOnly': true,
      'inputAttr': {'id': 'dx_dx-d1b705e9-7d5e-a017-972e-da7ec5a48be7_FormulaLabelEndComponent'},
      'name': 'FormulaLabelEndComponent'};
      fixture.componentInstance.editMode = false;
      fixture.componentInstance.update();
      expect(fixture.componentInstance.value).toBe('C6H6');
      expect(fixture.componentInstance.readOnly).toBeTruthy();
    });
  })));

});
