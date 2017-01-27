import {
  async,
  inject,
  TestBed,
} from '@angular/core/testing';
import {RegContainer} from './container.component';
import {RegUiModule} from '../ui/ui.module';
import {configureTests} from '../../tests.configure';

describe('Component: Container', () => {
  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [RegUiModule],
      });
    };

    configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegContainer);
      fixture.detectChanges();
      done();
    });
  });

  it('should have the correct class',
    async(inject([], () => {
      fixture.whenStable().then(() => {
        const compiled = fixture.debugElement.nativeElement;
        expect(compiled.querySelector('div').getAttribute('class'))
          .toBe('container-fluid pb4');
      });
    })
  ));
});
