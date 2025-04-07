import {
  ICreateChoiceSurvey,
  ICreateEvaluateChoiceSurvey,
} from './ICreateSurveyState';

export interface IChoiceSurvey extends ICreateChoiceSurvey {
  id: string;
}

export interface IEvaluateChoiceSurvey extends ICreateEvaluateChoiceSurvey {
  id: string;
}
