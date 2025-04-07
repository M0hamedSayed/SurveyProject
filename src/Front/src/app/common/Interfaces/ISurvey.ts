import { ICreateQuestionSurvey, ICreateSurvey } from './ICreateSurveyState';

export interface ISurvey extends ICreateSurvey {
  id: string;
}

export interface IQuestionSurvey extends ICreateQuestionSurvey {
  id: string;
}
