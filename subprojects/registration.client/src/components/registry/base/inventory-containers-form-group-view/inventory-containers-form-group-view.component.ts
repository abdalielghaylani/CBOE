import { Component, ChangeDetectionStrategy, OnInit, OnDestroy, ChangeDetectorRef, ElementRef, ViewChild, Input } from '@angular/core';
import { IInventoryContainerList } from '../../../../redux/index';
@Component({
  selector: 'inventory-containers-form-group-view',
  template: require('./inventory-containers-form-group-view.component.html'),
  styles: [require('../registry-base.css')], 
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class InventoryContainersFormGroup {
  private id: string = 'invContainerForm';
  @Input() invContainers: IInventoryContainerList;

  protected togglePanel(e) {
    if (e.srcElement.children.length > 0) {
      e.srcElement.children[0].click();
    }
  }
};
