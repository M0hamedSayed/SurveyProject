import {
  Component,
  ElementRef,
  inject,
  OnInit,
  ViewChild,
} from '@angular/core';
import { SurveyCreateFormService } from '../../../services/logic/survey-create-form.service';

@Component({
  selector: 'app-image-upload',
  standalone: true,
  imports: [],
  templateUrl: './image-upload.component.html',
  styleUrl: './image-upload.component.css',
})
export class ImageUploadComponent implements OnInit {
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;
  private _surveyForm = inject(SurveyCreateFormService);

  imagePreview: string | null = null;
  error: string | null = null;
  maxSizeMB = 10;
  maxWidth = 1200;
  maxHeight = 1200;

  ngOnInit(): void {
    if (this._surveyForm.state()?.image)
      this.handleImagePreview(this._surveyForm.state().image as File, false);
  }

  openFileSelector() {
    this.fileInput.nativeElement.click();
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    this.error = null;
    this.imagePreview = null;

    if (!file) return;

    const allowedTypes = ['image/jpeg', 'image/png'];

    // 1. Check type
    if (!allowedTypes.includes(file.type)) {
      this.error = 'Only JPG and PNG files are allowed.';
      return;
    }

    // 2. Check size
    const sizeMB = file.size / (1024 * 1024);
    if (sizeMB > this.maxSizeMB) {
      this.error = `Image must be less than ${this.maxSizeMB}MB.`;
      return;
    }

    // 3. Check dimensions
    this.handleImagePreview(file, true);
  }

  handleImagePreview(file: File, withValidation = false) {
    const reader = new FileReader();
    reader.onload = (e: any) => {
      const img = new Image();
      img.src = e.target.result;

      img.onload = () => {
        if (withValidation)
          if (img.width > this.maxWidth || img.height > this.maxHeight) {
            this.error = `Image resolution must not exceed ${this.maxWidth}x${this.maxHeight}.`;
            return;
          }

        this.imagePreview = e.target.result;
        if (withValidation) this._surveyForm.set('image', file);
      };
    };

    reader.readAsDataURL(file);
  }
}
