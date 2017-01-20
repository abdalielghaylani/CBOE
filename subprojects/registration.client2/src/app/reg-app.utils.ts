import { Inject } from '@angular/core';
import { NgRedux } from 'ng2-redux';
import notify from 'devextreme/ui/notify';
import { SessionActions } from '../actions';
import { IAppState } from '../store';

const notificationDuration = 2000;

export function notifyError(message: string) {
  notify(message, 'error', notificationDuration);
}

export function notifySuccess(message: string) {
  notify(message, 'success', notificationDuration);  
}
