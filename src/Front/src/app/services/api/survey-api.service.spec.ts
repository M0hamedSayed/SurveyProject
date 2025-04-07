import { TestBed } from '@angular/core/testing';

import { SurveyApiService } from './survey-api.service';

describe('SurveyApiService', () => {
  let service: SurveyApiService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SurveyApiService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
