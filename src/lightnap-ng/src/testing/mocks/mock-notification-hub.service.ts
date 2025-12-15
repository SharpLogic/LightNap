import { Injectable } from "@angular/core";
import { NotificationDto } from "@core";
import { Observable, Subject } from "rxjs";

@Injectable()
export class MockNotificationHubService {
  startConnection(): Promise<void> {
    return Promise.resolve();
  }

  stopConnection(): Promise<void> {
    return Promise.resolve();
  }

  get notifications$(): Observable<NotificationDto> {
    return new Subject<NotificationDto>();
  }
}
