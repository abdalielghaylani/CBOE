import { TestBed, async, inject } from '@angular/core/testing';
import { RegHeader } from './header.component';
import { TestModule } from '../../../test/test.module';

describe('Component : Header Component', () => {

  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [ TestModule ],
        declarations : [ RegHeader ],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegHeader);
      fixture.detectChanges();
      done();
    });
  });

  it('should create RegHeader Component', async(inject([], () => {
    fixture.whenStable().then(() => {
      expect(fixture.componentInstance).toBeDefined();
    });
  })));

});
