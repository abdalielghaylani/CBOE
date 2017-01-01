import { Injectable } from '@angular/core';
import { NgRedux } from 'ng2-redux';
import { IAppState } from '../store';

@Injectable()
export class HomeDetailActions {
  static SUBMIT = 'SUBMIT';

  constructor(private ngRedux: NgRedux<IAppState>) { }

  submit() {
    this.ngRedux.dispatch({ type: HomeDetailActions.SUBMIT });
  }

}
