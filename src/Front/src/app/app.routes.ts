import { Routes } from '@angular/router';
import { SurveysListComponent } from './pages/surveys-list/surveys-list.component';
import { CreateSurveyComponent } from './pages/create-survey/create-survey.component';
import { NotFoundComponent } from './pages/not-found/not-found.component';
import { SurveyComponent } from './pages/survey/survey.component';
import { adminGuard } from './guards/admin.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'survey', pathMatch: 'full' },
  { path: 'survey', component: SurveysListComponent },
  {
    path: 'survey/create',
    component: CreateSurveyComponent,
    canActivate: [adminGuard],
  },
  { path: 'survey/:id', component: SurveyComponent },
  { path: '**', component: NotFoundComponent },
];
