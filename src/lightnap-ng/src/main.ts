import { LocationStrategy, PathLocationStrategy } from "@angular/common";
import { provideHttpClient, withInterceptors } from "@angular/common/http";
import { enableProdMode, importProvidersFrom, inject, provideAppInitializer, provideZonelessChangeDetection } from "@angular/core";
import { bootstrapApplication, BrowserModule } from "@angular/platform-browser";
import { provideAnimations } from "@angular/platform-browser/animations";
import { provideRouter, TitleStrategy, withComponentInputBinding, withInMemoryScrolling, withRouterConfig } from "@angular/router";
import { API_URL_ROOT, APP_NAME, throwInlineError } from "@core";
import { apiResponseInterceptor } from "@core/interceptors/api-response-interceptor";
import { tokenInterceptor } from "@core/interceptors/token-interceptor";
import { InitializationService } from "@core/services/initialization.service";
import { PrependNameTitleStrategy } from "@core/strategies/prepend-name-title.strategy";
import { Routes } from "@routing/routes";
import { ConfirmationService, MessageService } from "primeng/api";
import { AppComponent } from "./app/app.component";
import { environment } from "./environments/environment";
import { providePrimeNG } from "primeng/config";
import Aura from "@primeng/themes/aura";

if (environment.production) {
  enableProdMode();
}

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(Routes, withInMemoryScrolling(), withComponentInputBinding(), withRouterConfig({})),
    importProvidersFrom(BrowserModule),
    InitializationService,
    provideZonelessChangeDetection(),
    provideAppInitializer(() => inject(InitializationService).initialize()),
    provideAnimations(),
    providePrimeNG({
      theme: {
        preset: Aura,
        options: {
          darkModeSelector: ".app-dark"
        },
      },
    }),
    {
      provide: API_URL_ROOT,
      useValue: environment.apiUrlRoot ?? throwInlineError("Required setting 'environment.apiUrlRoot' is not defined."),
    },
    { provide: APP_NAME, useValue: environment.appName },
    { provide: LocationStrategy, useClass: PathLocationStrategy },
    { provide: TitleStrategy, useClass: PrependNameTitleStrategy },
    provideHttpClient(withInterceptors([tokenInterceptor, apiResponseInterceptor])),
    MessageService,
    ConfirmationService,
  ],
}).catch(err => console.error("Error bootstrapping application:", err));
