
import { Component } from '@angular/core';
import * as dxDialog from 'devextreme/ui/dialog';
import { invIntegrationBasePath } from '../../../configuration';

export class RegInvContainerHandler {

  private warningText: string = `Inventory integration features are supported in Internet Explorer
    only. In order to create container(s) please use Internet Explorer web browser`;
    
  constructor() {
  }

  public openCreateContainerDetailView(RegBatchId: string) {
    let invUrlSection = `cheminv/gui/CreateOrEditContainer.asp?GetData=new&vRegBatchID=${RegBatchId}&RefreshOpenerLocation=true`;
    this.openContainerPopup(invUrlSection, 'Create New Container', null);
  }

  public openCreateContainerListView(RegIds: string[]) {
    let invUrlSection = `cheminv/gui/ImportFromChemReg.asp?RegIDList=${RegIds.join()}&OpenAsModalFrame=true`;
    let windowParams = `width=1100,height=700`;
    this.openContainerPopup(invUrlSection, 'Create New Container', windowParams);
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
