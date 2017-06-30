import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ChemDrawWeb } from './chemdraw-web/chemdraw-web.component';

export * from './chemdraw-web/chemdraw-web.component';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule
  ],
  declarations: [
    ChemDrawWeb
  ],
  exports: [
    ChemDrawWeb
  ]
})
export class RegCommonModule { }
