import { Injectable } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { IAppState } from '../redux';
import { notifyException } from '../common';

export const cdjsScriptName = 'cdjs';

declare var document: any;

@Injectable()
export class CdjsService {

  private scripts: any = {};

  constructor(private ngRedux: NgRedux<IAppState>) {
    let lookups = this.ngRedux.getState().session.lookups;
    this.scripts[cdjsScriptName] = {
      loaded: false,
      src: lookups.systemInformation.CDJSUrl + '/js/chemdrawweb/chemdrawweb.js'
    };
  }

  load(scripts: string[]) {
      let promises: any[] = [];
      scripts.forEach((script) => promises.push(this.loadScript(script)));
      return Promise.all(promises);
  }

  loadCdjsScript() {
      this.loadScript(cdjsScriptName)
        .catch(error => {
            notifyException(`CDJS script could not be loaded`, error, 5000);
          });
  }

  loadScript(name: string) {
      return new Promise((resolve, reject) => {
          if (this.scripts[name].loaded) {
              resolve({script: name, loaded: true, status: 'Already Loaded'});
          } else {
              let script = document.createElement('script');
              script.type = 'text/javascript';
              script.src = this.scripts[name].src;
              if (script.readyState) {
                  script.onreadystatechange = () => {
                      if (script.readyState === 'loaded' || script.readyState === 'complete') {
                          script.onreadystatechange = null;
                          this.scripts[name].loaded = true;
                          resolve({script: name, loaded: true, status: 'Loaded'});
                      }
                  };
              } else {
                  script.onload = () => {
                      this.scripts[name].loaded = true;
                      resolve({script: name, loaded: true, status: 'Loaded'});
                  };
              }
              script.onerror = (error: any) => reject({script: name, loaded: false, status: 'Error'});
              document.getElementsByTagName('head')[0].appendChild(script);
          }
      });
  }
}
