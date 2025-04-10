import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-survey',
  standalone: true,
  imports: [],
  templateUrl: './survey.component.html',
  styleUrl: './survey.component.css',
})
export class SurveyComponent {
  private _route = inject(ActivatedRoute);
  loading = signal(false);
  getSurveyData() {
    const id = this._route.snapshot.paramMap.get('id'); // '123'
  }
}
