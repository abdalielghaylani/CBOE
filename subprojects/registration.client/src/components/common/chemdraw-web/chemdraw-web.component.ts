import {
  Component,
  Input, Output, OnChanges,
  EventEmitter,
  ChangeDetectionStrategy,
  ElementRef, ViewEncapsulation,
  OnInit, OnDestroy, AfterViewInit
} from '@angular/core';

@Component({
  selector: 'chemdraw-web',
  styles: [require('./chemdraw-web.css')],
  template: require('./chemdraw-web.component.html'),
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ChemDrawWeb implements OnInit, OnDestroy, OnChanges, AfterViewInit {
  @Input() editMode: boolean = false;
  @Input() id: string;
  @Input() activated: boolean = false;
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

  ngAfterViewInit() {
    this.activate();
  }

  protected update() {
    if (this.drawingTool) {
      this.drawingTool.setViewOnly(!this.editMode);
      this.drawingTool.fitToContainer();
    } else {
      this.activate();
    }
  }

  protected createDrawingTool() {
    let cddContainer = jQuery(this.elementRef.nativeElement).find('div');
    let attachmentElement = cddContainer[0];
    if (this.drawingTool || this.creatingCDD || !attachmentElement) {
      return;
    }
    this.creatingCDD = true;
    const self = this;
    let params = {
      element: attachmentElement,
      viewonly: false,
      callback: function (drawingTool) {
        self.drawingTool = drawingTool;
        self.creatingCDD = false;
        drawingTool.fitToContainer();
        drawingTool.setViewOnly(!self.editMode);
        if (self.value) {
          drawingTool.loadCDXML(self.value);
          self.value = null;
        }
      },
      licenseUrl: 'https://chemdrawdirect.perkinelmer.cloud/js/license.xml',
      config: { features: { enable: ['ExtendedCopyPaste'] } }
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
    if (this.activated && !this.drawingTool && this.id) {
      this.createDrawingTool();
    }
  }
};
