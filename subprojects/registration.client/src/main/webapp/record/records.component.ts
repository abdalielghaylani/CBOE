import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Record } from './record.model';
import { RecordService } from './record.service';


@Component({
    selector: 'records',
    templateUrl: 'records.component.html'
})
export class RecordsComponent implements OnInit {
    private records: Record[];

    constructor(private recordsService: RecordService, private router: Router) {
    }

    ngOnInit() {
        this.recordsService.getRecords()
            .subscribe((records: Record[]) => this.records = records,
            error => console.error('RecordsComponent: cannot get records from RecordService'));
    }
}
