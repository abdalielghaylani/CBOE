import {
  async,
  inject,
  TestBed,
} from '@angular/core/testing';
import {RegAlert} from './alert.component';
import {RegUiModule} from '../../components/ui/ui.module';
import {configureTests} from '../../tests.configure';

describe('Component: Alert', () => {
  let fixture;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [RegUiModule],
      });
    };

    configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(RegAlert);
      fixture.detectChanges();
      done();
    });
  });

  it('should default to info status', async(inject([], () => {
    fixture.whenStable().then(() => {
      expect(fixture.componentInstance.status).toBe('info');
    });
  })));

  it('should have the correct background class',
    async(inject([], () => {
      fixture.whenStable().then(() => {
        fixture.componentInstance.qaid = 'alert-1';
        const compiled = fixture.debugElement.nativeElement;
        const allBgClasses = ['bg-blue', 'bg-yellow', 'bg-green', 'bg-red'];
        const status_class = [
          {
            status: 'info',
            class: 'alert-info',
          },
          {
            status: 'warning',
            class: 'alert-warning',
          },
          {
            status: 'success',
            class: 'alert-success',
          },
          {
            status: 'error',
            class: 'alert-danger',
          },
        ];

        status_class.map(item => {
          fixture.componentInstance.status = item.status;
          fixture.detectChanges();
          expect(compiled.querySelector('#alert-1')
            .getAttribute('class').split(' ')).toContain(item.class);
          allBgClasses.filter(bg_class => bg_class !== item.class)
            .map(bg_class_excluded => {
              expect(compiled.querySelector('#alert-1')
                .getAttribute('class').split(' '))
                .not.toContain(bg_class_excluded);
            });
        });
      });
    })
  ));

  it('should have class white if status is info or error',
    async(inject([], () => {
      fixture.whenStable().then(() => {
        fixture.componentInstance.qaid = 'alert-1';
        const compiled = fixture.debugElement.nativeElement;
        const allStatuses = ['info', 'warning', 'success', 'error'];
        const whiteTextStatuses = ['info', 'error'];
        allStatuses.map(status => {
          fixture.componentInstance.status = status;
          fixture.detectChanges();
          if (whiteTextStatuses.indexOf(status) >= 0) {
            expect(compiled.querySelector('#alert-1')
              .getAttribute('class').split(' ')).toContain('white');
          } else {
            expect(compiled.querySelector('#alert-1')
              .getAttribute('class').split(' '))
              .not.toContain('white');
          }
        });
      });
    })));
});

