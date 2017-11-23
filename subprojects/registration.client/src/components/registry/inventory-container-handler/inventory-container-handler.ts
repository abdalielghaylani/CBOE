
import { Component } from '@angular/core';
import * as dxDialog from 'devextreme/ui/dialog';
import { invIntegrationBasePath, invNormalWindowParams } from '../../../configuration';

export class RegInvContainerHandler {

  private warningText: string = `Inventory integration features are supported in Internet Explorer
    only. In order to create container(s) please use Internet Explorer web browser`;
    
  constructor() {
  }

  private isBrowserIE() {
    return (!!navigator.userAgent.match(/Trident/g)
      || !!navigator.userAgent.match(/MSIE/g));
  }

  public openContainerPopup(url: string, params: string) {
    if (!this.isBrowserIE()) { dxDialog.alert(this.warningText, `Warning`); return; }
    let windowParams = params ? params : invNormalWindowParams;
    window.open((invIntegrationBasePath + url), '_blank', windowParams);
    window.opener = null;
  }

};
