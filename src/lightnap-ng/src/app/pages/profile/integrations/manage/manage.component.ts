import { CommonModule } from "@angular/common";
import { Component, computed, inject, input } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { ErrorApiResponse, Integration, TypeHelpers } from "@core";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { IntegrationsService } from "@core/features/integrations/services/integrations.service";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { ToastService } from "@core/services/toast.service";
import { ButtonModule } from "primeng/button";
import { CardModule } from "primeng/card";
import { CheckboxModule } from "primeng/checkbox";
import { InputTextModule } from "primeng/inputtext";
import { PanelModule } from "primeng/panel";
import { TagModule } from "primeng/tag";
import { TextareaModule } from "primeng/textarea";
import { tap } from "rxjs";

@Component({
  templateUrl: "./manage.component.html",
  imports: [
    CommonModule,
    ReactiveFormsModule,
    InputTextModule,
    CheckboxModule,
    TextareaModule,
    ButtonModule,
    TagModule,
    PanelModule,
    CardModule,
    ApiResponseComponent,
  ],
})
export class ManageComponent {
  readonly #integrationsService = inject(IntegrationsService);
  readonly #toast = inject(ToastService);
  readonly #fb = inject(FormBuilder);
  readonly #routeAlias = inject(RouteAliasService);

  readonly integrationId = input.required<string>();
  readonly integrationIdNumber = computed(() => Number(this.integrationId()));

  readonly form = this.#fb.nonNullable.group({
    friendlyName: this.#fb.nonNullable.control<string>("", [Validators.required]),
    credentials: this.#fb.control<string>(""),
    shareWithClient: this.#fb.nonNullable.control<boolean>(false),
  });

  readonly integration$ = computed(() => {
    return this.#integrationsService.getMyIntegration(this.integrationIdNumber()).pipe(
      tap(integration => {
        if (!integration) {
          throw new ErrorApiResponse(["Integration not found"]);
        }
        this.form.patchValue(integration.integration);
      })
    );
  });

  readonly asIntegration = TypeHelpers.cast<Integration>;

  updateIntegration() {
    this.#integrationsService.updateMyIntegration(this.integrationIdNumber(), this.form.getRawValue()).subscribe(integration => {
      if (!integration) {
        throw new Error("Failed to update integration");
      }
      this.#toast.success("Integration updated successfully");
      this.#routeAlias.navigate("my-integrations");
    });
  }
}
