import { Input, Component, Directive, EventEmitter, Output } from '@angular/core';
import { async, inject, TestBed } from '@angular/core/testing';
import { TestModule } from '../../test';
import { RegNavigatorItems } from './navigator-items.component';
import { RegNavigatorItem } from './navigator-item.directive';
import { RegNavigatorModule } from './navigator.module';

@Component({
  selector: 'container',
  template: `
    <reg-navigator-items>
      <li reg-directive-item>Item 1</li>
      <li reg-directive-item>Item 2</li>
      <li reg-directive-item>Item 3</li>
    </reg-navigator-items>
  `
})
export class Container { }

describe('Directive: Navigator Item', () => {
  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        declarations: [Container],
        imports: [
          TestModule,
          RegNavigatorModule
        ],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(Container);
      fixture.detectChanges();
      done();
    });
  });

  it('should render the navigator item with the correct classes applied',
    async(inject([],
      () => {
        fixture.whenStable().then(() => {
          let compiled = fixture.debugElement.nativeElement;
          let items = compiled.querySelectorAll('li');
          expect(items.length).toBe(3);
          expect(items[0].hasAttribute('reg-navigator-item'));
        });
      })
    ));
});
