import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InitialSurveyCreationComponent } from './initial-survey-creation.component';

describe('InitialSurveyCreationComponent', () => {
  let component: InitialSurveyCreationComponent;
  let fixture: ComponentFixture<InitialSurveyCreationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InitialSurveyCreationComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(InitialSurveyCreationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
