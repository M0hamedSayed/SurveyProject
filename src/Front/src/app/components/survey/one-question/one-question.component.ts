import {
  Component,
  CUSTOM_ELEMENTS_SCHEMA,
  forwardRef,
  inject,
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
import { SurveyCreateFormService } from '../../../services/logic/survey-create-form.service';
import { distinctUntilChanged, startWith } from 'rxjs';

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
  patching = true;

  ngOnInit(): void {
    this.questionForm
      .get('questionType')!
      .valueChanges.pipe(
        startWith(this.questionForm.get('questionType')!.value),
        distinctUntilChanged()
      )
      .subscribe((value: any) => {
        if (this.patching) return;
        else {
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
      choices: new FormArray([], this.validateChoiceText),
      evaluateChoices: new FormArray([], this.validateChoiceText),
    },
    {
      validators: this.choicesValidator(),
    }
  );
  // ControlValueAccessor implementation
  private onChange = (value: any) => {};
  private onTouched = () => {};

  writeValue(obj: any): void {
    this.patching = true;

    if (obj) {
      // Patch simple form fields
      this.questionForm.patchValue(
        {
          questionEn: obj?.questionEn || '',
          questionAr: obj?.questionAr || '',
          questionType: obj?.questionType || '',
        },
        { emitEvent: false }
      );

      // Rebuild 'choices' FormArray
      this.choices.clear();
      if (obj.choices && Array.isArray(obj.choices)) {
        obj.choices.forEach((choice: any) => {
          this.addChoice(false, choice);
        });
      }

      // Rebuild 'evaluateChoices' FormArray
      this.EvaluateChoices.clear();
      if (obj.evaluateChoices && Array.isArray(obj.evaluateChoices)) {
        obj.evaluateChoices.forEach((eChoice: any) => {
          this.addChoice(true, eChoice);
        });
      }
    }
    // manually trigger valueChanges once all values are set
    this.questionForm.updateValueAndValidity({
      onlySelf: false,
      emitEvent: true,
    });

    // small delay to let valueChanges settle
    setTimeout(() => {
      this.patching = false;
    });
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
    if (this.questionForm.valid) return null;

    const errors: ValidationErrors = {};

    // form errors
    Object.keys(this.questionForm.controls).forEach((key) => {
      const controlErrors = this.questionForm.controls[key].errors;
      if (controlErrors) errors[key] = controlErrors;
    });
    // choices errors
    this.choices.controls.forEach((key) => {
      Object.keys((key as FormGroup).controls).forEach((k) => {
        const controlErrors = (key as FormGroup).controls[k].errors;
        if (controlErrors) errors[k] = controlErrors;
      });
    });
    // evaluate choices errors
    this.EvaluateChoices.controls.forEach((key) => {
      Object.keys((key as FormGroup).controls).forEach((k) => {
        const controlErrors = (key as FormGroup).controls[k].errors;
        if (controlErrors) errors[k] = controlErrors;
      });
    });

    return Object.keys(errors).length ? errors : this.questionForm.errors;
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

  validateChoiceText(control: AbstractControl): ValidationErrors | null {
    const formArray = control as FormArray;
    const seenTextEn = new Map<string, number[]>();
    const seenTextAr = new Map<string, number[]>();
    const seenEmoji = new Map<string, number[]>();

    formArray.controls.forEach((group, index) => {
      const textEn = group.get('textEn')?.value?.trim().toLowerCase();
      const textAr = group.get('textAr')?.value?.trim().toLowerCase();
      const emoji = group.get('emotion')?.value?.trim()?.toLowerCase();

      // handle textEn
      if (textEn) {
        if (!seenTextEn.has(textEn)) seenTextEn.set(textEn, []);
        seenTextEn.get(textEn)!.push(index);
      }

      // handle textAr
      if (textAr) {
        if (!seenTextAr.has(textAr)) seenTextAr.set(textAr, []);
        seenTextAr.get(textAr)!.push(index);
      }

      // handle emoji
      if (emoji) {
        if (!seenEmoji.has(emoji)) seenEmoji.set(emoji, []);
        seenEmoji.get(emoji)!.push(index);
      }
    });

    // Clear previous errors
    formArray.controls.forEach((group) => {
      const textEnControl = group.get('textEn');
      const textArControl = group.get('textAr');
      const emojiControl = group.get('emotion');

      if (textEnControl?.hasError('duplicate')) {
        const otherErrors = { ...textEnControl.errors };
        delete otherErrors['duplicate'];
        textEnControl.setErrors(
          Object.keys(otherErrors).length ? otherErrors : null
        );
      }

      if (textArControl?.hasError('duplicate')) {
        const otherErrors = { ...textArControl.errors };
        delete otherErrors['duplicate'];
        textArControl.setErrors(
          Object.keys(otherErrors).length ? otherErrors : null
        );
      }
      if (emojiControl?.hasError('duplicate')) {
        const otherErrors = { ...emojiControl.errors };
        delete otherErrors['duplicate'];
        emojiControl.setErrors(
          Object.keys(otherErrors).length ? otherErrors : null
        );
      }
    });

    // Apply duplicate errors
    let hasDuplicate = false;
    seenTextEn.forEach((indices) => {
      if (indices.length > 1) {
        hasDuplicate = true;
        indices.forEach((i) => {
          const textEnControl = formArray.at(i).get('textEn');
          const existingErrors = textEnControl?.errors || {};
          textEnControl?.setErrors({ ...existingErrors, duplicate: true });
        });
      }
    });
    seenTextAr.forEach((indices) => {
      if (indices.length > 1) {
        hasDuplicate = true;
        indices.forEach((i) => {
          const textArControl = formArray.at(i).get('textAr');
          const existingErrors = textArControl?.errors || {};
          textArControl?.setErrors({ ...existingErrors, duplicate: true });
        });
      }
    });
    seenEmoji.forEach((indices) => {
      if (indices.length > 1) {
        hasDuplicate = true;
        indices.forEach((i) => {
          const emojiControl = formArray.at(i).get('emotion');
          const existingErrors = emojiControl?.errors || {};
          emojiControl?.setErrors({ ...existingErrors, duplicate: true });
        });
      }
    });

    return hasDuplicate ? { duplicateText: true } : null;
  }

  get f() {
    return this.questionForm;
  }
  get choices(): FormArray<FormGroup> {
    return this.questionForm.get('choices') as FormArray<FormGroup>;
  }
  get EvaluateChoices(): FormArray<FormGroup> {
    return this.questionForm.get('evaluateChoices') as FormArray<FormGroup>;
  }
  //#endregion
  //#region handle choices
  createChoice(
    isEvaluate: boolean = false,
    obj = { textEn: '', textAr: '', emotion: '' }
  ): FormGroup {
    const controls: {
      textAr: FormControl;
      textEn: FormControl;
      emotion?: FormControl;
    } = {
      textEn: new FormControl(obj.textEn ?? '', {
        validators: Validators.required,
      }),
      textAr: new FormControl(obj.textAr, {
        validators: Validators.required,
      }),
    };
    if (isEvaluate)
      controls.emotion = new FormControl(obj.emotion, {
        validators: Validators.required,
      });
    return new FormGroup({
      ...controls,
    });
  }

  addChoice(
    isEvaluate: boolean = false,
    obj = { textEn: '', textAr: '', emotion: '' }
  ) {
    isEvaluate
      ? this.EvaluateChoices.push(this.createChoice(true, obj))
      : this.choices.push(this.createChoice(false, obj));
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
