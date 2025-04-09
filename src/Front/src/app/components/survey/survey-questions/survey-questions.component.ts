import {
  ChangeDetectorRef,
  Component,
  inject,
  input,
  OnInit,
  output,
} from '@angular/core';
import { OneQuestionComponent } from '../one-question/one-question.component';
import {
  AbstractControl,
  FormArray,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import {
  ICreateQuestionSurvey,
  ICreateSurveyState,
} from '../../../common/Interfaces/ICreateSurveyState';
import { SurveyCreateFormService } from '../../../services/logic/survey-create-form.service';
import { SurveyApiService } from '../../../services/api/survey-api.service';
import { switchMap } from 'rxjs';

@Component({
  selector: 'app-survey-questions',
  standalone: true,
  imports: [OneQuestionComponent, ReactiveFormsModule],
  templateUrl: './survey-questions.component.html',
  styleUrl: './survey-questions.component.css',
})
export class SurveyQuestionsComponent implements OnInit {
  private _surveyCreationState = inject(SurveyCreateFormService);
  private _surveyAPi = inject(SurveyApiService);
  private cdr = inject(ChangeDetectorRef);

  redirectToPreviousPage = output<void>();

  ngOnInit(): void {
    const questions = this._surveyCreationState.state().questions;
    console.log(questions);

    if (questions?.length)
      questions.forEach((q) => {
        this.questions.push(new FormControl(q, Validators.required));
      });
    else this.addQuestion();
    this.cdr.detectChanges();
  }

  form: FormGroup = new FormGroup(
    {
      questions: new FormArray<FormControl<ICreateQuestionSurvey>>([]),
    },
    { validators: this.questionValidator }
  );

  get questions(): FormArray {
    return this.form.get('questions') as FormArray<
      FormControl<ICreateQuestionSurvey>
    >;
  }

  private questionValidator(control: AbstractControl): ValidationErrors | null {
    if (!control.get('questions')?.value?.length)
      return { questionRequired: true };
    return null;
  }

  addQuestion() {
    const newQuestion = {
      questionEn: '',
      questionAr: '',
      questionType: '',
      choices: [],
      evaluateChoices: [],
    };
    this.questions.push(new FormControl(newQuestion, Validators.required));
  }

  removeQuestion(index: number) {
    this.questions.removeAt(index);
  }

  submit() {
    this.form.markAllAsTouched();
    if (this.form.invalid) return;

    this._surveyCreationState.setState(this.form.value);
    console.log('Submitted Questions:', this.form);
    // call api
    this.handleSaveSurvey();
    console.log(this._surveyCreationState.state());
  }

  handleSaveSurvey() {
    const image = this._surveyCreationState.state().image;
    if (image) {
      const formData = new FormData();
      formData.append('image', image);
      this._surveyAPi
        .uploadSurveyPhoto(formData)
        .pipe(
          switchMap((res: any) => {
            const finalSurveyData: ICreateSurveyState = {
              ...this._surveyCreationState.state(),
              imageUrl: res?.Data || '',
            };
            delete finalSurveyData.image;

            return this._surveyAPi.createSurvey(finalSurveyData);
          })
        )
        .subscribe({
          next: (result) => {
            console.log('Survey created successfully', result);
          },
          error: (err) => {
            console.error('Error occurred', err);
          },
        });
    } else {
      const finalSurveyData: ICreateSurveyState = {
        ...this._surveyCreationState.state(),
        imageUrl: '',
      };
      delete finalSurveyData.image;
      this._surveyAPi.createSurvey(finalSurveyData).subscribe({
        next: (result) => {
          console.log('Survey created successfully', result);
        },
        error: (err) => {
          console.error('Error occurred', err);
        },
      });
    }
  }

  handleDeleteQuestion(index: number) {
    this.questions.removeAt(index);
  }

  onPreviousPage() {
    this._surveyCreationState.setState(this.form.value);
    this.redirectToPreviousPage.emit();
  }
}
