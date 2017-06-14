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
import * as X2JS from 'x2js';
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
  template: require('./record-detail.component.html'),
  styles: [require('./records.css')],
  host: { '(document:click)': 'onDocumentClick($event)' },
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegRecordDetail implements IFormContainer, OnInit, OnDestroy {
  @ViewChildren(DxFormComponent) forms: QueryList<DxFormComponent>;
  @Input() temporary: boolean;
  @Input() id: number;
  @select(s => s.registry.currentRecord) recordDetail$: Observable<IRecordDetail>;
  public formGroup: CFormGroup;
  public editMode: boolean = false;
  private title: string;
  private drawingTool;
  private creatingCDD: boolean = false;
  private cdxml: string;
  private parentHeight: string;
  private recordString: string;
  private recordDoc: Document;
  private regRecord: CRegistryRecord = new CRegistryRecord();
  private regRecordVM: CRegistryRecordVM = new CRegistryRecordVM(this.regRecord, this);
  private dataSubscription: Subscription;
  private loadSubscription: Subscription;
  private currentIndex: number = 0;

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
    this.parentHeight = this.getParentHeight();
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

  private getParentHeight() {
    return ((this.elementRef.nativeElement.parentElement.clientHeight) - 100).toString();
  }

  private onResize(event: any) {
    this.parentHeight = this.getParentHeight();
  }

  private onDocumentClick(event: any) {
    if (event.srcElement.title === 'Full Screen') {
      let fullScreenMode = event.srcElement.className === 'fa fa-compress fa-stack-1x white';
      this.parentHeight = (this.elementRef.nativeElement.parentElement.clientHeight - (fullScreenMode ? 10 : 190)).toString();
    }
  }

  loadData(recordDetail: IRecordDetail) {
    if (this.temporary !== recordDetail.temporary || this.id !== recordDetail.id) {
      return;
    }
    this.recordDoc = registryUtils.getDocument(recordDetail.data);
    this.title = recordDetail.id < 0 ?
      'Register a New Compound' :
      recordDetail.temporary ?
        'Edit a Temporary Record: ' + this.getElementValue(this.recordDoc.documentElement, 'ID') :
        'Edit a Registry Record: ' + this.getElementValue(this.recordDoc.documentElement, 'RegNumber/RegNumber');
    // registryUtils.fixStructureData(this.recordDoc);
    let x2jsTool = new X2JS.default({
      arrayAccessFormPaths: [
        'MultiCompoundRegistryRecord.ComponentList.Component',
        'MultiCompoundRegistryRecord.ComponentList.Component.Compound.PropertyList.Property',
        'MultiCompoundRegistryRecord.ComponentList.Component.Compound.FragmentList.Fragment',
        'MultiCompoundRegistryRecord.ComponentList.Component.Compound.IdentifierList.Identifier',
        'MultiCompoundRegistryRecord.BatchList.Batch',
        'MultiCompoundRegistryRecord.BatchList.Batch.BatchComponentList.BatchComponent',
        'MultiCompoundRegistryRecord.BatchList.Batch.BatchComponentList.BatchComponent.BatchComponentFragmentList.BatchComponentFragment',
        'MultiCompoundRegistryRecord.IdentifierList.Identifier',
        'MultiCompoundRegistryRecord.ProjectList.Project',
        'MultiCompoundRegistryRecord.PropertyList.Property',
      ]
    });
    let recordJson: any = x2jsTool.dom2js(this.recordDoc);
    this.regRecord = recordJson.MultiCompoundRegistryRecord;
    let formGroupType = FormGroupType.SubmitMixture;
    if (recordDetail.id >= 0 && !recordDetail.temporary) {
      // TODO: For mixture, this should be ReviewRegistryMixture
      formGroupType = FormGroupType.ViewMixture;
    }
    prepareFormGroupData(formGroupType, this.ngRedux);
    let state = this.ngRedux.getState();
    this.formGroup = state.configuration.formGroups[FormGroupType[formGroupType]] as CFormGroup;
    this.editMode = this.id < 0 || this.formGroup.detailsForms.detailsForm[0].coeForms._defaultDisplayMode === 'Edit';
    this.regRecordVM = new CRegistryRecordVM(this.regRecord, this);
    if (!this.regRecord.ComponentList.Component[0].Compound.FragmentList) {
      this.regRecord.ComponentList.Component[0].Compound.FragmentList = { Fragment: [new FragmentData()] };
    }
    let structureData = registryUtils.getElementValue(this.recordDoc.documentElement,
      'ComponentList/Component/Compound/BaseFragment/Structure/Structure');
    this.loadCdxml(structureData);
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
    const self = this;
    jQuery(this.elementRef.nativeElement).find('.click_catch').height(cdHeight);
    let params = {
      element: attachmentElement,
      height: (cdHeight - 2),
      width: cdWidth,
      viewonly: false,
      callback: function (drawingTool) {
        self.drawingTool = drawingTool;
        jQuery(self.elementRef.nativeElement).find('.click_catch').addClass('hidden');
        if (drawingTool) {
          drawingTool.setViewOnly(false);
        }
        self.creatingCDD = false;
        drawingTool.fitToContainer();
        if (self.cdxml) {
          drawingTool.loadCDXML(self.cdxml);
          self.cdxml = null;
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

  private updateRecord() {
    registryUtils.setElementValue(this.recordDoc.documentElement,
      'ComponentList/Component/Compound/BaseFragment/Structure/Structure', this.drawingTool.getCDXML());
  }

  save() {
    this.updateRecord();
    if (this.id < 0) {
      this.actions.saveRecord(this.temporary, this.id, this.recordDoc);
    } else {
      this.setEditMode(false);
      this.actions.saveRecord(this.temporary, this.id, this.recordDoc);
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
    this.updateRecord();
    this.actions.saveRecord(this.temporary, this.id, this.recordDoc, true);
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

  private togglePanel(e) {
    if (e.srcElement.children.length > 0) {
      e.srcElement.children[0].click();
    }
  }

  private showTemplates(e) {
    this.currentIndex = 1;
    this.changeDetector.markForCheck();
  }

  private showDetails(e) {
    this.currentIndex = 0;
    this.changeDetector.markForCheck();
  }
};
