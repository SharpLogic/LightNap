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
   * @param clickHandler Optional click handler for the toast
   */
  show(message: string, title: string, severity: string, key: string, clickHandler?: () => void) {
    this.#messageService.add({
      key: key,
      detail: message,
      severity: severity,
      summary: title,
      data: { clickHandler },
    });
  }

  /**
   * Convenience method for showing a success toast.
   * @param message The message content
   * @param title The title (optional)
   * @param clickHandler Optional click handler for the toast
   */
  success(message: string, title: string = "Success", clickHandler?: () => void) {
    this.show(message, title, "success", "global", clickHandler);
  }

  /**
   * Convenience method for showing an info toast.
   * @param message The message content
   * @param title The title (optional)
   * @param clickHandler Optional click handler for the toast
   */
  info(message: string, title: string = "Info", clickHandler?: () => void) {
    this.show(message, title, "info", "global", clickHandler);
  }

  /**
   * Convenience method for showing a error toast.
   * @param message The message content
   * @param title The title (optional)
   * @param clickHandler Optional click handler for the toast
   */
  error(message: string, title: string = "Error", clickHandler?: () => void) {
    this.show(message, title, "error", "global", clickHandler);
  }

  /**
   * Convenience method for showing a warning toast.
   * @param message The message content
   * @param title The title (optional)
   * @param clickHandler Optional click handler for the toast
   */
  warn(message: string, title: string = "Warning", clickHandler?: () => void) {
    this.show(message, title, "warn", "global", clickHandler);
  }
}
