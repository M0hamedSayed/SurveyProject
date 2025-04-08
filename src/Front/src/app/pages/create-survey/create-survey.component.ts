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

  ngOnInit(): void {
    this.loadDynamicComponent(
      true,
      { isRedirection: false },
      {
        redirectToNextPage: () => {
          console.log('Proceeding to questions...');
          this.loadDynamicComponent(true);
        },
      }
    );
  }

  async loadDynamicComponent(
    redirectToQuestions: boolean = false,
    inputs: Record<string, any> = {},
    outputs: Record<string, Function> = {}
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

    // Bind outputs (assumes EventEmitter)
    if (cmpRef && outputs) {
      const instance = cmpRef.instance as any; // 👈 safely cast
      for (const [key, handler] of Object.entries(outputs)) {
        if (instance[key]?.subscribe) {
          instance[key].subscribe(handler);
        }
      }
    }
  }
}
