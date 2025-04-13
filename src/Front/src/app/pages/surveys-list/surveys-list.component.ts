import { Component, inject, OnInit, signal } from '@angular/core';
import { InputTextModule } from 'primeng/inputtext';
import { CalendarModule } from 'primeng/calendar';
import { InputSwitchModule } from 'primeng/inputswitch';
import { ButtonModule } from 'primeng/button';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { SelectButtonModule } from 'primeng/selectbutton';
import { SurveyTypeAutoCompleteComponent } from '../../components/survey/survey-type-auto-complete/survey-type-auto-complete.component';
import { DataViewModule } from 'primeng/dataview';
import { SurveyApiService } from '../../services/api/survey-api.service';
import { ISurvey } from '../../common/Interfaces/ISurvey';
import { CommonModule } from '@angular/common';
import { environment } from '../../../environments/environment';
import { RouterLink } from '@angular/router';
import { UserService } from '../../services/logic/user.service';

@Component({
  selector: 'app-surveys-list',
  standalone: true,
  imports: [
    InputTextModule,
    InputTextModule,
    InputSwitchModule,
    CalendarModule,
    ButtonModule,
    IconFieldModule,
    InputIconModule,
    ReactiveFormsModule,
    SelectButtonModule,
    SurveyTypeAutoCompleteComponent,
    DataViewModule,
    CommonModule,
    RouterLink,
  ],
  templateUrl: './surveys-list.component.html',
  styleUrl: './surveys-list.component.css',
})
export class SurveysListComponent implements OnInit {
  private _apiService = inject(SurveyApiService);
  user = inject(UserService);
  isActiveOptions = [
    { name: '✔️', value: true },
    { name: '❌', value: false },
  ];
  page = 1;
  limit = 10;
  hasNext = false;
  hasPrevious = false;
  totalRecords = 0;
  allSurveys: ISurvey[] = [];
  loading = signal(false);

  searchForm: FormGroup = new FormGroup({
    search: new FormControl(''),
    startDate: new FormControl(null),
    endDate: new FormControl(null),
    surveyTypeId: new FormControl(null),
    isActive: new FormControl(null),
  });

  api = environment.Static_API_URL;
  ngOnInit(): void {
    this.searchForm.valueChanges.subscribe((v) => console.log(v));
  }

  isAdmin() {
    return this.user.select('roles')().includes('Admin');
  }

  getControl(name: string) {
    return this.searchForm.controls[name] as FormControl;
  }

  getAllSurveyFromApi(resetPage: boolean = true) {
    if (resetPage) this.page = 1;

    const body = {
      ...this.searchForm.value,
      pageNumber: this.page,
      pageSize: this.limit,
    };
    body.surveyTypeId = body.surveyTypeId?.id || body.surveyTypeId;

    // loading
    this.loading.set(true);

    this._apiService.getAllSurvey(body).subscribe({
      next: (value: any) => {
        this.totalRecords = value.Data.totalCount;
        this.hasNext = value.Data.hasNextPage;
        this.hasPrevious = value.Data.hasPreviousPage;
        this.allSurveys = value.Data?.list || [];

        this.loading.set(false);
      },
      error: (error) => {
        console.log(error);
        this.loading.set(false);
      },
    });
  }

  loadMoreData(event: any) {
    console.log(event);
    const page =
      Math.floor((event.first ?? 0) / (event.rows ?? this.limit)) + 1;
    if (this.page != page) this.page = page;
    this.getAllSurveyFromApi(false);
  }

  onSearch() {
    console.log(this.searchForm.value);
    this.getAllSurveyFromApi();
  }

  onReload() {
    this.searchForm.reset();
    this.getAllSurveyFromApi();
  }
}
