import { Injectable } from '@angular/core';
import { GlobalState } from '../../common/classes/globalState';
import { ICreateSurveyState } from '../../common/Interfaces/ICreateSurveyState';

@Injectable({
  providedIn: 'root',
})
export class SurveyCreateFormService extends GlobalState<ICreateSurveyState> {
  constructor() {
    super();
  }
}
