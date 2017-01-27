import {
  async,
  inject,
  TestBed,
} from '@angular/core/testing';
import { RegNavigatorItems } from './navigator-items.component';
import { RegNavigatorModule } from './navigator.module';
import { configureTests } from '../../tests.configure';

describe('Component: Navigator Item Container', () => {
  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [RegNavigatorModule],
      });
    };

    configureTests(configure).then(testBed => {
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
