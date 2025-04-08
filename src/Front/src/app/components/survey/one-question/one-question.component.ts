import {
  Component,
  CUSTOM_ELEMENTS_SCHEMA,
  forwardRef,
  Input,
  input,
  OnInit,
  output,
} from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormArray,
  FormControl,
  FormGroup,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ReactiveFormsModule,
  ValidationErrors,
  Validator,
  Validators,
} from '@angular/forms';
import { QuestionType } from '../../../common/enums/questionType';
import { InputTextModule } from 'primeng/inputtext';
import 'emoji-picker-element';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-one-question',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    InputTextModule,
    OverlayPanelModule,
    ButtonModule,
  ],
  templateUrl: './one-question.component.html',
  styleUrl: './one-question.component.css',
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OneQuestionComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      useExisting: forwardRef(() => OneQuestionComponent),
      multi: true,
    },
  ],
})
export class OneQuestionComponent
  implements OnInit, ControlValueAccessor, Validator
{
  questionNo = input<number>(1);
  onDelete = output<boolean>();

  ngOnInit(): void {
    this.questionForm
      .get('questionType')!
      .valueChanges.subscribe((value: any) => {
        this.questionForm.get('choices')!.updateValueAndValidity();
        this.questionForm.get('evaluateChoices')!.updateValueAndValidity();
        this.EvaluateChoices.clear();
        this.choices.clear();
        if (
          value == QuestionType.oneChoice ||
          value == QuestionType.multiChoice
        ) {
          this.addChoice(false);
          this.addChoice(false);
        } else if (value == QuestionType.evaluate) {
          this.addChoice(true);
          this.addChoice(true);
        }
      });
  }
  //#region Form with control value accessor
  questionForm: FormGroup = new FormGroup(
    {
      questionEn: new FormControl('', { validators: Validators.required }),
      questionAr: new FormControl('', { validators: Validators.required }),
      questionType: new FormControl('', {
        validators: Validators.required,
      }),
      choices: new FormArray([]),
      evaluateChoices: new FormArray([]),
    },
    {
      validators: this.choicesValidator(),
    }
  );
  // ControlValueAccessor implementation
  private onChange = (value: any) => {};
  private onTouched = () => {};

  writeValue(obj: any): void {
    if (obj) {
      this.questionForm.patchValue(obj);
    }
  }

  registerOnChange(fn: any): void {
    this.questionForm.valueChanges.subscribe(fn);
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    isDisabled ? this.questionForm.disable() : this.questionForm.enable();
  }

  // Custom validator
  validate(control: AbstractControl): ValidationErrors | null {
    return this.questionForm.valid ? null : this.questionForm.errors;
  }

  private choicesValidator(): (
    control: AbstractControl
  ) => ValidationErrors | null {
    return (group: AbstractControl): ValidationErrors | null => {
      const choices = group.get('choices')?.value;
      const evaluateChoices = group.get('evaluateChoices')?.value;
      const type = group.get('questionType')?.value;

      if (type == QuestionType.sample) {
        return choices.length === 0 && evaluateChoices.length === 0
          ? null
          : { sampleShouldNotHaveChoices: true };
      }

      if (type == QuestionType.oneChoice || type == QuestionType.multiChoice) {
        const hasTwoValid = choices?.length >= 2;
        return hasTwoValid ? null : { minValidChoicesRequired: true };
      }

      if (type == QuestionType.evaluate) {
        return evaluateChoices?.length >= 2
          ? null
          : { invalidEvaluateChoices: true };
      }

      return null;
    };
  }

  get f() {
    return this.questionForm;
  }
  get choices(): FormArray {
    return this.questionForm.get('choices') as FormArray;
  }
  get EvaluateChoices(): FormArray {
    return this.questionForm.get('evaluateChoices') as FormArray;
  }
  //#endregion
  //#region handle choices
  createChoice(isEvaluate: boolean = false): FormGroup {
    const controls: {
      textAr: FormControl;
      textEn: FormControl;
      emotion?: FormControl;
    } = {
      textEn: new FormControl('', { validators: Validators.required }),
      textAr: new FormControl('', { validators: Validators.required }),
    };
    if (isEvaluate)
      controls.emotion = new FormControl('', {
        validators: Validators.required,
      });
    return new FormGroup({
      ...controls,
    });
  }

  addChoice(isEvaluate: boolean = false) {
    isEvaluate
      ? this.EvaluateChoices.push(this.createChoice(true))
      : this.choices.push(this.createChoice());
  }

  removeChoice(index: number, isEvaluate: boolean = false) {
    isEvaluate
      ? this.EvaluateChoices.removeAt(index)
      : this.choices.removeAt(index);
    this.questionForm.get('choices')!.updateValueAndValidity();
    this.questionForm.get('evaluateChoices')!.updateValueAndValidity();
  }

  onEmojiClick(event: any, index: number) {
    const emoji = event.detail.unicode;
    console.log('Selected Emoji Unicode:', emoji);
    this.EvaluateChoices.controls[index].get('emotion')?.patchValue(emoji);
  }
  //#endregion
}
