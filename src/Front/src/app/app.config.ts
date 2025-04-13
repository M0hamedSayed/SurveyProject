import { APP_INITIALIZER, ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './interceptor/auth.interceptor';
import { UserService } from './services/logic/user.service';
import { firstValueFrom } from 'rxjs';

function initApp(user: UserService) {
  return () => firstValueFrom(user.InitializeUserData());
}

const appInitializerProvider = {
  provide: APP_INITIALIZER,
  useFactory: initApp,
  deps: [UserService],
  multi: true,
};

export const appConfig: ApplicationConfig = {
  providers: [
    appInitializerProvider,
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideAnimationsAsync(),
  ],
};
