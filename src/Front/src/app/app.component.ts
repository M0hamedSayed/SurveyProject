import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CreateSurveyComponent } from './pages/create-survey/create-survey.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CreateSurveyComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {}
