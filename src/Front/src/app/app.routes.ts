import { Routes } from '@angular/router';
import { SurveysListComponent } from './pages/surveys-list/surveys-list.component';
import { CreateSurveyComponent } from './pages/create-survey/create-survey.component';
import { NotFoundComponent } from './pages/not-found/not-found.component';

export const routes: Routes = [
  { path: '', redirectTo: 'survey', pathMatch: 'full' },
  { path: 'survey', component: SurveysListComponent },
  { path: 'survey/:id', component: SurveysListComponent },
  { path: 'survey/create', component: CreateSurveyComponent },
  { path: '**', component: NotFoundComponent },
];
