import { LocationStrategy, PathLocationStrategy } from "@angular/common";
import { provideHttpClient, withInterceptors } from "@angular/common/http";
import { enableProdMode, inject, isDevMode, provideAppInitializer, provideZonelessChangeDetection } from "@angular/core";
import { createCustomElement } from "@angular/elements";
import { bootstrapApplication } from "@angular/platform-browser";
import { provideAnimationsAsync } from "@angular/platform-browser/animations/async";
import { provideRouter, TitleStrategy, withComponentInputBinding, withInMemoryScrolling, withRouterConfig } from "@angular/router";
import { provideServiceWorker } from "@angular/service-worker";
import { APP_NAME } from "@core";
import { CMS_ELEMENTS } from "@core/features/content/models/cms-elements";
import { apiResponseInterceptor } from "@core/interceptors/api-response-interceptor";
import { dateInterceptor } from "@core/interceptors/date-interceptor";
import { tokenInterceptor } from "@core/interceptors/token-interceptor";
import { InitializationService } from "@core/services/initialization.service";
import { PrependNameTitleStrategy } from "@core/strategies/prepend-name-title.strategy";
import Aura from "@primeng/themes/aura";
import { provideMarkdown } from "ngx-markdown";
import { ConfirmationService, MessageService } from "primeng/api";
import { providePrimeNG } from "primeng/config";
import { AppComponent } from "./app/app.component";
import { Routes } from "./app/pages/routes";
import { environment } from "./environments/environment";

if (environment.production) {
  enableProdMode();
}

bootstrapApplication(AppComponent, {
  providers: [
    // 1. Core Angular providers
    provideZonelessChangeDetection(),

    // 2. Routing
    provideRouter(Routes, withInMemoryScrolling(), withComponentInputBinding(), withRouterConfig({})),

    // 3. HTTP & Interceptors
    provideHttpClient(withInterceptors([tokenInterceptor, apiResponseInterceptor, dateInterceptor])),

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

    // Iterate through CMS_ELEMENTS and define each as a custom element so that these components
    // can be used in zones and pages.
    CMS_ELEMENTS.forEach(element => {
      customElements.define(element.tagName, createCustomElement(element.component, { injector }));
    });
  })
  .catch(err => console.error("Error bootstrapping application:", err));
