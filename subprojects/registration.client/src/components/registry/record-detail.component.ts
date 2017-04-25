import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
  OnInit, OnDestroy, AfterViewInit,
  ElementRef, ChangeDetectorRef,
  ViewChildren, QueryList
} from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { select, NgRedux } from '@angular-redux/store';
import * as x2js from 'x2js';
import { RecordDetailActions, ConfigurationActions } from '../../actions';
import { IAppState, IRecordDetail } from '../../store';
import * as registryUtils from './registry.utils';
import { CFormGroup, prepareFormGroupData, notify } from '../../common';
import { CRegistryRecord, CRegistryRecordVM, FragmentData } from './registry.types';
import { DxFormComponent } from 'devextreme-angular';
import { basePath } from '../../configuration';
import { FormGroupType, IFormContainer, getFormGroupData } from '../../common';

declare var jQuery: any;

@Component({
  selector: 'reg-record-detail',
  styles: [require('./records.css')],
  template: require('./record-detail.component.html'),
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegRecordDetail implements  IFormContainer, OnInit, OnDestroy {
  @ViewChildren(DxFormComponent) forms: QueryList<DxFormComponent>;
  @Input() temporary: boolean;
  @Input() id: number;
  @select(s => s.registry.currentRecord) recordDetail$: Observable<IRecordDetail>;
  @select(s => s.registry.structureData) structureData$: Observable<string>;
  public formGroup: CFormGroup;
  public editMode: boolean = false;
  private title: string;
  private drawingTool;
  private creatingCDD: boolean = false;
  private cdxml: string;
  private recordString: string;
  private recordDoc: Document;
  private regRecord: CRegistryRecord = new CRegistryRecord();
  private regRecordVM: CRegistryRecordVM = new CRegistryRecordVM(this.regRecord, this);
  private dataSubscription: Subscription;
  private loadSubscription: Subscription;

  constructor(
    public ngRedux: NgRedux<IAppState>,
    private elementRef: ElementRef,
    private router: Router,
    private actions: RecordDetailActions,
    private changeDetector: ChangeDetectorRef) {
  }

  ngOnInit() {
    let state = this.ngRedux.getState();
    if (this.id >= 0 && (state.registry.records.data.rows.length === 0 && state.registry.tempRecords.data.rows.length === 0)) {
      this.router.navigate(['/']);
      return;
    }
    this.createDrawingTool();
    this.actions.retrieveRecord(this.temporary, this.id);
    this.dataSubscription = this.recordDetail$.subscribe((value: IRecordDetail) => this.loadData(value));
  }

  ngOnDestroy() {
    if (this.dataSubscription) {
      this.dataSubscription.unsubscribe();
    }
    if (this.loadSubscription) {
      this.loadSubscription.unsubscribe();
    }
    this.actions.clearRecord();
  }

  loadData(data: IRecordDetail) {
    if (this.temporary !== data.temporary || this.id !== data.id) {
      return;
    }
    let output = registryUtils.getDocument(data.data);
    this.recordString = output.documentElement.firstChild.textContent;
    this.recordDoc = registryUtils.getDocument(this.recordString);
    this.title = data.id < 0 ?
      'Register a New Compound' :
      data.temporary ?
        'Edit a Temporary Record: ' + this.getElementValue(this.recordDoc.documentElement, 'ID') :
        'Edit a Registry Record: ' + this.getElementValue(this.recordDoc.documentElement, 'RegNumber/RegNumber');
    registryUtils.fixStructureData(this.recordDoc);
    let x2jsTool = new x2js({
      arrayAccessFormPaths: [
        'MultiCompoundRegistryRecord.ComponentList.Component',
        'MultiCompoundRegistryRecord.ComponentList.Component.Compound.PropertyList.Property',
        'MultiCompoundRegistryRecord.ComponentList.Component.Compound.FragmentList.Fragment',
        'MultiCompoundRegistryRecord.ComponentList.Component.Compound.IdentifierList.Identifier',
        'MultiCompoundRegistryRecord.BatchList.Batch',
        'MultiCompoundRegistryRecord.BatchList.Batch.BatchComponentList.BatchComponent',
        'MultiCompoundRegistryRecord.BatchList.Batch.BatchComponentList.BatchComponent.BatchComponentFragmentList.BatchComponentFragment',
        'MultiCompoundRegistryRecord.ProjectList.Project',
        'MultiCompoundRegistryRecord.PropertyList.Property',
      ]
    });
    let recordJson: any = x2jsTool.dom2js(this.recordDoc);
    this.regRecord = recordJson.MultiCompoundRegistryRecord;
    let formGroupType = FormGroupType.SubmitMixture;
    if (data.id >= 0 && !data.temporary) {
      // TODO: For mixture, this should be ReviewRegistryMixture
      formGroupType = FormGroupType.ViewMixture;
    }
    prepareFormGroupData(formGroupType, this.ngRedux);
    let state = this.ngRedux.getState();
    this.formGroup = state.configuration.formGroups[FormGroupType[formGroupType]] as CFormGroup;
    this.editMode = this.formGroup.detailsForms.detailsForm[0].coeForms._defaultDisplayMode === 'Edit';
    this.regRecordVM = new CRegistryRecordVM(this.regRecord, this);
    if (!this.regRecord.ComponentList.Component[0].Compound.FragmentList) {
      this.regRecord.ComponentList.Component[0].Compound.FragmentList = { Fragment: [new FragmentData()] };
    }
    this.actions.loadStructure(registryUtils.getElementValue(this.recordDoc.documentElement,
      'ComponentList/Component/Compound/BaseFragment/Structure/Structure'));
    this.loadSubscription = this.structureData$.subscribe((value: string) => this.loadCdxml(value));
    this.changeDetector.markForCheck();
  }

  loadCdxml(cdxml: string) {
    if (this.drawingTool && !this.creatingCDD) {
      this.drawingTool.clear();
      if (cdxml) {
        this.drawingTool.loadCDXML(cdxml);
      }
    } else {
      this.cdxml = cdxml;
    }
  }

  getElementValue(e: Element, path: string) {
    return registryUtils.getElementValue(e, path);
  }

  createDrawingTool() {
    if (this.drawingTool || this.creatingCDD) {
      return;
    }
    this.creatingCDD = true;
    this.removePreviousDrawingTool();

    let cddContainer = jQuery(this.elementRef.nativeElement).find('.cdContainer');
    let cdWidth = cddContainer.innerWidth() - 4;
    let attachmentElement = cddContainer[0];
    let cdHeight = attachmentElement.offsetHeight;
    jQuery(this.elementRef.nativeElement).find('.click_catch').height(cdHeight);
    let params = {
      element: attachmentElement,
      height: (cdHeight - 2),
      width: cdWidth,
      viewonly: false,
      parent: this,
      callback: function (drawingTool) {
        this.parent.drawingTool = drawingTool;
        jQuery(this.parent.elementRef.nativeElement).find('.click_catch').addClass('hidden');
        if (drawingTool) {
          drawingTool.setViewOnly(false);
        }
        this.parent.creatingCDD = false;
        drawingTool.fitToContainer();
        if (this.parent.cdxml) {
          drawingTool.loadCDXML(this.parent.cdxml);
          this.parent.cdxml = null;
        }
      },
      licenseUrl: 'https://chemdrawdirect.perkinelmer.cloud/js/license.xml',
      config: { features: { disabled: ['ExtendedCopyPaste'] } }
    };

    (<any>window).perkinelmer.ChemdrawWebManager.attach(params);
  };

  removePreviousDrawingTool = function () {
    if (this.drawingTool) {
      let container = jQuery(this.elementRef.nativeElement).find('.cdContainer');
      container.find('div')[2].remove();
      this.drawingTool = undefined;
    }
  };

  save() {
    if (this.id < 0) {
      // Retrieve CDXML from CDD
      registryUtils.setElementValue(this.recordDoc.documentElement,
        'ComponentList/Component/Compound/BaseFragment/Structure/Structure', this.drawingTool.getCDXML());
      this.actions.saveRecord(this.recordDoc);
    } else {
      // notify('Saving is not supported yet!', 'warning');
      // this.actions.updateRecord(this.recordDoc);
      this.setEditMode(false);
    }
  }

  cancel() {
    this.setEditMode(false);
  }

  edit() {
    // notify('Editing is experimental', 'warning');
    this.setEditMode(true);
  }

  register() {
    this.actions.registerRecord(this.recordDoc);
  }

  private setEditMode(editMode: boolean) {
    this.editMode = editMode;
    this.changeDetector.markForCheck();
    this.forms.forEach(f => {
      f.items.forEach(i => {
        if (i.template) {
          i.disabled = !editMode;
        }
      });
      f.readOnly = !editMode;
      f.instance.repaint();
    });
  }
};
