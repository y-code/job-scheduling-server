import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, mergeMap, switchMap, tap } from 'rxjs';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  public forecasts: JobSchedule[] = [];

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.getJobs$().subscribe();
  }

  createNew() {
    this.http.put(this.baseUrl + 'api/JobSchedule', {
      start: new Date(new Date().getTime() + 10 * 1000).toISOString(),
      end: new Date(new Date().getTime() + 20 * 1000).toISOString(),
      description: 'This is test job',
    }).pipe(
      catchError((error, caught) => {
        console.error(error);
        throw error;
      }),
      switchMap(() => this.getJobs$()),
    ).subscribe();
  }

  private getJobs$() {
    return this.http.get<JobSchedule[]>(this.baseUrl + 'api/JobSchedule').pipe(
      tap(result => {
        this.forecasts = result;
      }),
      catchError((error, caught) => {
        console.error(error);
        throw error;
      }),
    );
  }
}

interface JobSchedule {
  start: string;
  end: string;
  description: number;
}
