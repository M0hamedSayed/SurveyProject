import { Component, OnInit } from '@angular/core';
import { OneQuestionComponent } from '../one-question/one-question.component';
import {
  FormArray,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';

interface Question {
  questionEn: string;
  questionAr: string;
  questionType: string;
  choices: string[];
  evaluateChoices: string[];
}
@Component({
  selector: 'app-survey-questions',
  standalone: true,
  imports: [OneQuestionComponent, ReactiveFormsModule],
  templateUrl: './survey-questions.component.html',
  styleUrl: './survey-questions.component.css',
})
export class SurveyQuestionsComponent implements OnInit {
  form: FormGroup = new FormGroup({
    questions: new FormArray<FormControl<Question>>([]),
  });
  get questions(): FormArray {
    return this.form.get('questions') as FormArray<FormControl<Question>>;
  }

  addQuestion() {
    const newQuestion: Question = {
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

    if (this.form.valid) {
      console.log('Submitted Questions:', this.form);
    } else {
      console.warn('Form invalid', this.form);
    }
  }

  ngOnInit(): void {
    this.addQuestion();
  }
}
