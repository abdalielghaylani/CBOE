import { Injectable } from '@angular/core';
import { Http, ResponseContentType, RequestOptions, Request, RequestOptionsArgs, Response, Headers } from '@angular/http';
import * as crypto from 'crypto';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/forkJoin';
import { b64Encode } from '../../common';

@Injectable()
export class CStructureImagePrintService {
  private static HASH_KEY = 'reg';
  private static QUEUE_SIZE: number = 1000;
  private static queue: any[] = [];

  private modals: any[] = [];

  constructor(private http: Http) {
  }

  generateMultipleImages(values: Array<string>): Observable<Array<string>> {
    let singleObservables = values.map((value: string) => {
     return this.generateSingleImage(value);
  });

   return Observable.forkJoin(singleObservables);
  }

  getImage(value: string) {
    let hash = crypto.createHmac('sha256', CStructureImagePrintService.HASH_KEY).update(value).digest('hex');
    let queue = CStructureImagePrintService.queue;
    let found = queue.find(e => e.hash === hash);
    if (found) {
      return found.imageData;
    }
  }

  generateSingleImage(value: string): Promise<string> {
    let hash = crypto.createHmac('sha256', CStructureImagePrintService.HASH_KEY).update(value).digest('hex');
    let queue = CStructureImagePrintService.queue;
    let found = queue.find(e => e.hash === hash);
    if (found) {
      return Promise.resolve(found.imageData);
    }
    let data: any = {
      chemData: value,
      chemDataType: 'CDXML',
      resolution: '300',
      imageType: 'png'
    };
    let headers = new Headers({
      'Content-Type': 'application/json',
      Authorization: 'Basic VTlueFA3VzM/XlE4bVE6VFF6SzNlMyllNDNlQEE=',
      Accept: 'image/png'
    });
    let options = new RequestOptions({ headers: headers, responseType: ResponseContentType.ArrayBuffer });
    return this.http.post('https://chemdrawdirect.perkinelmer.cloud/rest/generateImage', data, options)
      .toPromise()
      .then(result => {
        let imageData = 'data:image/png;base64,' + b64Encode(new Uint8Array(result.arrayBuffer()));
        queue.push({ hash, imageData });
        if (queue.length > CStructureImagePrintService.QUEUE_SIZE) {
          queue.shift();
        }
        return imageData;
      });
  }
}
