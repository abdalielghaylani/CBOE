import dxNotify from 'devextreme/ui/notify';

const notificationDuration = 2000;

export function getExceptionMessage(baseMessage: string, error): string {
  let errorResult, reason;
  if (error._body) {
    errorResult = JSON.parse(error._body);
    reason = errorResult.Message;
  }
  return baseMessage + ((reason) ? ': ' + reason : '!');
}

export function notify(message: string, type: string, duration: number = notificationDuration) {
  dxNotify(message, type, duration);
}

export function notifyError(message: string, duration: number = notificationDuration) {
  dxNotify(message, 'error', duration);
}

export function notifyException(message: string, error, duration: number = notificationDuration) {
  notifyError(getExceptionMessage(message, error), duration);  
}

export function notifySuccess(message: string, duration: number = notificationDuration) {
  dxNotify(message, 'success', duration);  
}
