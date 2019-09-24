import * as dxDialog from 'devextreme/ui/dialog';
import dxNotify from 'devextreme/ui/notify';

const notificationDuration = 2000;

export function getExceptionMessage(baseMessage: string, error, isTitleRequired = false): any {
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
    const text = 'Your session has expired. Please login to continue.';
    return isTitleRequired ? { message: text, title: 'Session Expired' } : text;
  }
  const text = baseMessage + ((reason) ? ': ' + reason : '!');
  return isTitleRequired ? { message: text, title: 'Error' } : text;
}

export function notify(message: string, type: string, duration: number = notificationDuration) {
  if (type === 'success') {
    dxNotify({ message: message, position: { my: 'Center', at: 'Center', of: window } }, 'success', notificationDuration);
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
  let res = getExceptionMessage(message, error, true);
  dxDialog.alert(res.message, res.title);
}

export function notifySuccess(message: string, duration: number = notificationDuration) {
  let dynamicWidth = getwidth(message);
  dxNotify(
    {
      message: message,
      position: { my: 'Center', at: 'Center', of: window }, width: dynamicWidth
    },
    'success',
    notificationDuration);
}

function getwidth(val) {
  let width = val.length;
  if (window.innerWidth < (width * 6 + 100)) {
    return (window.innerWidth - 100) + 'px';
  } else {
    return (width + 8).toString() + 'ex';
  }
}

export function b64Encode(utf8Array) {
  let chunkSize = 0x8000;
  let c = [];
  for (let i = 0; i < utf8Array.length; i += chunkSize) {
    c.push(String.fromCharCode.apply(null, utf8Array.subarray(i, i + chunkSize)));
  }
  return btoa(c.join(''));
}
