import {
  Component,
  Input, 
  OnInit,
  OnDestroy,
  ChangeDetectorRef, 
  ChangeDetectionStrategy,
  ViewEncapsulation
} from '@angular/core';
import { select, NgRedux } from '@angular-redux/store';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { HttpService } from '../../services';
import { CStructureImagePrintService } from '../common/structure-image-print.service';
import { IAppState, ILookupData, CSystemSettings } from '../../redux';
import { FormGroupType, prepareFormGroupData, IFormGroup, getExceptionMessage, notify, notifyError, notifySuccess, notifyException } from '../../common';
import { apiUrlPrefix, printAndExportLimit } from '../../configuration';
import { RegistryStatus } from './registry.types';
import { CViewGroup, CViewGroupColumns } from './base';
import { forEach } from '@angular/router/src/utils/collection';

@Component({
  selector: 'reg-record-print',
  template: require('./record-print.component.html'),
  styles: [` table, tr, td 
  {
      border:solid 1px #f0f0f0 !important;
      font-size: 12px !important;
      font-family: 'Helvetica Neue', 'Segoe UI', Helvetica, Verdana, sans-serif !important;
      border-spacing: 0px !important;
  }
  img {
    max-width: 100px;
    margin-left: auto;
    margin-right: auto;
    display: block;
  }
  .center {
    text-align: center;
  }
  .green {
    color: #58a618 !important;
  }
  .red {
    color: #b71234 !important;
  }
  @media print {
    img {
      max-width: 100px !important;
      margin-left: auto !important;
      margin-right: auto !important;
      display: block !important;
    }
  }`],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})

export class RegRecordPrint implements OnInit, OnDestroy {
  @Input() temporary: boolean;
  @select(s => s.session.lookups) lookups$: Observable<ILookupData>;
  private printContents: string = '';
  private lookupsSubscription: Subscription;
  private lookups: ILookupData;
  private hitListId: number = 0;
  private sortCriteria: string;
  private selectedRows: string;
  private defaultPrintStructureImage = require('../common/assets/no-structure.png');
  private approvedIcon = require('../common/assets/approved.png');
  private notApprovedIcon = require('../common/assets/notapproved.png');
  private viewGroupsColumns: CViewGroupColumns;
  private loadIndicatorVisible: boolean = false;

  constructor(
    private router: Router,
    private http: HttpService,
    private ngRedux: NgRedux<IAppState>,
    private imageService: CStructureImagePrintService,
    private changeDetector: ChangeDetectorRef) {
  }

  ngOnInit() {
    this.loadIndicatorVisible = true;
    let urlSegments = this.router.url.split('?');
    if (urlSegments.length === 2) {
      let params = urlSegments[1].split('&');
      params.forEach(p => {
        if (p.indexOf('temp') === 0) {
          this.temporary = true;
        }
        if (p.indexOf('hitListId') === 0) {
          let param = p.split('=');
          this.hitListId = param.length === 2 ? Number(decodeURIComponent(param[1])) : 0;
        }
        if (p.indexOf('sort') === 0) {
          let param = p.split('=');
          this.sortCriteria = param.length === 2 ? decodeURIComponent(param[1]) : '';
        }
        if (p.indexOf('selected') === 0) {
          let param = p.split('=');
          this.selectedRows = param.length === 2 ? decodeURIComponent(param[1]) : '';
        }
      });
    }

    this.lookupsSubscription = this.lookups$.subscribe(d => { if (d) { this.retrieveContents(d); } });
  }

  // Trigger data retrieval for the view to show.
  retrieveContents(lookups: ILookupData) {
    this.lookups = lookups;
    let formGroupType = this.temporary ? FormGroupType.SearchTemporary : FormGroupType.SearchPermanent;
    prepareFormGroupData(formGroupType, this.ngRedux);
    let state = this.ngRedux.getState();
    let formGroup = state.configuration.formGroups[FormGroupType[formGroupType]] as IFormGroup;
    this.viewGroupsColumns = this.lookups
      ? CViewGroup.getColumns(this.temporary, formGroup, this.lookups.disabledControls, this.lookups.pickListDomains, 
        new CSystemSettings(this.lookups.systemSettings)) : new CViewGroupColumns();

    this.printRecords();
  }

  ngOnDestroy() {
    if (this.lookupsSubscription) {
      this.lookupsSubscription.unsubscribe();
    }
  }

  private print() {
    window.print();
    window.close();
  }

