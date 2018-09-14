import * as dxDialog from 'devextreme/ui/dialog';
import dxNotify from 'devextreme/ui/notify';

const notificationDuration = 2000;

export function getExceptionMessage(baseMessage: string, error): string {
  let errorResult;
  let reason: string;
  if (error._body) {
    try {
      errorResult = JSON.parse(error._body);
      reason = errorResult.Message;
      if (reason.endsWith('!')) {
        reason = reason.substring(0, reason.length - 1);
      }
    } catch (e) {
      // Ignore JSON parse error, in case the error message is due to login failure
    }
  }

  if (!reason) {
    return 'Your session has expired. Please login to continue.';
  }
  return baseMessage + ((reason) ? ': ' + reason : '!');
}

export function notify(message: string, type: string, duration: number = notificationDuration) {
  if (type === 'success') {
    dxNotify(message, 'success', notificationDuration);
    return;
  } else if (type === 'error') {
    type = 'Error';
  } else if (type === 'warning') { type = 'Warning'; }
  dxDialog.alert(message, type);
}

export function notifyError(message: string, duration: number = notificationDuration) {
  dxDialog.alert(message, 'Error');
}

export function notifyWarning(message: string, duration: number = notificationDuration) {
  dxDialog.alert(message, 'Warning');
}

export function notifyException(message: string, error, duration: number = notificationDuration) {
  dxDialog.alert(getExceptionMessage(message, error), 'Error');
}

export function notifySuccess(message: string, duration: number = notificationDuration) {
  dxNotify(message, 'success', notificationDuration);
}

export function b64Encode(utf8Array) {
  let chunkSize = 0x8000;
  let c = [];
  for (let i = 0; i < utf8Array.length; i += chunkSize) {
    c.push(String.fromCharCode.apply(null, utf8Array.subarray(i, i + chunkSize)));
  }
  return btoa(c.join(''));
}
