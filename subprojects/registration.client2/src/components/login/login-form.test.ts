import {
  async,
  inject,
  TestBed,
} from '@angular/core/testing';
import {RegLoginForm} from './index';
import {RegLoginModule} from './login.module';
import {configureTests} from '../../tests.configure';

describe('Component: Login Form', () => {
  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [RegLoginModule],
      });
    };

    configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegLoginForm);
      fixture.detectChanges();
      done();
    });
  });

  it('should create the component', async(inject([], () => {
    fixture.whenStable().then(() => {
      let element = fixture.nativeElement;
      expect(element.querySelector('#qa-pending-alert')).toBeNull();
      expect(element.querySelector('#qa-alert')).toBeNull();
      expect(element.querySelector('#qa-uname-input')).not.toBeNull();
      expect(element.querySelector('#qa-uname-validation').className)
        .toContain('display-none');
      expect(element.querySelector('#qa-password-input')).not.toBeNull();
      expect(element.querySelector('#qa-password-validation').className)
        .toContain('display-none');
      expect(element.querySelector('#qa-login-button')).not.toBeNull();
      expect(fixture.componentInstance.onSubmit).toBeTruthy();
    });
  })));

  it('should display alert if the form hasError', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.componentInstance.hasError = true;
      fixture.autoDetectChanges();
      let alert = fixture.nativeElement.querySelector('#qa-alert');
      expect(alert).not.toBeNull();
      expect(alert.innerText).toEqual('Invalid username and password');
    });
  })));

  it('should display alert if the form isPending', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.componentInstance.isPending = true;
      fixture.autoDetectChanges();
      let alert = fixture.nativeElement.querySelector('#qa-pending');
      expect(alert).not.toBeNull();
      expect(alert.innerText).toEqual('Loading...');
    });
  })));

  it('should display name warning for invalid username',
    async(inject([], () => {
      fixture.whenStable().then(() => {
        fixture.componentInstance.username.setValue('');
        fixture.autoDetectChanges();
        let alert = fixture.nativeElement.querySelector('#qa-uname-validation');
        expect(alert).not.toBeNull();
        expect(alert.innerText).toEqual('Username is required.');
      });
    })
  ));

  it('should display password warning for invalid password',
    async(inject([], () => {
      fixture.whenStable().then(() => {
        fixture.componentInstance.password.setValue('');
        fixture.autoDetectChanges();
        let alert = fixture.nativeElement
          .querySelector('#qa-password-validation');
        expect(alert).not.toBeNull();
        expect(alert.innerText).toEqual('Password is required.');
      });
    })
  ));

  it('should emit an event when the login button is clicked',
    async(inject([], () => {
      fixture.whenStable().then(() => {
        fixture.componentInstance.username.setValue('user');
        fixture.componentInstance.password.setValue('pass');
        fixture.autoDetectChanges();
        fixture.componentInstance.onSubmit.subscribe(data => {
          expect(data).toBeDefined();
          expect(data.username).toEqual('user');
          expect(data.password).toEqual('pass');
        });
        let button = fixture.nativeElement.querySelector('#qa-login-button');
        button.click();
      });
    }))
  );
});