  private printRecords() {
    let url = `${apiUrlPrefix}hitlists/${this.hitListId}/print`;
    let params = '';
    if (this.temporary) { params += '?temp=true'; }
    params += `${params ? '&' : '?'}count=${printAndExportLimit}`;
    if (this.sortCriteria) { params += `&sort=${this.sortCriteria}`; }
    params += `&highlightSubStructures=${this.ngRedux.getState().registrysearch.highLightSubstructure}`;

    let data: any[];
    if (this.selectedRows && this.selectedRows !== '') {
      data = this.selectedRows.split(',');
    }
    url += params;
    this.http.post(url, data).toPromise()
      .then(res => {
        let rows = res.json().rows;
        let structureColumnNamae = this.temporary ? 'Structure' : 'STRUCTUREAGGREGATION';
        this.imageService.generateMultipleImages(rows.map(r => r[structureColumnNamae])).subscribe(
          (values: Array<string>) => {
            let printContents: string;
            printContents = '<table width="100%" height="auto"><tr>';
            this.viewGroupsColumns.baseTableColumns.forEach(c => {
              if (c.visible) {
                printContents += `<td>${c.caption}</td>`;
              }
            });
            this.viewGroupsColumns.batchTableColumns.forEach(c => {
              if (c.visible) {
                printContents += `<td>${c.caption}</td>`;
              }
            });
            printContents += '</tr>';
            rows.forEach(row => {
              printContents += '<tr>';
              this.viewGroupsColumns.baseTableColumns.forEach(c => {
                if (c.visible) {
                  let field = row[c.dataField];
                  if (c.caption === 'Approved') {
                    printContents += `<td rowspan=${row.BatchDataSource.length}>
                    <img style="max-width: 100px;" src="${(field === RegistryStatus.Approved) ? this.approvedIcon : this.notApprovedIcon}" /></td>`;
                  } else if (c.dataField === 'Structure' || c.dataField === 'STRUCTUREAGGREGATION') {
                    let structureImage = this.imageService.getImage(field);
                    printContents += `<td rowspan=${row.BatchDataSource.length}><img src="${structureImage ?
                      structureImage : this.defaultPrintStructureImage}" /></td>`;
                  } else if (c.dataType && c.dataType === 'date') {
                    let date = new Date(field);
                    printContents += `<td rowspan=${row.BatchDataSource.length}>
                    ${(field) ? `${date.getMonth() + 1}/${date.getDate()}/${date.getFullYear()}` : ''}</td>`;
                  } else if (c.dataType && c.dataType === 'boolean') {
                    printContents += `<td rowspan=${row.BatchDataSource.length}><div class="center">
                    <i class="fa fa-${(field === 'T') ? 'check-' : ''}square-o"></i></div></td>`;
                  } else {
                    if (c.lookup && c.lookup.dataSource) {
                      let option = c.lookup.dataSource.find(d => d.key.toString() === field);
                      if (option) {
                        field = option.value;
                      }
                    } 
                    printContents += `<td rowspan=${row.BatchDataSource.length}>${(field) ? field : ''}</td>`;
                  }
                }
              });
              let rowIndex = 0;
              row.BatchDataSource.forEach(batchRow => {
                if (rowIndex > 0) {
                  printContents += '<tr>';
                }
                this.viewGroupsColumns.batchTableColumns.forEach(c => {
                  if (c.visible) {
                    let field = batchRow[c.dataField];
                    if (c.dataType && c.dataType === 'date') {
                      let date = new Date(field);
                      printContents += `<td>${(field) ? date.toDateString() : ''}</td>`;
                    } else if (c.dataType && c.dataType === 'boolean') {
                      printContents += `<td><div class="center">
                      <i class="fa fa-${(field === 'T') ? 'check-' : ''}square-o"></i></div></td>`;
                    } else {
                      if (c.lookup && c.lookup.dataSource) {
                        let option = c.lookup.dataSource.find(d => d.key.toString() === field);
                        if (option) {
                          field = option.value;
                        }
                      } 
                      printContents += `<td >${(field) ? field : ''}</td>`;
                    }
                  }
                });
                printContents += '</tr>';
              });
            });

            this.printContents = printContents;
            this.changeDetector.markForCheck();
            setTimeout( () => { this.print(); }, 500);           
            this.loadIndicatorVisible = false;
          });
      })
      .catch(error => {
  
      });
  }
}
