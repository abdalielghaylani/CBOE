export { }

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

String.prototype.encodeHtml = String.prototype.encodeHtml ||
  function () {
    'use strict';
    return this.replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;')
      .replace(/'/g, '&apos;');
  };
