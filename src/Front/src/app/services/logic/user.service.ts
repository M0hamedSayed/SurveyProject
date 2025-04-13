import { inject, Injectable } from '@angular/core';
import { GlobalState } from '../../common/classes/globalState';
import { IUser } from '../../common/Interfaces/IUser';
import { SurveyApiService } from '../api/survey-api.service';
import { catchError, Observable, of, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class UserService extends GlobalState<IUser> {
  private _api = inject(SurveyApiService);

  constructor() {
    super();
  }

  InitializeUserData(): Observable<any> {
    return this._api.getMe().pipe(
      tap((value: any) => {
        if (value?.Data) {
          this.setState(value?.Data);
        }
      }),
      catchError((err) => {
        console.log(err);
        return of(null);
      })
    );
  }
}
