import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
  OnInit,
  ElementRef,
} from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { select, NgRedux } from 'ng2-redux';
import * as x2js from 'x2js';
import { RecordDetailActions } from '../../actions';
import { IAppState } from '../../store';
import * as registryUtils from './registry-utils';

declare var jQuery: any;

@Component({
  selector: 'reg-record-detail',
  styles: [require('./records.css')],
  template: require('./record-detail.component.html'),
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegRecordDetail implements OnInit {
  @Input() id: number = -1;
  @Input() temporary: boolean = false;
  @Input() data: string;
  @select(s => s.registry.structureData) structureData$: Observable<string>;
  private drawingTool;
  private creatingCDD: boolean = false;
  private projects: any[];
  private recordString: string;
  private recordJson: any;
  private recordDoc: Document;
  private rootJson: { CompoundList: any[], ProjectList: any[], PropertyList: any[], RegNumber: any, StructureAggregation: string };

  constructor(private elementRef: ElementRef, private ngRedux: NgRedux<IAppState>, private actions: RecordDetailActions) {
  }

  ngOnInit() {
    this.createDrawingTool();
    let output = registryUtils.getDocument(this.data);
    this.recordString = output.documentElement.firstChild.textContent;
    this.recordDoc = registryUtils.getDocument(this.recordString);
    registryUtils.fixStructureData(this.recordDoc);
    let x2jsTool = new x2js.default({
        arrayAccessFormPaths : [
           'MultiCompoundRegistryRecord.ComponentList.Component',
           'MultiCompoundRegistryRecord.ComponentList.Component.Compound.PropertyList.Property',
           'MultiCompoundRegistryRecord.BatchList.Batch',
           'MultiCompoundRegistryRecord.ProjectList.Project',
           'MultiCompoundRegistryRecord.PropertyList.Proprty',
        ]
    });
    this.recordJson = x2jsTool.dom2js(this.recordDoc);
    this.rootJson = this.recordJson.MultiCompoundRegistryRecord;
    this.actions.loadStructure(registryUtils.getElementValue(this.recordDoc.documentElement,
      'ComponentList/Component/Compound/BaseFragment/Structure/Structure'));
    this.structureData$.subscribe((value: string) => this.loadCdxml(value));
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
