import { Injectable } from '@angular/core';
import { Http, ResponseContentType, RequestOptions, Request, RequestOptionsArgs, Response, Headers } from '@angular/http';
import * as crypto from 'crypto';

@Injectable()
export class CStructureImageService {
  private static HASH_KEY = 'reg';
  private static QUEUE_SIZE: number = 20;
  private static queue: any[] = [];

  private modals: any[] = [];

  constructor(private http: Http) {
  }

  generateImage(value: string): Promise<string> {
    let hash = crypto.createHmac('sha256', CStructureImageService.HASH_KEY).update(value).digest('hex');
    let queue = CStructureImageService.queue;
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
        let imageData = 'data:image/png;base64,' + btoa(String.fromCharCode.apply(null, new Uint8Array(result.arrayBuffer())));
        queue.push({ hash, imageData });
        if (queue.length > CStructureImageService.QUEUE_SIZE) {
          queue.shift();
        }
        return imageData;
      });
  }
}
