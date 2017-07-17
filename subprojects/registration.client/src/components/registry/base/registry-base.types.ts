import { IFormGroup, IForm, ICoeForm, ICoeFormMode, IFormElement } from '../../../common';

export interface IFormItemTemplate {
  editMode: boolean;
  data: any;
}

export interface IViewGroup {
  id: string;
  data: ICoeForm[];
}

export class CViewGroup implements IViewGroup {
  public id: string;
  public title: string;
  constructor(public data: ICoeForm[]) {
    this.update();
  }

  private update() {
    if (this.data.length === 1) {
      this.title = this.data[0].title;
      if (this.title) {
        this.id = this.title.toLowerCase().replace(/\s/g, '_');
      }
    }
  }

  private canAppend(f: ICoeForm): boolean {
    return this.data.length === 0 || !f.title || this.title === f.title;
  }

  private getFormElementContainer(f: ICoeForm, mode: string): ICoeFormMode {
    return mode === 'add' ? f.addMode : mode === 'edit' ? f.editMode : f.viewMode;
  }

  private setItemValue(item: any, property: string, value: any) {
    if (value) {
      item[property] = value;
    }
  }

  private getEditorType(fe: IFormElement): string {
    return fe.displayInfo.type.indexOf('COEDatePicker') > 0 ? 'dxDateBox' : 'dxTextBox';
  }

  private getDataField(fe: IFormElement): string {
    return fe._name.replace(/\s/g, '');
  }

  private getCellTemplate(fe: IFormElement): string {
    return fe.bindingExpression === 'ProjectList' ? 'projectsTemplate' : undefined;
  }

  public append(f: ICoeForm): boolean {
    let canAppend = this.canAppend(f);
    if (canAppend) {
      this.data.push(f);
      this.update();
    }
    return canAppend;
  }

  public getItems(displayMode: string): any[] {
    let items = [];
    this.data.forEach(f => {
      let formElementContainer = this.getFormElementContainer(f, displayMode);
      if (formElementContainer && formElementContainer.formElement) {
        formElementContainer.formElement.forEach(fe => {
          if (fe.displayInfo && fe.displayInfo.visible === 'true' && fe._name) {
            let item: any = {};
            this.setItemValue(item, 'label', { text: fe.label });
            this.setItemValue(item, 'editorType', this.getEditorType(fe));
            this.setItemValue(item, 'dataField', this.getDataField(fe));
            this.setItemValue(item, 'template', this.getCellTemplate(fe));
            items.push(item);
          }
        });
      }
    });
    return items;
  }
}
