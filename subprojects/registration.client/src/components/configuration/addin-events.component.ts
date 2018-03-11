import {
  Component, EventEmitter, Input, Output, ElementRef, OnChanges, OnInit,
  ChangeDetectionStrategy, ViewEncapsulation, ViewChild, ChangeDetectorRef
} from '@angular/core';
import { HttpService } from '../../services';
import { RegConfigBaseComponent } from './config-base';
import { ILookupData } from '../../redux';
import { DxDataGridComponent, DxFormComponent } from 'devextreme-angular';

@Component({
  selector: 'reg-addin-events-template',
  template: `
  <dx-data-grid [dataSource]="eventSource.events ? eventSource.events : []" [disabled]="disabled"
  [filterRow]='{ visible: true }' width="100%" [height]="200" rowAlternationEnabled="true" 
  (onInitialized)='onInitialized($event)' (onCellPrepared)='onCellPrepared($event)'
  (onRowInserted)="onDataSourceChange($event)" (onRowUpdated)="onDataSourceChange($event)" (onRowRemoved)="onDataSourceChange($event)">
  <dxi-column dataField="eventName" caption="Event" editorType="dxSelectBox" [width]="200" dataType="string">
  <dxo-lookup [dataSource]="eventNameSrc"></dxo-lookup>
  <dxi-validation-rule type='required' message='Event required'></dxi-validation-rule>
  </dxi-column>
  <dxi-column dataField="eventHandler" caption="Handler" editorType="dxSelectBox" [width]="200" dataType="string">
  <dxo-lookup [dataSource]="eventHandlerSrc" valueExpr="name"  displayExpr="name"></dxo-lookup>
  <dxi-validation-rule type='required' message='Handler required'></dxi-validation-rule>
  </dxi-column>
  <dxo-editing mode="form" [allowUpdating]="true" [allowDeleting]="true" [allowAdding]="true"></dxo-editing>
  <dxo-scrolling mode="infinite"></dxo-scrolling>
</dx-data-grid> `,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegAddinEventsListItem extends RegConfigBaseComponent implements OnInit, OnChanges {
  @Input() disabled: boolean = false;
  @Input() eventSource: any = [];
  @ViewChild(DxDataGridComponent) grid;
  private eventNameSrc: any = [];
  private eventHandlerSrc: any = [];
  @Input() addinName: string;
  @Input() addinAssemblies: any;
  @Output() valueUpdated: EventEmitter<any> = new EventEmitter<any>();

  constructor(elementRef: ElementRef, http: HttpService, private changeDetecter: ChangeDetectorRef) {
    super(elementRef, http);
  }

  ngOnChanges() {
    this.update();
  }

  update() {
    if (this.addinName) {
      let val = this.addinAssemblies[0].classes.find(i => i.name.endsWith(this.addinName));
      this.eventNameSrc = ['Loaded', 'Inserting', 'Inserted', 'Updating', 'Updated', 'Registering',
        'UpdatingPerm', 'Saving', 'Saved', 'PropertyChanged'];
      this.eventHandlerSrc = (val && val.eventHandlers) ? val.eventHandlers : [];
    }
    this.changeDetecter.markForCheck();
  }

  public hasEditData(): boolean {
    return this.grid.instance ? this.grid.instance.hasEditData() : false;
  }
  onDataSourceChange(e) {
    this.valueUpdated.emit(
      (e.component && e.component.getDataSource()._items) ? e.component.getDataSource()._items : []);
  }
};
