import { QuestionType } from '../enums/questionType';

export interface ICreateSurveyState extends ICreateSurvey {
  image?: File;
}

export interface ICreateSurvey {
  surveyTypeId: string;
  nameEn: string;
  nameAr: string;
  startDate: Date;
  endDate: Date;
  descriptionEn: string;
  descriptionAr: string;
  closingAddressEn: string;
  closingAddressAr: string;
  closingStatementEn: string;
  closingStatementAr: string;
  imageUrl: string;
  isRequired: boolean;
  questions: ICreateQuestionSurvey[];
}

export interface ICreateQuestionSurvey {
  questionEn: string;
  questionAr: string;
  questionType: QuestionType;
  choices: ICreateChoiceSurvey[];
  evaluateChoices: ICreateEvaluateChoiceSurvey[];
}

export interface ICreateChoiceSurvey {
  textAr: string;
  textEn: string;
}

export interface ICreateEvaluateChoiceSurvey extends ICreateChoiceSurvey {
  emotion: string;
}
