import { Component, OnInit, viewChild, ViewContainerRef } from '@angular/core';
import { InitialSurveyCreationComponent } from '../../components/survey/initial-survey-creation/initial-survey-creation.component';

@Component({
  selector: 'app-create-survey',
  standalone: true,
  imports: [InitialSurveyCreationComponent],
  templateUrl: './create-survey.component.html',
  styleUrl: './create-survey.component.css',
})
export class CreateSurveyComponent implements OnInit {
  viewComponentRef = viewChild('container', { read: ViewContainerRef });
  // Store output handlers here
  private outputs: Record<string, Function> = {};

  ngOnInit(): void {
    this.setupOutputHandlers();
    this.loadDynamicComponent(false, { isRedirection: false });
  }

  private setupOutputHandlers() {
    this.outputs = {
      redirectToNextPage: () => {
        console.log('Redirected to questions...');
        this.loadDynamicComponent(true);
      },
      redirectToPreviousPage: () => {
        console.log('Redirected to initial...');
        this.loadDynamicComponent(false, { isRedirection: true });
      },
    };
  }

  async loadDynamicComponent(
    redirectToQuestions: boolean = false,
    inputs: Record<string, any> = {}
  ) {
    let dComponent;

    if (!redirectToQuestions) {
      const { InitialSurveyCreationComponent } = await import(
        './../../components/survey/initial-survey-creation/initial-survey-creation.component'
      );
      dComponent = InitialSurveyCreationComponent;
    } else {
      const { SurveyQuestionsComponent } = await import(
        './../../components/survey/survey-questions/survey-questions.component'
      );
      dComponent = SurveyQuestionsComponent;
    }
    this.viewComponentRef()?.clear();
    const cmpRef = this.viewComponentRef()?.createComponent(dComponent as any);

    // Apply inputs
    if (cmpRef && inputs) {
      for (const [key, value] of Object.entries(inputs)) {
        cmpRef.setInput(key, value);
      }
    }

    // Bind outputs
    for (const [key, handler] of Object.entries(this.outputs)) {
      const instance = cmpRef?.instance as any;
      if (instance[key]?.subscribe) {
        instance[key].subscribe(handler);
      }
    }
  }
}
