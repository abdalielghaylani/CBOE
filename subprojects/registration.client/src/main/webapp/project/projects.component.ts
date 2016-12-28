import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Project } from './project.model';
import { ProjectService } from './project.service';


@Component({
    selector: 'projects',
    templateUrl: 'projects.component.html'
})
export class ProjectsComponent implements OnInit {
    private projects: Project[];

    constructor(private projectsService: ProjectService, private router: Router) {
    }

    ngOnInit() {
        this.projectsService.getProjects()
            .subscribe((projects: Project[]) => this.projects = projects,
            error => console.error('ProjectsComponent: cannot get projects from ProjectService'));
    }

    contentReady(e: any) {
        var grid = e.component;
        if (grid.columnCount() > 0 && grid.columnOption('_links', 'visible'))
            grid.columnOption('_links', 'visible', false);
    }
}
