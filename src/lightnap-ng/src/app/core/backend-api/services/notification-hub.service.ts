import { Injectable, inject } from "@angular/core";
import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { Observable, Subject } from "rxjs";
import { NotificationDto } from "../dtos";
import { IdentityService } from "@core/services/identity.service";
import { NotificationHelper } from "../helpers/notification.helper";

@Injectable({
  providedIn: "root",
})
export class NotificationHubService {
  #identityService = inject(IdentityService);
  #hubConnection: HubConnection = new HubConnectionBuilder()
    .withUrl("/api/hubs/notifications", {
      accessTokenFactory: () => this.#identityService.getToken() || "",
    })
    .withAutomaticReconnect()
    .build();
  #notificationSubject = new Subject<NotificationDto>();

  constructor() {
    this.#hubConnection.on("ReceiveNotification", (notification: NotificationDto) => {
      NotificationHelper.rehydrate(notification);
      this.#notificationSubject.next(notification);
    });
  }

  startConnection(): Promise<void> {
    return this.#hubConnection.start();
  }

  stopConnection(): Promise<void> {
    return this.#hubConnection.stop();
  }

  get notifications$(): Observable<NotificationDto> {
    return this.#notificationSubject.asObservable();
  }
}
