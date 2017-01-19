import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
  OnInit, OnDestroy, AfterViewInit,
  ElementRef,
  ViewChildren, QueryList
} from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { select, NgRedux } from 'ng2-redux';
import * as x2js from 'x2js';
import { RecordDetailActions } from '../../actions';
import { IAppState, IRecordDetail } from '../../store';
import * as registryUtils from './registry-utils';
import * as regTypes from './registry.types';
import { DxFormComponent } from 'devextreme-angular';

declare var jQuery: any;

@Component({
  selector: 'reg-record-detail',
  styles: [require('./records.css')],
  template: require('./record-detail.component.html'),
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegRecordDetail implements OnInit, OnDestroy {
  @ViewChildren(DxFormComponent) forms: QueryList<DxFormComponent>;
  @select(s => s.registry.currentRecord) recordDetail$: Observable<IRecordDetail>;
  @select(s => s.registry.structureData) structureData$: Observable<string>;
  private title: string;
  private drawingTool;
  private creatingCDD: boolean = false;
  private projects: any[];
  private recordString: string;
  private recordJson: any;
  private recordDoc: Document;
  private rootJson: {
    ComponentList: {
      Component: any[]
    },
    ProjectList: any[],
    PropertyList: any[],
    RegNumber: any,
    StructureAggregation: string,
    BatchList: {
      Batch: any[]
    }
  };
  private id: number;
  private temporary: boolean;
  private componentItems: any;
  private compoundItems: any;
  private fragmentItems: any;
  private batchItems: any;
  private dataSubscription: Subscription;
  private loadSubscription: Subscription;
  private componentData: any;
  private compoundData: any;
  private fragmentData: any;
  private batchData: any;

  constructor(private elementRef: ElementRef, private ngRedux: NgRedux<IAppState>, private actions: RecordDetailActions) {
  }

  ngOnInit() {
    this.createDrawingTool();
    this.dataSubscription = this.recordDetail$.subscribe((value: IRecordDetail) => this.loadData(value));
  }

  ngOnDestroy() {
    if (this.dataSubscription) {
      this.dataSubscription.unsubscribe();
    }
    if (this.loadSubscription) {
      this.loadSubscription.unsubscribe();
    }
  }

  loadData(data: IRecordDetail) {
    this.temporary = data.temporary;
    this.id = data.id;
    let output = registryUtils.getDocument(data.data);
    this.recordString = output.documentElement.firstChild.textContent;
    this.recordDoc = registryUtils.getDocument(this.recordString);
    registryUtils.fixStructureData(this.recordDoc);
    let x2jsTool = new x2js.default({
      arrayAccessFormPaths: [
        'MultiCompoundRegistryRecord.ComponentList.Component',
        'MultiCompoundRegistryRecord.ComponentList.Component.Compound.PropertyList.Property',
        'MultiCompoundRegistryRecord.BatchList.Batch',
        'MultiCompoundRegistryRecord.ProjectList.Project',
        'MultiCompoundRegistryRecord.PropertyList.Proprty',
      ]
    });
    this.recordJson = x2jsTool.dom2js(this.recordDoc);
    this.rootJson = this.recordJson.MultiCompoundRegistryRecord;
    if (!this.rootJson.ComponentList.Component[0].Compound.FragmentList) {
      this.rootJson.ComponentList.Component[0].Compound.FragmentList = { Fragment: [new regTypes.FragmentData()] };
    }
    this.actions.loadStructure(registryUtils.getElementValue(this.recordDoc.documentElement,
      'ComponentList/Component/Compound/BaseFragment/Structure/Structure'));
    this.loadSubscription = this.structureData$.subscribe((value: string) => this.loadCdxml(value));
    this.title = data.id < 0 ?
      'Register a New Compound' :
      data.temporary ?
        'Edit a Temporary Record: ' + this.getElementValue(this.recordDoc.documentElement, 'ID') :
        'Edit a Registry Record: ' + this.getElementValue(this.recordDoc.documentElement, 'RegNumber/RegNumber');
    this.componentItems = regTypes.COMPONENT_DESC_LIST;
    this.compoundItems = regTypes.COMPOUND_DESC_LIST;
    this.fragmentItems = regTypes.FRAGMENT_DESC_LIST;
    this.batchItems = regTypes.BATCH_DESC_LIST;
    this.componentData = this.rootJson.ComponentList.Component[0];
    this.compoundData = this.rootJson.ComponentList.Component[0].Compound;
    this.fragmentData = this.rootJson.ComponentList.Component[0].Compound.FragmentList.Fragment[0];
    this.batchData = this.rootJson.BatchList.Batch[0];
  }

  loadCdxml(cdxml: string) {
    if (this.drawingTool && !this.creatingCDD) {
      this.drawingTool.clear();
      if (cdxml) {
        this.drawingTool.loadCDXML(cdxml);
      }
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
    let ccData = '';
    if (this.drawingTool !== undefined) {
      ccData = this.drawingTool.getCDXML();
    }
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
        if (ccData !== '') {
          drawingTool.loadCDXML(ccData);
        }
        jQuery(this.parent.elementRef.nativeElement).find('.click_catch').addClass('hidden');
        if (drawingTool) {
          drawingTool.setViewOnly(false);
        }
        this.parent.creatingCDD = false;
        drawingTool.fitToContainer();
      }
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
    this.actions.save(this.recordDoc);
  }

  update() {
    this.actions.update(this.recordDoc);
  }

  register() {
    this.actions.register(this.recordDoc);
  }
};
