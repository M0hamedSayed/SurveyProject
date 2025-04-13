import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SurveyApiService } from '../../services/api/survey-api.service';
import { ISurvey } from '../../common/Interfaces/ISurvey';
import { environment } from '../../../environments/environment';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-survey',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './survey.component.html',
  styleUrl: './survey.component.css',
})
export class SurveyComponent implements OnInit {
  private _route = inject(ActivatedRoute);
  private _api = inject(SurveyApiService);
  apiUrl = environment.Static_API_URL;
  survey!: ISurvey;
  loading = signal(true);

  ngOnInit(): void {
    this.getSurveyData();
  }

  getSurveyData() {
    const id = this._route.snapshot.paramMap.get('id');
    if (!id) return;
    this.loading.set(true);
    this._api.getSurveyById(id).subscribe({
      next: (value: any) => {
        console.log(value);
        this.survey = value.Data;
        this.loading.set(false);
      },
      error: (err: any) => {
        console.log(err);
        this.loading.set(false);
      },
    });
  }
}

// remember handle authorize to create survey
