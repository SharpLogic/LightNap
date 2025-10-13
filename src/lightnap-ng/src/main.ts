import { LocationStrategy, PathLocationStrategy } from "@angular/common";
import { provideHttpClient, withInterceptors } from "@angular/common/http";
import { enableProdMode, importProvidersFrom, inject, isDevMode, provideAppInitializer, provideZonelessChangeDetection } from "@angular/core";
import { createCustomElement } from "@angular/elements";
import { bootstrapApplication, BrowserModule } from "@angular/platform-browser";
import { provideAnimations } from "@angular/platform-browser/animations";
import { provideRouter, TitleStrategy, withComponentInputBinding, withInMemoryScrolling, withRouterConfig } from "@angular/router";
import { provideServiceWorker } from "@angular/service-worker";
import { API_URL_ROOT, APP_NAME, throwInlineError } from "@core";
import { UserIdComponent } from "@core/components/user-id/user-id.component";
import { apiResponseInterceptor } from "@core/interceptors/api-response-interceptor";
import { tokenInterceptor } from "@core/interceptors/token-interceptor";
import { InitializationService } from "@core/services/initialization.service";
import { PrependNameTitleStrategy } from "@core/strategies/prepend-name-title.strategy";
import Aura from "@primeng/themes/aura";
import { ConfirmationService, MessageService } from "primeng/api";
import { Card } from "primeng/card";
import { providePrimeNG } from "primeng/config";
import { AppComponent } from "./app/app.component";
import { Routes } from "./app/pages/routes";
import { environment } from "./environments/environment";

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
          darkModeSelector: ".app-dark",
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
    provideServiceWorker("ngsw-worker.js", {
      enabled: !isDevMode(),
      registrationStrategy: "registerWhenStable:30000",
    }),
  ],
})
  .then(appRef => {
    const injector = appRef.injector;

    customElements.define("user-id-control", createCustomElement(UserIdComponent, { injector }));
    customElements.define("p-card-control", createCustomElement(Card, { injector }));

    // Register other dynamic components as needed
  })
  .catch(err => console.error("Error bootstrapping application:", err));
