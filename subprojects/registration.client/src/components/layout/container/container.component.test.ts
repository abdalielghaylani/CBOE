import {
  async,
  inject,
  TestBed,
} from '@angular/core/testing';
import { TestModule } from '../../../test';
import { RegLayoutComponentModule } from '../layout-component.module';
import { RegContainer } from './container.component';

describe('Component: Container', () => {
  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [
          TestModule,
          RegLayoutComponentModule
        ],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
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
          .toBe('regcontainer border-light background-white pb2');
      });
    })
    ));
});
