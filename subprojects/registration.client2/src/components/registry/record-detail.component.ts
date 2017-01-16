import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
  OnInit,
  ElementRef,
} from '@angular/core';
import { RecordDetailActions } from '../../actions';
import { ICounter } from '../../store';
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
  private document: Document;
  private drawingTool;
  private creatingCDD: boolean = false;

  constructor(private elementRef: ElementRef, private actions: RecordDetailActions) {
  }

  ngOnInit() {
    let output = registryUtils.getDocument(this.data);
    this.document = registryUtils.getDocument(output.documentElement.firstChild.textContent);
    this.createDrawingTool();
  }

  getElementValue(e: Element, path: string) {
    return registryUtils.getElementValue(e, path);
  }

  createDrawingTool = function () {
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

  removePreviousDrawingTool = function() {
    if (this.drawingTool) {
      let container = jQuery(this.elementRef.nativeElement).find('.cdContainer');
      container.find('div')[2].remove();
      this.drawingTool = undefined;
    }
  };

  save() {
    this.actions.save(this.document);
  }

  update() {
    this.actions.update(this.document);
  }

  register() {
    this.actions.register(this.document);
  }
};
