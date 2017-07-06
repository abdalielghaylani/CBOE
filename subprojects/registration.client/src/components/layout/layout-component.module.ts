import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { RegContainer } from './container';
import { RegFooter } from './footer';
import { RegHeader } from './header';
import { RegLogo, RegLoginLogo } from './logo';

export * from './container';
export * from './footer';
export * from './header';
export * from './logo';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule
  ],
  declarations: [
    RegContainer,
    RegFooter, RegHeader,
    RegLogo, RegLoginLogo
  ],
  exports: [
    RegContainer,
    RegFooter, RegHeader,
    RegLogo, RegLoginLogo
  ]
})
export class RegLayoutComponentModule { }
