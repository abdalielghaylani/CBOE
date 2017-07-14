import { Injectable } from '@angular/core';
import { CanActivate, CanActivateChild } from '@angular/router';
import { Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { ILookupData, SessionActions, IAppState } from '../redux';
import { NgRedux, select } from '@angular-redux/store';
import { Observable } from 'rxjs/Observable';
import { HttpService } from '../services';

@Injectable()
export class AuthGuard implements CanActivate, CanActivateChild {

  @select(s => s.session.lookups) lookups$: Observable<ILookupData>;
  private lookupsSubscription: Subscription;
  private lookups: ILookupData;
  private ngRedux: NgRedux<IAppState>;

  constructor(private router: Router, ngRedux: NgRedux<IAppState>) {
    this.lookupsSubscription = this.lookups$.subscribe(d => { if (d) { this.retrieveContents(d); } });
    this.ngRedux = ngRedux;
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    let privilege = route.data.privilege;
    return this.getPrivilege(privilege);
  }

  canActivateChild() {
    return true;
  }

  getPrivilege(privilage: string) {
    if (this.lookups) {
      let privilages = this.lookups.homeMenuPrivileges;
      let privilageItem = privilages.find(p => p.privilegeName === privilage);
      if (privilageItem === undefined) {
        return false;
      }

      if (privilageItem.visibility) {
        return true;
      } else {
        // redirect to login page, if no previlege to access route
        this.ngRedux.dispatch(SessionActions.logoutUserAction());
        return false;
      }
    }
    return false;
  }

  // Trigger data retrieval for the view to show.
  retrieveContents(lookups: ILookupData) {
    this.lookups = lookups;
  }
}
