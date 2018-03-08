import { TestBed, async, inject } from '@angular/core/testing';
import { Component } from '@angular/core';
import { TestModule } from '../../test/test.module';
import { CommandButton } from './command-button.component';
import { By } from '@angular/platform-browser';

describe('Component : Command Button', () => {

  let fixture;
  let queryByDir;
  let queryByCss;

  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [ TestModule ],
        declarations : [ CommandButton, CommandButtonTestController ],
      });
    };

    TestModule.configureTests(configure).then(testBed => {
      fixture = testBed.createComponent(CommandButtonTestController);
      queryByDir = fixture.debugElement.query(By.directive(CommandButton));
      fixture.detectChanges();
      done();
    });
  });

  it('should create Command Button Component', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(queryByDir).toBeTruthy();
      expect(queryByDir.componentInstance).toBeTruthy();
    });
  })));

  it('should set test id as per value passed', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(queryByDir.componentInstance.testid).toContain('command-button-test');
    });
  })));

  it('should display appropriate icon class', async(inject([], () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      queryByCss = fixture.debugElement.query(By.css('i'));
      expect(queryByCss.nativeElement.className).toContain('fa-test-icon');
    });
  })));

  it('should display assigned title', () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      queryByCss = fixture.debugElement.query(By.css('button:last-child'));
      expect(queryByCss.nativeElement.innerText).toEqual('Test Title');
    });
  });

  it('should default to xs size and verify if appropriate class applied', () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(queryByDir.componentInstance.size).toEqual('xs');
      queryByCss = fixture.debugElement.query(By.css('div'));
      expect(queryByCss.nativeElement.className).toContain('btn-group-xs');
    });
  });

  it('should default to color blue and verify if appropriate class applied', () => {
    fixture.whenStable().then(() => {
      fixture.autoDetectChanges();
      expect(queryByDir.componentInstance.color).toEqual('blue');
      queryByCss = fixture.debugElement.query(By.css('button:first-child'));
      expect(queryByCss.nativeElement.className).toContain('background-blue border-blue');
      queryByCss = fixture.debugElement.query(By.css('button:last-child'));
      expect(queryByCss.nativeElement.className).toContain('border-blue blue');
    });
  });

});

@Component({
  selector: 'test',
  template: `
    <command-button testid="command-button-test" (onClick)="handleClick($event)" 
    iconName="test-icon" title="Test Title"></command-button>
  `
})
class CommandButtonTestController { }
