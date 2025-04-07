import { TestBed } from '@angular/core/testing';

import { SurveyCreateFormService } from './survey-create-form.service';

describe('SurveyCreateFormService', () => {
  let service: SurveyCreateFormService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SurveyCreateFormService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
