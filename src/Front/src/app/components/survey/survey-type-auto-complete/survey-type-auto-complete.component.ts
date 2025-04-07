import { Component, inject, signal } from '@angular/core';
import { SurveyApiService } from '../../../services/api/survey-api.service';
import { DropdownModule } from 'primeng/dropdown';

@Component({
  selector: 'app-survey-type-auto-complete',
  standalone: true,
  imports: [DropdownModule],
  templateUrl: './survey-type-auto-complete.component.html',
  styleUrl: './survey-type-auto-complete.component.css',
})
export class SurveyTypeAutoCompleteComponent {
  private _surveyApi = inject(SurveyApiService);

  types: any[] = [];
  loading = false;
  page = 0;
  pageSize = 20;
  totalRecords = 0;
  currentQuery = '';

  loadItems(event: any) {
    this.page = 0;
    this.currentQuery = event.query;
    this.types = [];
    this.fetchData();
  }

  onScroll(event: any) {
    const totalLoaded = (this.page + 1) * this.pageSize;
    if (totalLoaded < this.totalRecords) {
      this.page++;
      this.fetchData(true);
    }
  }

  fetchData(append = false) {
    this.loading = true;
  }
}
