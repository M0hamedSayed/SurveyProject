import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const unAuthUrls: string[] = [];
  const shouldSkip = unAuthUrls.some((url) => req.url.includes(url));
  // Always add withCredentials
  let modifiedReq = req.clone({ withCredentials: true });
  // If not an unAuth request, attach token
  if (!shouldSkip) {
    const token = localStorage.getItem('accessToken');
    if (token) {
      modifiedReq = modifiedReq.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`,
        },
      });
    }
  }

  return next(modifiedReq);
};
