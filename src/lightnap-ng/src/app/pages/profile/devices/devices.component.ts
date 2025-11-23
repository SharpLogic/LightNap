import { CommonModule } from "@angular/common";
import { Component, inject, signal } from "@angular/core";
import { DeviceDto, setApiErrors, TypeHelpers } from "@core";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { ConfirmDialogComponent } from "@core/components/confirm-dialog/confirm-dialog.component";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { IdentityService } from "@core/services/identity.service";
import { ConfirmationService } from "primeng/api";
import { ButtonModule } from "primeng/button";
import { PanelModule } from "primeng/panel";
import { TableModule } from "primeng/table";
import { tap } from "rxjs";

@Component({
  standalone: true,
  templateUrl: "./devices.component.html",
  imports: [CommonModule, TableModule, ButtonModule, ErrorListComponent, PanelModule, ApiResponseComponent, ConfirmDialogComponent],
})
export class DevicesComponent {
  readonly #devicesService = inject(IdentityService);
  readonly #confirmationService = inject(ConfirmationService);

  readonly devices$ = signal(
    this.#devicesService.getDevices().pipe(
      tap(devices => {
        console.log("Devices loaded:", devices);
      })
    )
  );

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
