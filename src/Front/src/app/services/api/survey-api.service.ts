import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { ICreateSurveyState } from './../../common/Interfaces/ICreateSurveyState';

@Injectable({
  providedIn: 'root',
})
export class SurveyApiService {
  private _http = inject(HttpClient);
  surveyApi = environment.SURVEY_API_URL;

  uploadSurveyPhoto(body: FormData) {
    this._http.post(`${this.surveyApi}/`, body);
  }

  createSurvey(body: ICreateSurveyState) {
    this._http.post(`${this.surveyApi}/`, body);
  }
}
