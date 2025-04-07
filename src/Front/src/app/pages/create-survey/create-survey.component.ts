import { Component } from '@angular/core';
import { InitialSurveyCreationComponent } from '../../components/survey/initial-survey-creation/initial-survey-creation.component';

@Component({
  selector: 'app-create-survey',
  standalone: true,
  imports: [InitialSurveyCreationComponent],
  templateUrl: './create-survey.component.html',
  styleUrl: './create-survey.component.css',
})
export class CreateSurveyComponent {}
