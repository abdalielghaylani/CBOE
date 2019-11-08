import { IInventoryContainerList } from './../../../../redux/store/registry/registry.types';
import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import validationEngine from 'devextreme/ui/validation_engine';
import { CViewGroup, CViewGroupContainer, IRegistryRecord } from '../registry-base.types';
import { IViewControl } from '../../../common';
import { IFormGroup, IForm, ICoeForm, PrivilegeUtils } from '../../../../common';
import { IAppState } from '../../../../redux';
import { CdjsService } from '../../../../services';


@Component({
  selector: 'reg-form-group-view',
  template: require('./form-group-view.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegFormGroupView implements IViewControl, OnChanges {
  @Input() id: string;
  @Input() activated: boolean;
  @Input() editMode: boolean;
  @Input() template: boolean;
  @Input() displayMode: string = 'add';
  @Input() viewModel: any;
  @Input() viewConfig: CViewGroupContainer[];
  @Input() updatable: boolean = false;
  @Input() invIntegrationEnabled: boolean = false;
  @Input() sendToInventoryEnabled: boolean = false;
  @Output() valueUpdated: EventEmitter<any> = new EventEmitter<any>();
  @Input() invContainers: IInventoryContainerList;

  constructor(private ngRedux: NgRedux<IAppState>, public cdjsService: CdjsService) {
    this.cdjsService.loadCdjsScript();
  }

  ngOnChanges() {
    this.update();
  }

  protected update() {
  }

  protected onValueUpdated(e) {
    this.validate();
    this.valueUpdated.emit(this);
  }

  validate() {
    if (this.activated && validationEngine.getGroupConfig('vg') != null) {
      let result = validationEngine.validateGroup('vg');
      return result;
    }
    return true;
  }

  private get inventoryContainersViewEnabled(): boolean {
    return this.invIntegrationEnabled
      && PrivilegeUtils.hasBatchContainersViewPrivilege(this.ngRedux.getState().session.lookups.userPrivileges)
      && this.displayMode === 'view'
      && this.invContainers && this.invContainers.containers
      && this.invContainers.containers.length > 0 ? true : false;
  }
}
