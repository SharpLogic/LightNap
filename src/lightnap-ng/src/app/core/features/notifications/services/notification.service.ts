import { Injectable, inject } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import {
  ApiResponseDto,
  LatestNotifications,
  NotificationDto,
  NotificationItem,
  NotificationSearchResults,
  NotificationStatus,
  NotificationType,
  SearchNotificationsRequestDto,
} from "@core";
import { NotificationHubService } from "@core/backend-api/hubs/notification-hub.service";
import { LightNapWebApiService } from "@core/backend-api/services/lightnap-api";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { AdminUsersService } from "@core/features/users/services/admin-users.service";
import { RequestPollingManager } from "@core/helpers";
import { IdentityService } from "@core/services/identity.service";
import { ToastService } from "@core/services/toast.service";
import { ReplaySubject, combineLatest, finalize, forkJoin, map, of, switchMap, tap } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class NotificationService {
  #webApiService = inject(LightNapWebApiService);
  #identityService = inject(IdentityService);
  #adminService = inject(AdminUsersService);
  #routeAlias = inject(RouteAliasService);
  #toastService = inject(ToastService);
  #hubService = inject(NotificationHubService);

  #notifications?: Array<NotificationItem>;
  #notificationsSubject = new ReplaySubject<Array<NotificationItem>>(1);
  #unreadCountSubject = new ReplaySubject<number>(1);
  #unreadCount = 0;
  #pollingManager = new RequestPollingManager(() => this.#requestLatestNotifications(), 5 * 60 * 1000);

  constructor() {
    this.#hubService.notifications$.subscribe(notification => this.#onNotificationReceived(notification));

    this.#identityService
      .watchLoggedIn$()
      .pipe(takeUntilDestroyed())
      .subscribe({
        next: loggedIn => {
          if (loggedIn) {
            this.#hubService.startConnection().catch(err => console.error("Error starting SignalR connection:", err));
            this.#pollingManager.startPolling(); // Keep polling as fallback
          } else {
            this.#hubService.stopConnection().catch(err => console.error("Error stopping SignalR connection:", err));
            this.#notifications = undefined;
            this.#notificationsSubject.next([]);
            this.#unreadCount = 0;
            this.#unreadCountSubject.next(0);
            this.#pollingManager.stopPolling();
          }
        },
      });
  }

  searchNotifications(searchNotificationsRequest: SearchNotificationsRequestDto) {
    return this.#webApiService.searchMyNotifications(searchNotificationsRequest).pipe(
      tap(results => {
        this.#unreadCount = results.unreadCount;
        this.#unreadCountSubject.next(results.unreadCount);
      }),
      switchMap(results =>
        this.#loadNotificationItems(results.data).pipe(map(notifications => <NotificationSearchResults>{ ...results, notifications }))
      )
    );
  }

  watchLatest$() {
    return combineLatest([this.#notificationsSubject, this.#unreadCountSubject]).pipe(
      map(([notifications, unreadCount]) => <LatestNotifications>{ notifications, unreadCount })
    );
  }

  #onNotificationReceived(notification: NotificationDto) {
    this.#loadNotificationItem(notification).subscribe({
      next: item => {
        if (item) {
          this.#notifications = [item, ...(this.#notifications || [])];
          this.#notificationsSubject.next(this.#notifications);
          this.#unreadCount++;
          this.#unreadCountSubject.next(this.#unreadCount);
          this.#toastService.info(item.title, item.description);
        }
      },
      error: err => console.error("Error processing received notification:", err),
    });
  }

  #requestLatestNotifications() {
    return this.searchNotifications({ sinceId: this.#notifications?.[0]?.id, pageNumber: 1, pageSize: 25 }).pipe(
      tap(results => {
        if (!results.notifications.length && this.#notifications) return;
        this.#notifications = [...results.notifications, ...(this.#notifications || [])];
        this.#notificationsSubject.next(this.#notifications);
      })
    );
  }

  #refreshLatestNotifications() {
    this.#pollingManager.pausePolling();
    this.searchNotifications({ pageNumber: 1, pageSize: 25 })
      .pipe(finalize(() => this.#pollingManager.resumePolling()))
      .subscribe({
        next: results => {
          this.#notifications = results.notifications;
          this.#notificationsSubject.next(this.#notifications);
        },
        error: (response: ApiResponseDto<any>) => console.error("Unable to refresh unread notifications", response.errorMessages),
      });
  }

  markAllNotificationsAsRead() {
    return this.#webApiService.markAllMyNotificationsAsRead().pipe(tap(() => this.#refreshLatestNotifications()));
  }

  markNotificationAsRead(id: number) {
    return this.#webApiService.markMyNotificationAsRead(id).pipe(tap(() => this.#refreshLatestNotifications()));
  }

  #loadNotificationItems(notifications: Array<NotificationDto>) {
    if (!notifications.length) return of(new Array<NotificationItem>());
    return forkJoin(notifications.map(notification => this.#loadNotificationItem(notification))).pipe(
      // Filter out any null items (e.g., if the user was deleted). In the future we may prefer to keep the nulls
      // and render them to indicate that the notification is no longer valid.
      map(notificationItems => notificationItems.filter(item => item !== null))
    );
  }

  #loadNotificationItem(notification: NotificationDto) {
    var notificationItem = <NotificationItem>{
      id: notification.id,
      timestamp: notification.timestamp,
      isUnread: notification.status === NotificationStatus.Unread,
      title: "",
      description: "",
      routerLink: [],
    };

    switch (notification.type) {
      case NotificationType.AdministratorNewUserRegistration:
        return this.#adminService.getUser(notification.data["userId"] as string).pipe(
          map(user => {
            // User may have been deleted since the notification was created
            if (!user) return null;

            notificationItem.title = `New user registered: ${user.userName}`;
            notificationItem.description = "A new user registered!";
            notificationItem.icon = "pi pi-user";
            notificationItem.routerLink = this.#routeAlias.getRoute("admin-user", user.userName);
            return notificationItem;
          })
        );
      default:
        throw new Error(`Unknown notification type in NotificationService: '${notification.type}'`);
    }
  }
}
