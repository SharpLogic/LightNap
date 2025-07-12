import { CommonModule } from "@angular/common";
import { Component, inject, signal } from "@angular/core";
import { Router } from "@angular/router";
import { ErrorListComponent, ApiResponseComponent, EmptyPagedResponse, NotificationItem, ToastService } from "@core";
import { NotificationItemComponent } from "@core/components/notification-item/notification-item.component";
import { NotificationService } from "@core/notifications/services";
import { ButtonModule } from "primeng/button";
import { PanelModule } from "primeng/panel";
import { TableLazyLoadEvent, TableModule } from "primeng/table";
import { startWith, Subject, switchMap } from "rxjs";

@Component({
  standalone: true,
  templateUrl: "./notifications.component.html",
  imports: [CommonModule, TableModule, ButtonModule, ErrorListComponent, PanelModule, ApiResponseComponent, NotificationItemComponent],
})
export class NotificationsComponent {
  readonly pageSize = 10;

  readonly #notificationService = inject(NotificationService);
  readonly #toast = inject(ToastService);
  readonly #router = inject(Router);

  #lazyLoadEventSubject = new Subject<TableLazyLoadEvent>();
  notifications$ = this.#lazyLoadEventSubject.pipe(
    switchMap(_ =>
      this.#notificationService.searchNotifications({
        pageSize: this.pageSize,
        pageNumber: this.#currentPage,
      })
    ),
    // We need to bootstrap the p-table with a response to get the whole process running. We do it this way to
    // fake an empty response so we can avoid a redundant call to the API.
    startWith(new EmptyPagedResponse<NotificationItem>())
  );

  errors = signal(new Array<string>());
  #currentPage = 0;

  onLazyLoad(event: TableLazyLoadEvent) {
    this.#currentPage = (event.first ?? 0) / this.pageSize + 1;
    this.#lazyLoadEventSubject.next(event);
  }

  notificationClicked(notification: NotificationItem) {
    this.#notificationService.markNotificationAsRead(notification.id).subscribe();
    this.#router.navigate(notification.routerLink);
  }

  markAllAsRead() {
    this.#notificationService.markAllNotificationsAsRead().subscribe({
      next: () => {
        this.#toast.success("All notifications marked as read.");
        this.#lazyLoadEventSubject.next({ first: 0 });
      },
      error: response => this.errors.set(response.errorMessages),
    });
  }
}
