import { CommonModule } from "@angular/common";
import { Component, inject, signal } from "@angular/core";
import { ConfirmDialogComponent, DeviceDto, IdentityService, setApiErrors, TypeHelpers } from "@core";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { ProfileService } from "@core/services/profile.service";
import { ConfirmationService } from "primeng/api";
import { ButtonModule } from "primeng/button";
import { PanelModule } from "primeng/panel";
import { TableModule } from "primeng/table";

@Component({
  standalone: true,
  templateUrl: "./devices.component.html",
  imports: [CommonModule, TableModule, ButtonModule, ErrorListComponent, PanelModule, ApiResponseComponent, ConfirmDialogComponent],
})
export class DevicesComponent {
  readonly #devicesService = inject(IdentityService);
  readonly #confirmationService = inject(ConfirmationService);

  readonly devices$ = signal(this.#devicesService.getDevices());

  readonly errors = signal(new Array<string>());

  readonly asDevices = TypeHelpers.cast<Array<DeviceDto>>;
  readonly asDevice = TypeHelpers.cast<DeviceDto>;

  revokeDevice(event: any, deviceId: string) {
    this.#confirmationService.confirm({
      header: "Confirm Revoke",
      message: `Are you sure that you want to revoke this device?`,
      target: event.target,
      key: deviceId,
      accept: () => {
        this.#devicesService.revokeDevice(deviceId).subscribe({
          next: () => this.devices$.set(this.#devicesService.getDevices()),
          error: setApiErrors(this.errors),
        });
      },
    });
  }
}
