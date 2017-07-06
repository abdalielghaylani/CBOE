import {
  async,
  inject,
  TestBed,
} from '@angular/core/testing';
import { TestModule } from '../../test';
import { RegNavigatorItems } from './navigator-items.component';
import { RegNavigatorModule } from './navigator.module';

describe('Component: Navigator Item Container', () => {
  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [
          TestModule,
          RegNavigatorModule
        ],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegNavigatorItems);
      fixture.detectChanges();
      done();
    });
  });

  it('should render the navigation item container with the correct classes applied',
    async(inject([], () => {
      fixture.whenStable().then(() => {
        let compiled = fixture.debugElement.nativeElement;
        expect(compiled.querySelector('div').getAttribute('class'))
          .toBe('navbar-collapse collapse');
      });
    })
    ));
});
