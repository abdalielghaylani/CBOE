import { Component, Input, Output, EventEmitter, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ConfigurationActions } from '../../actions/configuration.actions';
import { IConfiguration } from '../../store';
import * as _ from 'lodash';

@Component({
  selector: 'reg-configuration',
  template: `
    <div class="container">
      <h4 data-testid="configuration-heading" id="qa-configuration-heading">{{ this.tableName() }}</h4>

      <dx-data-grid [dataSource]=this.configuration.rows [paging]='{pageSize: 10}' 
        [pager]='{ showPageSizeSelector: true, allowedPageSizes: [5, 10, 20], showInfo: true }'
        [searchPanel]='{ visible: true }' [filterRow]='{ visible: true }' (onRowRemoving)='deleteRecord($event)'
        (onInitNewRow)='addRecord()' (onEditingStart)='editRecord($event)' rowAlternationEnabled=true,
        [editing]='{ mode: form, allowUpdating: true, allowDeleting: true, allowAdding: true }'>
      </dx-data-grid>
    </div>
  `,
})
export class RegConfiguration implements OnInit, OnDestroy {
  @Input() configuration: IConfiguration;
  private sub: any;

  constructor(private route: ActivatedRoute, private configurationActions: ConfigurationActions) { }

  ngOnInit() {
    this.sub = this.route.params.subscribe(params => {
      let paramLabel = 'tableId';
      this.configurationActions.openTable(params[paramLabel]);
    });
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  tableName() {
    return this.configuration.tableId.split('-').map(n => _.upperFirst(n)).join(' ');
  }
};
