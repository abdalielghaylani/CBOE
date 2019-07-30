import { Component, Input } from '@angular/core';
declare var jQuery: any;

@Component({
  selector: 'reg-xml-viewer',
  styles: [
    require('./xml-viewer.component.css')],
  template: `<span id="xmlId" class="simpleXML"></span>`
})
export class RegXmlViewer {
  @Input() xml;

  ngOnChanges() {
    let result = this.getXML({ xmlString: this.xml });
    document.getElementById('xmlId').innerHTML = result;
    $('.simpleXML-expanderHeader').on('click', function (event) {
      let expanderHeader = $(this).closest('.simpleXML-expanderHeader');

      let expander = expanderHeader.find('.simpleXML-expander');

      let content = expanderHeader.parent().find('.simpleXML-content').first();
      let collapsedText = expanderHeader.parent().children('.simpleXML-collapsedText').first();
      let closeExpander = expanderHeader.parent().children('.simpleXML-expanderClose').first();

      if (expander.hasClass('simpleXML-expander-expanded')) {
        // Already Expanded, therefore collapse time...
        expander.removeClass('simpleXML-expander-expanded').addClass('simpleXML-expander-collapsed');

        collapsedText.attr('style', 'display: inline;');
        content.attr('style', 'display: none;');
        closeExpander.attr('style', 'display: none');
      } else {
        // Time to expand..
        expander.addClass('simpleXML-expander-expanded').removeClass('simpleXML-expander-collapsed');
        collapsedText.attr('style', 'display: none;');
        content.attr('style', '');
        closeExpander.attr('style', '');
      }
    });
  }

  getXML(options) {
    let settings = $.extend({
      // These are the defaults.
      collapsedText: '...',
    }, options);
    if (settings.xml === undefined && settings.xmlString === undefined) {
      throw 'No XML to be displayed was supplied';
    }
    if (settings.xml !== undefined && settings.xmlString !== undefined) {
      throw 'Only one of xml and xmlString may be supplied';
    }
    let xml = settings.xml;
    if (xml === undefined) {
      xml = $.parseXML(settings.xmlString);
    }
    let wrapperNode = document.createElement('span');
    $(wrapperNode).addClass('simpleXML');
    this.showNode(wrapperNode, xml, settings);
    return $(wrapperNode)[0].innerHTML;
  }

  showNode(parent, xml, settings) {
    if (xml.nodeType === 9) {
      for (let i = 0; i < xml.childNodes.length; i++) {
        this.showNode(parent, xml.childNodes[i], settings);
      }
      return;
    }

    switch (xml.nodeType) {
      case 1: // Simple element
        {
          let hasChildNodes = xml.childNodes.length > 0;
          let expandingNode = hasChildNodes && (xml.childNodes.length > 1 || xml.childNodes[0].nodeType !== 3);

          let expanderHeader = expandingNode ? this.makeSpan1('', 'simpleXML-expanderHeader') : parent;

          let expanderSpan = this.makeSpan1('', 'simpleXML-expander');
          if (expandingNode) {
            $(expanderSpan).addClass('simpleXML-expander-expanded');
          }
          expanderHeader.appendChild(expanderSpan);

          expanderHeader.appendChild(this.makeSpan1('<', 'simpleXML-tagHeader'));
          expanderHeader.appendChild(this.makeSpan1(xml.nodeName, 'simpleXML-tagValue'));

          if (expandingNode) {
            parent.appendChild(expanderHeader);
          }

          // Handle attributes
          let attributes = xml.attributes;
          for (let attrIdx = 0; attrIdx < attributes.length; attrIdx++) {
            expanderHeader.appendChild(this.makeSpan(' '));
            expanderHeader.appendChild(this.makeSpan1(attributes[attrIdx].name, 'simpleXML-attrName'));
            expanderHeader.appendChild(this.makeSpan('="'));
            expanderHeader.appendChild(this.makeSpan1(attributes[attrIdx].value, 'simpleXML-attrValue'));
            expanderHeader.appendChild(this.makeSpan('"'));
          }

          // Handle child nodes
          if (hasChildNodes) {

            parent.appendChild(this.makeSpan1('>', 'simpleXML-tagHeader'));

            if (expandingNode) {
              let ulElement = document.createElement('ul');
              for (let i = 0; i < xml.childNodes.length; i++) {
                let liElement = document.createElement('li');
                this.showNode(liElement, xml.childNodes[i], settings);
                ulElement.appendChild(liElement);
              }

              let collapsedTextSpan = this.makeSpan1(settings.collapsedText, 'simpleXML-collapsedText');
              collapsedTextSpan.setAttribute('style', 'display: none;');
              ulElement.setAttribute('class', 'simpleXML-content');
              parent.appendChild(collapsedTextSpan);
              parent.appendChild(ulElement);

              parent.appendChild(this.makeSpan1('', 'simpleXML-expanderClose'));
            } else {
              parent.appendChild(this.makeSpan(xml.childNodes[0].nodeValue));
            }

            // Closing tag
            parent.appendChild(this.makeSpan1('</', 'simpleXML-tagHeader'));
            parent.appendChild(this.makeSpan1(xml.nodeName, 'simpleXML-tagValue'));
            parent.appendChild(this.makeSpan1('>', 'simpleXML-tagHeader'));
          } else {
            let closingSpan = document.createElement('span');
            closingSpan.innerText = '/>';
            parent.appendChild(closingSpan);
          }
        }
        break;

      case 3: // text
        {
          if (xml.nodeValue.trim() !== '') {
            parent.appendChild(this.makeSpan1('', 'simpleXML-expander'));
            parent.appendChild(this.makeSpan(xml.nodeValue));
          }
        }
        break;

      case 4: // cdata
        {
          parent.appendChild(this.makeSpan1('', 'simpleXML-expander'));
          parent.appendChild(this.makeSpan1('<![CDATA[', 'simpleXML-tagHeader'));
          parent.appendChild(this.makeSpan1(xml.nodeValue, 'simpleXML-cdata'));
          parent.appendChild(this.makeSpan1(']]>', 'simpleXML-tagHeader'));
        }
        break;

      case 8: // comment
        {
          parent.appendChild(this.makeSpan1('', 'simpleXML-expander'));
          parent.appendChild(this.makeSpan1('<!--' + xml.nodeValue + '-->', 'simpleXML-comment'));
        }
        break;

      default:
        {
          let item = document.createElement('span');
          item.innerText = '' + xml.nodeType + ' - ' + xml.name;
          parent.appendChild(item);
        }
        break;
    }
  }

  makeSpan(innerText) {
    return this.makeSpan1(innerText, undefined);
  }

  makeSpan1(innerText, classes) {
    let span = document.createElement('span');
    span.innerText = innerText;

    if (classes !== undefined) {
      span.setAttribute('class', classes);
    }

    return span;
  }

}
