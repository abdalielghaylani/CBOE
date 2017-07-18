import {
  Component,
  Input, Output, OnChanges,
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
export class ChemDrawWeb implements OnInit, OnDestroy, OnChanges {
  @Input() protected editMode: boolean = true;
  protected drawingTool: any;
  protected creatingCDD: boolean = false;
  protected value: string;
  constructor(protected elementRef: ElementRef) {
  }

  ngOnInit() {
  }

  ngOnDestroy() {
  }

  ngOnChanges() {
    this.update();
  }

  protected update() {
    if (this.drawingTool) {
      this.drawingTool.setViewOnly(!this.editMode);
    }
  }

  protected createDrawingTool() {
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
      viewonly: !this.editMode,
      callback: function (drawingTool) {
        self.drawingTool = drawingTool;
        self.creatingCDD = false;
        drawingTool.fitToContainer();
        if (self.value) {
          drawingTool.loadCDXML(self.value);
          self.value = null;
        }
      },
      licenseUrl: 'https://chemdrawdirect.perkinelmer.cloud/js/license.xml',
      config: { features: { disabled: ['ExtendedCopyPaste'] } }
    };

    (<any>window).perkinelmer.ChemdrawWebManager.attach(params);
  };

  public setValue(value: string) {
    if (this.drawingTool && !this.creatingCDD) {
      this.drawingTool.clear();
      if (value) {
        this.drawingTool.loadCDXML(value);
      }
    } else {
      this.value = value;
    }
  }

  public getValue() {
    return this.drawingTool ? this.drawingTool.getCDXML() : this.value;
  }

  public activate() {
    if (!this.drawingTool) {
      this.createDrawingTool();
    }
  }
};
