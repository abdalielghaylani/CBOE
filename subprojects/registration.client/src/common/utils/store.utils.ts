export function copyObject(source: any): any {
  let target = {};
  for (let k in source) {
    if (source.hasOwnProperty(k)) {
      target[k] = source[k];
    }
  }
  return target;
}

export function copyObjectAndSet(source: any, key: string, value: any): any {
  let target = copyObject(source);
  target[key] = value;
  return target;
}
