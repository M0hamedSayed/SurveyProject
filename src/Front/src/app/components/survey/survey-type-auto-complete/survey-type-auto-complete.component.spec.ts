import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SurveyTypeAutoCompleteComponent } from './survey-type-auto-complete.component';

describe('SurveyTypeAutoCompleteComponent', () => {
  let component: SurveyTypeAutoCompleteComponent;
  let fixture: ComponentFixture<SurveyTypeAutoCompleteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SurveyTypeAutoCompleteComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SurveyTypeAutoCompleteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
