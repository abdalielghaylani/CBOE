import dxNotify from 'devextreme/ui/notify';

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
