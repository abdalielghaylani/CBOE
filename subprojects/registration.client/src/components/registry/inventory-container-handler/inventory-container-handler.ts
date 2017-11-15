
import { Component } from '@angular/core';
import * as dxDialog from 'devextreme/ui/dialog';
import { invIntegrationBasePath } from '../../../configuration';
import { IRegInvModel } from '../registry.types';

@Component({
  selector: 'inventory-container-handler',
  template: ''
})
export class RegInvContainerHandler {

  private warningText: string = `Inventory integration features are supported in Internet Explorer
    only. In order to create container(s) please use Internet Explorer web browser`;

  constructor() {
  }

  public createContainer(invModel: IRegInvModel) {
    if (!this.isBrowserIE()) { dxDialog.alert(this.warningText, `Warning`); return; }
    let chemInvUrlSection: string = invModel.isBulkContainerCreation
      ? (`cheminv/gui/ImportFromChemReg.asp?RegIDList=${invModel.batchIDs.join()}&OpenAsModalFrame=false`)
      : (`cheminv/gui/CreateOrEditContainer.asp?GetData=new&vRegBatchID=${invModel.batchIDs[0]}&RefreshOpenerLocation=true`);
    let windowParams: string = invModel.isBulkContainerCreation
      ? (`width=1100,height=700`)
      : (`width=800,height=700`);
    this.openContainerPopup(chemInvUrlSection, 'Create New Container', windowParams);
  }

  private isBrowserIE() {
    return (!!navigator.userAgent.match(/Trident/g)
      || !!navigator.userAgent.match(/MSIE/g));
  }

  public openContainerPopup(url, title, params) {
    if (!this.isBrowserIE()) { dxDialog.alert(this.warningText, `Warning`); return; }
    let windowParams = params ? params : `width=800,height=700`;
    window.open((invIntegrationBasePath + url), title, windowParams);
    window.opener = null;
  }

};
