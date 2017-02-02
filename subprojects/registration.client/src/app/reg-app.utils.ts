import { Inject } from '@angular/core';
import { NgRedux } from 'ng2-redux';
import dxNotify from 'devextreme/ui/notify';
import { SessionActions } from '../actions';
import { IAppState } from '../store';

const notificationDuration = 2000;

export function notify(message: string, type: string, duration: number = notificationDuration) {
  dxNotify(message, type, duration);
}

export function notifyError(message: string, duration: number = notificationDuration) {
  dxNotify(message, 'error', duration);
}

export function notifySuccess(message: string, duration: number = notificationDuration) {
  dxNotify(message, 'success', duration);  
}
