import { Directive, ElementRef, HostListener, HostBinding, Input } from '@angular/core';

@Directive({
  selector: '[reg-navigator-item]',
})
export class RegNavigatorItem {
  constructor(private el: ElementRef) { }
  @Input() testid: string;
};
