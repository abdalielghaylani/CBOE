export { }

declare global {
  interface String {
    format(any): String;
    encodeHtml(): String;
  }
}
