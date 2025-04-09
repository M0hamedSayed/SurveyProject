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
    return this._http.post(`${this.surveyApi}/Survey/add`, body);
  }

  createSurvey(body: ICreateSurveyState) {
    return this._http.post(`${this.surveyApi}/Survey/add-survey-photo`, body);
  }
}
