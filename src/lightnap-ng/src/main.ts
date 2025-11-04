import { LocationStrategy, PathLocationStrategy } from "@angular/common";
import { provideHttpClient, withInterceptors } from "@angular/common/http";
import { enableProdMode, importProvidersFrom, inject, isDevMode, provideAppInitializer, provideZonelessChangeDetection } from "@angular/core";
import { createCustomElement } from "@angular/elements";
import { bootstrapApplication, BrowserModule } from "@angular/platform-browser";
import { provideAnimationsAsync } from "@angular/platform-browser/animations/async";
import { provideRouter, TitleStrategy, withComponentInputBinding, withInMemoryScrolling, withRouterConfig } from "@angular/router";
import { provideServiceWorker } from "@angular/service-worker";
import { API_URL_ROOT, APP_NAME, throwInlineError } from "@core";
import { BrandedCardComponent } from "@core/components/branded-card/branded-card.component";
import { apiResponseInterceptor } from "@core/interceptors/api-response-interceptor";
import { tokenInterceptor } from "@core/interceptors/token-interceptor";
import { InitializationService } from "@core/services/initialization.service";
import { PrependNameTitleStrategy } from "@core/strategies/prepend-name-title.strategy";
import Aura from "@primeng/themes/aura";
import { provideMarkdown } from "ngx-markdown";
import { ConfirmationService, MessageService } from "primeng/api";
import { Card } from "primeng/card";
import { providePrimeNG } from "primeng/config";
import { Panel } from "primeng/panel";
import { AppComponent } from "./app/app.component";
import { Routes } from "./app/pages/routes";
import { environment } from "./environments/environment";
import { CMS_ELEMENTS } from "@core/features/content/models/cms-elements";

if (environment.production) {
  enableProdMode();
}

bootstrapApplication(AppComponent, {
  providers: [
    // 1. Core Angular providers
    importProvidersFrom(BrowserModule),
    provideZonelessChangeDetection(),
    provideAnimationsAsync(),

    // 2. Routing
    provideRouter(Routes, withInMemoryScrolling(), withComponentInputBinding(), withRouterConfig({})),

    // 3. HTTP & Interceptors
    provideHttpClient(withInterceptors([tokenInterceptor, apiResponseInterceptor])),

    // 4. Third-party libraries
    providePrimeNG({
      theme: {
        preset: Aura,
        options: {
          darkModeSelector: ".app-dark",
        },
      },
    }),
    provideMarkdown(),

    // 5. Application initialization
    InitializationService,
    provideAppInitializer(() => inject(InitializationService).initialize()),

    // 6. Configuration values (tokens)
    {
      provide: API_URL_ROOT,
      useValue: environment.apiUrlRoot ?? throwInlineError("Required setting 'environment.apiUrlRoot' is not defined."),
    },
    { provide: APP_NAME, useValue: environment.appName },

    // 7. Strategies & overrides
    { provide: LocationStrategy, useClass: PathLocationStrategy },
    { provide: TitleStrategy, useClass: PrependNameTitleStrategy },

    // 8. Services
    MessageService,
    ConfirmationService,

    // 9. Service Worker (last, as it's environment-dependent)
    provideServiceWorker("ngsw-worker.js", {
      enabled: !isDevMode(),
      registrationStrategy: "registerWhenStable:30000",
    }),
  ],
})
  .then(appRef => {
    const injector = appRef.injector;

    CMS_ELEMENTS.forEach(element => {
      customElements.define(element.tagName, createCustomElement(element.component, { injector }));
    });
  })
  .catch(err => console.error("Error bootstrapping application:", err));
