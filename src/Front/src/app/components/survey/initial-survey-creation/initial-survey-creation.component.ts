import { Component, inject, input, OnInit, output } from '@angular/core';
import { SurveyCreateFormService } from '../../../services/logic/survey-create-form.service';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { CalendarModule } from 'primeng/calendar';
import { InputSwitchModule } from 'primeng/inputswitch';
import { ButtonModule } from 'primeng/button';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { ImageUploadComponent } from '../image-upload/image-upload.component';
import { SurveyTypeAutoCompleteComponent } from '../survey-type-auto-complete/survey-type-auto-complete.component';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-initial-survey-creation',
  standalone: true,
  imports: [
    DropdownModule,
    InputTextModule,
    InputTextareaModule,
    CalendarModule,
    InputSwitchModule,
    ReactiveFormsModule,
    ImageUploadComponent,
    ButtonModule,
    SurveyTypeAutoCompleteComponent,
    RouterLink,
  ],
  templateUrl: './initial-survey-creation.component.html',
  styleUrl: './initial-survey-creation.component.css',
})
export class InitialSurveyCreationComponent implements OnInit {
  private _surveyCreationState = inject(SurveyCreateFormService);

  redirectToNextPage = output<void>();
  isRedirection = input<boolean>(false);

  ngOnInit(): void {
    if (this.isRedirection())
      this.surveyForm.patchValue(this._surveyCreationState.state());
  }

  surveyForm: FormGroup = new FormGroup(
    {
      surveyTypeId: new FormControl('', {
        validators: Validators.required,
      }),
      nameEn: new FormControl('', { validators: Validators.required }),
      nameAr: new FormControl('', { validators: Validators.required }),
      startDate: new FormControl('', { validators: Validators.required }),
      endDate: new FormControl('', { validators: Validators.required }),
      descriptionEn: new FormControl(''),
      descriptionAr: new FormControl(''),
      closingAddressEn: new FormControl(''),
      closingAddressAr: new FormControl(''),
      closingStatementEn: new FormControl(''),
      closingStatementAr: new FormControl(''),
      isRequired: new FormControl(false),
    },
    {
      validators: this.minHourDifference,
    }
  );

  getControl(name: string) {
    return this.surveyForm.controls[name] as FormControl;
  }

  minHourDifference(group: AbstractControl): ValidationErrors | null {
    const start = group.get('startDate')?.value;
    const end = group.get('endDate')?.value;

    if (!start || !end) return null;

    const startDate = new Date(start);
    const endDate = new Date(end);
    const diffMs = endDate.getTime() - startDate.getTime();

    if (diffMs < 60 * 60 * 1000) {
      return { minHourDiff: true };
    }

    return null;
  }

  get f() {
    return this.surveyForm;
  }

  validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(
      (field: string | readonly (string | number)[]) => {
        const control = formGroup.get(field);
        if (control instanceof FormControl) {
          control.markAsTouched({ onlySelf: true });
        } else if (control instanceof FormGroup) {
          this.validateAllFormFields(control);
        }
      }
    );
  }

  onNextPage() {
    console.log(this.surveyForm);

    if (this.surveyForm.invalid) {
      this.surveyForm.markAllAsTouched();
    } else {
      // update state

      this._surveyCreationState.setState(this.surveyForm.value);
      console.log(this._surveyCreationState.state());
      // redirect to next page
      this.redirectToNextPage.emit();
    }
  }
}
