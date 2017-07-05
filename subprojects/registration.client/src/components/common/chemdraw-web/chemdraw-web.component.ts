import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
  ElementRef,
  OnInit, OnDestroy
} from '@angular/core';

@Component({
  selector: 'chemdraw-web',
  styles: [require('./chemdraw-web.css')],
  template: require('./chemdraw-web.component.html'),
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ChemDrawWeb implements OnInit, OnDestroy {
  private drawingTool;
  private creatingCDD: boolean = false;
  private cdxml: string;
  constructor(private elementRef: ElementRef) {
  }

  ngOnInit() {
  }

  ngOnDestroy() {
  }

  createDrawingTool() {
    if (this.drawingTool || this.creatingCDD) {
      return;
    }
    this.creatingCDD = true;

    let cddContainer = jQuery(this.elementRef.nativeElement).find('.cdContainer');
    let cdWidth = cddContainer.innerWidth() - 4;
    let attachmentElement = cddContainer[0];
    let cdHeight = attachmentElement.offsetHeight;
    const self = this;
    let params = {
      element: attachmentElement,
      viewonly: false,
      callback: function (drawingTool) {
        self.drawingTool = drawingTool;
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
