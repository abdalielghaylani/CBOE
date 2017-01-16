import { DOMParser, DOMParserStatic, XMLSerializer } from 'xmldom';

export function getDocument(data: string): Document {
  return new DOMParser().parseFromString(data);
}

export function getDocumentElement(data: string) {
  let output = this.getDocument(data);
  return new DOMParser().parseFromString(output.documentElement.firstChild.textContent).documentElement;
}

export function getElementValue(element: Element, path: string): string {
  let pathSegments: string[] = path.split('/');
  let first: string = pathSegments.shift();
  if (first) {
    let nextElement: Element = null;
    let elements = element.getElementsByTagName(first);
    for (let i = 0; i < elements.length; ++i) {
      let e = elements.item(i);
      if (e.parentNode === element) {
        nextElement = e;
        break;
      }
    }
    if (nextElement) {
      if (pathSegments.length === 0) {
        return nextElement.textContent;
      } else {
        return this.getElementValue(nextElement, pathSegments.join('/'));
      }
    }
  }
  return null;
}
