import { ApplicationConfig, provideZonelessChangeDetection, LOCALE_ID} from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { authInterceptor } from './core/interceptors/auth.interceptors';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { getEspañolPaginatorIntl } from './core/providers/paginator-es';
import { MAT_DATE_LOCALE } from '@angular/material/core';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZonelessChangeDetection(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideAnimationsAsync(),
    { provide: MatPaginatorIntl, useValue: getEspañolPaginatorIntl() },
    { provide: MAT_DATE_LOCALE, useValue: 'es-PE' },
    { provide: LOCALE_ID, useValue: 'es-PE' }
  ]
};
