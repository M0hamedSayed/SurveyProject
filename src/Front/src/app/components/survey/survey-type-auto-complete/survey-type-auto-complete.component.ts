import {
  Component,
  inject,
  input,
  model,
  OnInit,
  signal,
  viewChild,
} from '@angular/core';
import { SurveyApiService } from '../../../services/api/survey-api.service';
import { MultiSelect, MultiSelectModule } from 'primeng/multiselect';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { ISurvey } from '../../../common/Interfaces/ISurvey';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { DropdownModule } from 'primeng/dropdown';

@Component({
  selector: 'app-survey-type-auto-complete',
  standalone: true,
  imports: [DropdownModule, ReactiveFormsModule],
  templateUrl: './survey-type-auto-complete.component.html',
  styleUrl: './survey-type-auto-complete.component.css',
})
export class SurveyTypeAutoCompleteComponent implements OnInit {
  private _surveyApi = inject(SurveyApiService);

  // inputs
  control = input.required<FormControl>();
  placeHolder = input('Select Survey Type');
  loading = model<boolean>(false);
  multiSelect = viewChild(MultiSelect);
  allData: ISurvey[] = [];
  page = 1;
  limit = 10;
  hasMore = false;
  totalRecords = 0;
  search: string = '';

  ngOnInit(): void {
    this.getDataFromAPI();
  }
  getDataFromAPI(isNew: boolean = true, isSearching: boolean = false) {
    if (this.loading()) return;
    // apply loading
    this.loading.set(true);
    this.page = isNew ? 1 : this.page;
    const body = {
      pageNumber: this.page,
      pageSize: this.limit,
      search: this.search,
    };
    const SurveyTypeApi = isSearching
      ? this._surveyApi
          .getAllSurveyTypes(body)
          .pipe(debounceTime(300), distinctUntilChanged())
      : this._surveyApi.getAllSurveyTypes(body);

    SurveyTypeApi.subscribe({
      next: (value: any) => {
        this.totalRecords = value.Data.totalCount;
        this.hasMore = value.Data.hasNextPage;
        if (isNew) {
          this.allData = value.Data?.list || [];
        } else {
          this.allData = [...this.allData, ...value.Data?.list];
        }
        this.loading.set(false);
      },
      error: (err) => {
        console.log(err);
        this.loading.set(false);
      },
    });
  }

  onSearch(event: any) {
    this.search =
      event?.filter?.replace(/\s+/g, ' ').trim().toLowerCase() ||
      event?.value?.replace(/\s+/g, ' ').trim().toLowerCase();

    this.getDataFromAPI(true, true);
  }

  loadExtraData() {
    if (this.hasMore) {
      this.page++;
      this.getDataFromAPI(false, false);
    }
  }
}
