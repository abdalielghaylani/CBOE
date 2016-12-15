import {Component, OnInit} from '@angular/core';
import {Project} from './project.model';
import {ProjectService} from './project.service';


@Component({
    selector: 'projects',
    templateUrl: 'projects.component.html'
})
export class ProjectsComponent implements OnInit{
    private projects: Project[];

    constructor(private projectsService: ProjectService) {
    }

    ngOnInit() {
        this.projectsService.getProjects()
            .subscribe((projects: Project[]) => this.projects = projects,
                error => console.error('ProjectsComponent: cannot get projects from ProjectService'));
    }
}
