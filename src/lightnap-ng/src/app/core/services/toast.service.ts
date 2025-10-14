import { Injectable, inject } from "@angular/core";
import { MessageService } from "primeng/api";

@Injectable({
  providedIn: "root",
})
/**
 * @class ToastService
 * @description
 * The ToastService class is responsible for displaying toast messages using the main toast channel.
 */
export class ToastService {
  #messageService = inject(MessageService);

  /**
   * Displays a toast message.
   * @param message The message content
   * @param title The title
   * @param severity The severity (color)
   * @param key The key of the toast container ("global" for the main one)
   */
  show(message: string, title: string, severity: string, key: string) {
    this.#messageService.add({
      key: key,
      detail: message,
      severity: severity,
      summary: title,
    });
  }

  /**
   * Convenience method for showing a success toast.
   * @param message The message content
   * @param title The title (optional)
   */
  success(message: string, title: string = "Success") {
    this.show(message, title, "success", "global");
  }

  /**
   * Convenience method for showing an info toast.
   * @param message The message content
   * @param title The title (optional)
   */
  info(message: string, title: string = "Info") {
    this.show(message, title, "info", "global");
  }

  /**
   * Convenience method for showing a error toast.
   * @param message The message content
   * @param title The title (optional)
   */
  error(message: string, title: string = "Error") {
    this.show(message, title, "error", "global");
  }

  /**
   * Convenience method for showing a warning toast.
   * @param message The message content
   * @param title The title (optional)
   */
  warn(message: string, title: string = "Warning") {
    this.show(message, title, "warn", "global");
  }
}
