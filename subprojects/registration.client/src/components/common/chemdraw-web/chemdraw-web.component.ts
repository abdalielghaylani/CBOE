import {
  Component,
  Input, Output, OnChanges,
  EventEmitter,
  ChangeDetectionStrategy,
  ElementRef, ViewEncapsulation,
  OnInit, OnDestroy, AfterViewInit
} from '@angular/core';
import 'devextreme/integration/jquery';

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
  @Output() valueUpdated: EventEmitter<any> = new EventEmitter<any>();
  protected cdd: any;
  protected creatingCdd: boolean = false;
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
    if (this.cdd) {
      this.cdd.setViewOnly(!this.editMode);
      this.cdd.fitToContainer();
    } else {
      this.activate();
    }
  }

  protected onContentChanged(e) {
    this.valueUpdated.emit(this);
  }

  protected onCddInit(cdd) {
    this.cdd = cdd;
    this.creatingCdd = false;
    cdd.fitToContainer();
    cdd.setViewOnly(!this.editMode);
    if (this.value) {
      cdd.loadCDXML(this.value);
      this.value = null;
    }
    cdd.markAsSaved();
    cdd.setContentChangedHandler(e => {
      this.onContentChanged(e);
    });
  }

  protected createCdd() {
    let cddContainer = jQuery(this.elementRef.nativeElement).find('div.cdweb');
    let attachmentElement = cddContainer[0];
    if (this.cdd || this.creatingCdd || !attachmentElement) {
      return;
    }
    this.creatingCdd = true;
    const self = this;
    let params = {
      element: attachmentElement,
      viewonly: false,
      callback: function (cdd) {
        self.onCddInit(cdd);
      },
      licenseUrl: 'https://chemdrawdirect.perkinelmer.cloud/js/license.xml',
      config: { features: { enable: ['ExtendedCopyPaste'] } }
    };

    (<any>window).perkinelmer.ChemdrawWebManager.attach(params);
  };

  public setValue(value: string) {
    if (this.cdd && !this.creatingCdd) {
      this.cdd.clear();
      if (value) {
        this.cdd.loadCDXML(value);
      }
    } else {
      this.value = value;
    }
  }

  public getValue(): string {
    return this.cdd ? this.cdd.getCDXML() : this.value;
  }

  public activate() {
    if (this.activated && !this.cdd && this.id) {
      this.createCdd();
    }
  }

  public toString(): string {
    return this.getValue();
  }
};
