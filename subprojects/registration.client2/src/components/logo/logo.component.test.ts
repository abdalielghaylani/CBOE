import {
  async,
  inject,
  TestBed,
} from '@angular/core/testing';
import { RegLogo } from './index';
import { RegUiModule } from '../ui/ui.module';
import { configureTests } from '../../tests.configure';

describe('Component: Logo', () => {
  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [RegUiModule],
      });
    };

    configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegLogo);
      fixture.detectChanges();
      done();
    });
  });

  it('should set the image location',
    async(inject([], () => {
      fixture.whenStable().then(() => {
        fixture.componentInstance.LogoImage = 'data:image/gif;base64,fake';
        fixture.detectChanges();
        let compiled = fixture.debugElement.nativeElement;
        expect(compiled.querySelector('img').getAttribute('src'))
          .toBe('data:image/gif;base64,fake');
      });
    })));
});
