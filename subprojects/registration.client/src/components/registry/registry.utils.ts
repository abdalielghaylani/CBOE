import { DOMParser, DOMParserStatic, XMLSerializer } from 'xmldom';
import { isUserHasPrivilege } from '../../common/utils/view.utils';

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

export function getElement(element: Element, path: string): Element {
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
        return nextElement;
      } else {
        return this.getElement(nextElement, pathSegments.join('/'));
      }
    }
  }
  return null;
}

export function getElementValue(element: Element, path: string): string {
  let nextElement = this.getElement(element, path);
  return nextElement ? nextElement.textContent : null;
}

export function setElementValue(element: Element, path: string, value: string) {
  let nextElement = this.getElement(element, path);
  if (nextElement) {
    nextElement.textContent = value;
  }
}

export function hasDeleteRecordPrivilege(temporary: boolean, userPrivileges: any[]): boolean {
  let privilege = temporary ? 'DELETE_TEMP' : 'DELETE_REG';
  return isUserHasPrivilege(privilege, userPrivileges);
}
