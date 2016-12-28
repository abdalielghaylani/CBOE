import {Injectable} from '@angular/core';
import {Http, Response} from '@angular/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import {Observable} from 'rxjs/Observable';
import {Record} from './record.model';

@Injectable()
export class RecordService {
    private baseURI: string;
    private currentRecord: Record;

    constructor(private http: Http) {
        let uri: string = '/api/records';
        this.baseURI = uri;
        this.currentRecord = new Record('', '', '');
    }

    public getRecords(): Observable<Record[]> {
        let observable: Observable<Record[]> =
            this.http.get(this.baseURI)
                .map((response: Response) => response.json()._embedded['records'])
                .catch(this.handleError);
        return observable;
    }

    public getRecordById(id: string): Observable<Record> {
        let observable: Observable<Record> =
            this.http.get(this.baseURI + '/' + id)
                .map((response: Response) => response.json())
                .catch(this.handleError);
        return observable;
    }

    public deleteRecordById(id: string): Observable<number> {
        let observable: Observable<number> =
            this.http.delete(this.baseURI + '/' + id)
                .map((response: Response) => console.log('record service: deleted record ' + id + ', HTTP response status: ' + response.status))
                .catch(this.handleError);
        return observable;
    }

    public createRecord(record: Record): Observable<Record> {
        let observable: Observable<Record> =
            this.http.post(this.baseURI, record)
                .map((response: Response) => {
                    console.log('record service: created record ' + response.json().id + ', HTTP response status: ' + response.status);
                    console.log('record response location: ' + response.headers.get('Location'));
                    this.currentRecord = response.json();
                    return this.currentRecord;
                })
                .catch(this.handleError);
        return observable;
    }

    public getCurrentRecord(): Record {
        return this.currentRecord;
    }

    private handleError(error: any, observable: Observable<any>) {
        let errMsg = 'RecordService: problems with http server';
        console.error(errMsg); // log to console instead
        return Observable.throw(errMsg);
    }
}
