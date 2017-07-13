import { CFormGroup, CForm, CCoeForm } from '../../../common';

export interface IFormItemTemplate {
  editMode: boolean;
  data: any;
}

export interface IViewGroup {
  id: string;
  data: CCoeForm[];
}

export class CViewGroup implements IViewGroup {
  public id: string;
  public title: string;
  constructor(public data: CCoeForm[]) {
    this.update();
  }

  private update() {
    if (this.data.length === 1) {
      this.title = this.data[0].title;
      if (this.title) {
        this.id = this.title.toLowerCase().replace(' ', '_');
      }
    }
  }

  private canAppend(f: CCoeForm): boolean {
    return this.data.length === 0 || !f.title || this.title === f.title;
  }

  public append(f: CCoeForm): boolean {
    let canAppend = this.canAppend(f);
    if (canAppend) {
      this.data.push(f);
      this.update();
    }
    return canAppend;
  }
}
