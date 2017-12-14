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

  protected togglePanel(event) {
    const target = event.target || event.srcElement;
    if (target.children.length > 0) {
      target.children[0].click();
    }
  }
};
