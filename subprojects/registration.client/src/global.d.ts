export { }

declare global {
  interface String {
    format(any): String;
    decodeHtml(): String;
    encodeHtml(): String;
  }
}
