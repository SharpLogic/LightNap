import { CommonModule } from "@angular/common";
import { Component, inject, input } from "@angular/core";
import { Router } from "@angular/router";
import { NotificationItem } from "@core";
import { NotificationService } from "@core/features/notifications/services/notification.service";
import { SincePipe } from "@core/pipes/since.pipe";
import { ButtonModule } from "primeng/button";

@Component({
  selector: "ln-notification-item",
  templateUrl: "./notification-item.component.html",
  imports: [CommonModule, ButtonModule, SincePipe],
})
export class NotificationItemComponent {
  readonly #notificationService = inject(NotificationService);
  readonly #router = inject(Router);
  readonly notification = input.required<NotificationItem>();

  onClick() {
    this.#notificationService.markNotificationAsRead(this.notification().id).subscribe();
    this.#router.navigate(this.notification().routerLink);
  }
}
