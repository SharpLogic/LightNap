import { AdminService } from "@admin/services/admin.service";
import { inject, Injectable } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { ApiResponse, PagedResponse, RequestPollingManager } from "@core";
import { IdentityService } from "@identity";
import { LatestNotifications, NotificationItem, SearchNotificationsRequest } from "@profile";
import { Notification } from "@profile/models/response/notification";
import { RouteAliasService } from "@routing";
import { combineLatest, finalize, forkJoin, map, of, ReplaySubject, switchMap, tap } from "rxjs";
import { DataService } from "./data.service";

@Injectable({
  providedIn: "root",
})
export class NotificationService {
  #dataService = inject(DataService);
  #identityService = inject(IdentityService);
  #adminService = inject(AdminService);
  #routeAlias = inject(RouteAliasService);

  #notifications?: Array<NotificationItem>;
  #notificationsSubject = new ReplaySubject<Array<NotificationItem>>(1);
  #unreadCountSubject = new ReplaySubject<number>(1);
  #pollingManager = new RequestPollingManager(() => this.#requestLatestNotifications(), 15 * 1000);

  constructor() {
    this.#identityService
      .watchLoggedIn$()
      .pipe(takeUntilDestroyed())
      .subscribe({
        next: loggedIn => {
          if (loggedIn) {
            this.#pollingManager.startPolling();
          } else {
            this.#notifications = undefined;
            this.#notificationsSubject.next([]);
            this.#unreadCountSubject.next(0);
            this.#pollingManager.stopPolling();
          }
        },
      });
  }

  searchNotifications(searchNotificationsRequest: SearchNotificationsRequest) {
    return this.#dataService.searchNotifications(searchNotificationsRequest).pipe(
      tap(results => this.#unreadCountSubject.next(results.unreadCount)),
      switchMap(results =>
        this.#loadNotificationItems(results.data).pipe(map(notifications => <PagedResponse<NotificationItem>>{ ...results, data: notifications }))
      )
    );
  }

  watchLatest$() {
    return combineLatest([this.#notificationsSubject, this.#unreadCountSubject]).pipe(
      map(([notifications, unreadCount]) => <LatestNotifications>{ notifications, unreadCount })
    );
  }

  #requestLatestNotifications() {
    return this.searchNotifications({ sinceId: this.#notifications?.[0]?.id }).pipe(
      tap(results => {
        if (!results.data.length && this.#notifications) return;
        this.#notifications = [...results.data, ...(this.#notifications || [])];
        this.#notificationsSubject.next(this.#notifications);
      })
    );
  }

  #refreshLatestNotifications() {
    this.#pollingManager.pausePolling();
    this.searchNotifications({})
      .pipe(finalize(() => this.#pollingManager.resumePolling()))
      .subscribe({
        next: results => {
          this.#notifications = results.data;
          this.#notificationsSubject.next(this.#notifications);
        },
        error: (response: ApiResponse<any>) => console.error("Unable to refresh unread notifications", response.errorMessages),
      });
  }

  markAllNotificationsAsRead() {
    return this.#dataService.markAllNotificationsAsRead().pipe(tap(() => this.#refreshLatestNotifications()));
  }

  markNotificationAsRead(id: number) {
    return this.#dataService.markNotificationAsRead(id).pipe(tap(() => this.#refreshLatestNotifications()));
  }

  #loadNotificationItems(notifications: Array<Notification>) {
    if (!notifications.length) return of(new Array<NotificationItem>());
    return forkJoin(notifications.map(notification => this.#loadNotificationItem(notification)));
  }

  #loadNotificationItem(notification: Notification) {
    var notificationItem = <NotificationItem>{
      id: notification.id,
      timestamp: notification.timestamp,
      isUnread: notification.status === "Unread",
    };

    switch (notification.type) {
      case "AdministratorNewUserRegistration":
        return this.#adminService.getUser(notification.data.userId).pipe(
          map(user => {
            notificationItem.title = `New user registered: ${user.userName}`;
            notificationItem.description = "A new user registered!";
            notificationItem.icon = "pi pi-user";
            notificationItem.routerLink = this.#routeAlias.getRoute("admin-user", user.id);
            return notificationItem;
          })
        );
      default:
        throw new Error(`Unknown notification type in NotificationService: '${notification.type}'`);
    }
  }
}
