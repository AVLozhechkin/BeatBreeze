import { bootstrapApplication, BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { provideRouter } from '@angular/router';
import { DEFAULT_ROUTES } from './app/routes';
import { importProvidersFrom, LOCALE_ID } from '@angular/core';
import {
  HTTP_INTERCEPTORS,
  provideHttpClient,
  withInterceptorsFromDi,
} from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import localeRu from '@angular/common/locales/ru';
import { registerLocaleData } from '@angular/common';
import { CacheInterceptor } from './app/shared/interceptors/cache-interceptor';
import { provideToastr } from 'ngx-toastr';

registerLocaleData(localeRu, 'ru');

bootstrapApplication(AppComponent, {
  providers: [
    { provide: LOCALE_ID, useValue: 'ru' },
    importProvidersFrom(BrowserModule),
    provideHttpClient(withInterceptorsFromDi()),
    { provide: HTTP_INTERCEPTORS, useClass: CacheInterceptor, multi: true },
    provideRouter(DEFAULT_ROUTES),
    provideAnimations(),
    provideToastr(),
  ],
}).catch((err) => console.log(err));
