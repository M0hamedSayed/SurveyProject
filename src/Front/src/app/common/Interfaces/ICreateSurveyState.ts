import { QuestionType } from '../enums/questionType';

export interface ICreateSurveyState extends ICreateSurvey {
  image?: File;
}

export interface ICreateSurvey {
  surveyTypeId: string | ISurveyType;
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

export interface ISurveyType {
  id: string;
  nameEn: string;
  nameAr: string;
}

export interface ICreateQuestionSurvey {
  questionEn: string;
  questionAr: string;
  questionType: QuestionType;
  choices?: ICreateChoiceSurvey[];
  evaluateChoices?: ICreateEvaluateChoiceSurvey[];
}

export interface ICreateChoiceSurvey {
  textAr: string;
  textEn: string;
}

export interface ICreateEvaluateChoiceSurvey extends ICreateChoiceSurvey {
  emotion: string;
}
