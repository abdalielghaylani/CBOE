import { Component, Input, Output, EventEmitter, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import * as _ from 'lodash';

@Component({
  selector: 'reg-configuration',
  template: `
    <div class="container">
      <h4 data-testid="configuration-heading" id="qa-configuration-heading">
        {{ this.tableName() }}
      </h4>

      <dx-data-grid [columns]='gridColumns' [dataSource]='records' [paging]='{pageSize: 10}' 
        [pager]='{ showPageSizeSelector: true, allowedPageSizes: [5, 10, 20], showInfo: true }'
        [searchPanel]='{ visible: true }' [filterRow]='{ visible: true }' (onRowRemoving)='deleteRecord($event)'
        (onInitNewRow)='addRecord()' (onEditingStart)='editRecord($event)' rowAlternationEnabled=true,
        [editing]='{ mode: form, allowUpdating: true, allowDeleting: true, allowAdding: true }'>
      </dx-data-grid>
    </div>
  `,
})
export class RegConfiguration implements OnInit, OnDestroy {
  @Input() tableId: string = 'projects';
  private sub: any;

  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.sub = this.route.params.subscribe(params => {
      let paramLabel = 'tableId';
      this.tableId = params[paramLabel];
    });
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  tableName() {
    return _.upperFirst(this.tableId);
  }
};
