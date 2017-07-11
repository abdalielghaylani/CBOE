import {
  async,
  inject,
  TestBed,
} from '@angular/core/testing';
import { TestModule } from '../../test';
import { RegLoginModal } from './login-modal';
import { RegLoginModule } from './login.module';

describe('Component: Login Modal', () => {
  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [
          TestModule,
          RegLoginModule
        ],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegLoginModal);
      fixture.detectChanges();
      done();
    });
  });

  // it('should create the component', async(inject([], () => {
  //   fixture.whenStable().then(() => {
  //     fixture.detectChanges();
  //     let element = fixture.nativeElement;
  //     expect(element.querySelector('reg-modal-content')).not.toBeNull();
  //     expect(element.querySelector('h1').innerText).toEqual('Login');
  //     expect(element.querySelector('reg-login-form')).not.toBeNull();
  //     expect(fixture.componentInstance.onSubmit).toBeTruthy();
  //   });
  // })));

  it('should emit an event when handleSubmit is called',
    async(inject([], () => {
      fixture.whenStable().then(() => {
        let login = { username: 'user', password: 'pass' };
        fixture.componentInstance.handleSubmit(login);
        fixture.componentInstance.onSubmit.subscribe(data => {
          expect(data).toBeDefined();
          expect(data.username).toEqual('user');
          expect(data.password).toEqual('pass');
        });
      });
    }))
  );
});
