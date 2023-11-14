import { bootstrapApplication, BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { provideRouter } from '@angular/router';
import { DEFAULT_ROUTES } from './app/routes';
import { importProvidersFrom, LOCALE_ID } from '@angular/core';
import { provideHttpClient } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import localeRu from '@angular/common/locales/ru';
import { registerLocaleData } from '@angular/common';

registerLocaleData(localeRu, 'ru');

bootstrapApplication(AppComponent, {
  providers: [
    { provide: LOCALE_ID, useValue: 'ru' },
    importProvidersFrom(BrowserModule),
    provideHttpClient(),
    provideRouter(DEFAULT_ROUTES),
    provideAnimations(),
  ],
}).catch((err) => console.log(err));
