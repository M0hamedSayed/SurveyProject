import { ICreateQuestionSurvey, ICreateSurvey } from './ICreateSurveyState';

export interface ISurvey extends ICreateSurvey {
  id: string;
  surveyTypeNameEn: string;
  surveyTypeNameAr: string;
  isActive: boolean;
}

export interface IQuestionSurvey extends ICreateQuestionSurvey {
  id: string;
}
