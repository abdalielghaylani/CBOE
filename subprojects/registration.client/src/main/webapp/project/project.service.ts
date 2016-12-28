import {Injectable} from '@angular/core';
import {Http, Response} from '@angular/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import {Observable} from 'rxjs/Observable';
import {Project} from './project.model';

@Injectable()
export class ProjectService {
    private baseURI: string;
    private currentProject: Project;

    constructor(private http: Http) {
        let uri: string = '/api/projects';
        this.baseURI = uri;
        this.currentProject = new Project('', '', '', '', false, false);
    }

    public getProjects(): Observable<Project[]> {
        let observable: Observable<Project[]> =
            this.http.get(this.baseURI)
                .map((response: Response) => response.json()._embedded['projects'])
                .catch(this.handleError);
        return observable;
    }

    public getProjectById(id: string): Observable<Project> {
        let observable: Observable<Project> =
            this.http.get(this.baseURI + '/' + id)
                .map((response: Response) => response.json())
                .catch(this.handleError);
        return observable;
    }

    public deleteProjectById(id: string): Observable<number> {
        let observable: Observable<number> =
            this.http.delete(this.baseURI + '/' + id)
                .map((response: Response) => console.log('project service: deleted project ' + id + ', HTTP response status: ' + response.status))
                .catch(this.handleError);
        return observable;
    }

    public createProject(project: Project): Observable<Project> {
        let observable: Observable<Project> =
            this.http.post(this.baseURI, project)
                .map((response: Response) => {
                    console.log('project service: created project ' + response.json().id + ', HTTP response status: ' + response.status)
                    console.log('project response location: ' + response.headers.get('Location'));
                    this.currentProject = response.json();
                    return this.currentProject;
                })
                .catch(this.handleError);
        return observable;
    }

    public getCurrentProject(): Project {
        return this.currentProject;
    }

    private handleError(error: any, observable: Observable<any>) {
        let errMsg = 'ProjectService: problems with http server';
        console.error(errMsg); // log to console instead
        return Observable.throw(errMsg);
    }
}
