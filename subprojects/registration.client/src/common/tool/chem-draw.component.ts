import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
  ElementRef,
  OnInit, OnDestroy
} from '@angular/core';
import { basePath } from '../../configuration';
import * as registryUtils from '../../components/registry/registry.utils';

@Component({
  selector: 'chem-draw-tool',
  styles: [`.cdContainer {
            border:5px solid #f0f0f0;
            height: 300px;
            width: 100%;
          }`],
  template: `
            <div class='cdContainer'>
              <div class='chem-draw'>
                <div class='toastMessagePresenter'></div>
              </div>
            </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ChemDrawingTool implements OnInit, OnDestroy {
  private drawingTool;
  private recordDoc: Document;
  private creatingCDD: boolean = false;
  private cdxml: string;
  constructor(
    private elementRef: ElementRef) {

  }

  ngOnInit() {
  }

  ngOnDestroy() {
    this.removePreviousDrawingTool();    
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
    jQuery(this.elementRef.nativeElement).find('.chem-draw').height(cdHeight);
    let params = {
      element: attachmentElement,
      viewonly: false,
      callback: function (drawingTool) {
        self.drawingTool = drawingTool;
        jQuery(self.elementRef.nativeElement).find('.chem-draw').addClass('hidden');
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

  getValue() {
    return this.drawingTool.getCDXML();
  }

  activate() {
    if (!this.drawingTool) {
      this.createDrawingTool();
    }
  }

};
