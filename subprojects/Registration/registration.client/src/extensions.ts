export { }

declare global {
  interface String {
    format(any): String;
    decodeHtml(): String;
    encodeHtml(): String;
  }
}

declare global {
  interface Window {
    NewRegWindowHandle: any;
  }
}

String.prototype.format = String.prototype.format ||
  function () {
    'use strict';
    let str = this.toString();
    if (arguments.length) {
      let t = typeof arguments[0];
      let args = ('string' === t || 'number' === t) ? Array.prototype.slice.call(arguments) : arguments[0];
      for (let key in args) {
        if (args.hasOwnProperty(key)) {
          str = str.replace(new RegExp('\\{' + key + '\\}', 'gi'), args[key]);
        }
      }
    }
    return str;
  };

String.prototype.decodeHtml = String.prototype.decodeHtml ||
  function () {
    'use strict';
    return jQuery('<textarea/>').html(this.toString()).text();
  };

String.prototype.encodeHtml = String.prototype.encodeHtml ||
  function () {
    'use strict';
    return jQuery('<textarea/>').text(this.toString()).html();
  };
