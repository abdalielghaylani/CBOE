import { DOMParser, DOMParserStatic, XMLSerializer } from 'xmldom';

export function getDocument(data: string): Document {
  return new DOMParser().parseFromString(data);
}

function fixStructureElement(element: Element) {
  if (element.textContent.startsWith('VmpD')) {
    element.textContent = element.textContent.replace(/[\r\n]/g, '');
  }
}

function fixStructureElements(elements: NodeListOf<Element>) {
  for (let i = 0; i < elements.length; ++i) {
    fixStructureElement(elements.item(i));
  }
}
export function fixStructureData(data: Document) {
  fixStructureElements(data.getElementsByTagName('Structure'));
  fixStructureElements(data.getElementsByTagName('StructureAggregation'));
  fixStructureElements(data.getElementsByTagName('NormalizedStructure'));
}

export function serializeData(data: Document): string {
  return new XMLSerializer().serializeToString(data.documentElement);
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
