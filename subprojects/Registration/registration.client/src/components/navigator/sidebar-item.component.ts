import { Directive, ElementRef, HostListener, HostBinding, Input } from '@angular/core';

@Directive({
  selector: '[reg-sidebar-item]',
})
export class RegSidebarItem {
  constructor(private el: ElementRef) { }
  @Input() testid: string;
};
