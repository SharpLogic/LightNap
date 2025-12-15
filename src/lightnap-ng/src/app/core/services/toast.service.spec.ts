import type { MockedObject } from "vitest";
import { provideZonelessChangeDetection } from "@angular/core";
import { TestBed } from "@angular/core/testing";
import { MessageService } from "primeng/api";
import { ToastService } from "./toast.service";

describe("ToastService", () => {
  let service: ToastService;
  let messageServiceSpy: MockedObject<MessageService>;

  beforeEach(() => {
    const messageSpy = {
      add: vi.fn().mockName("MessageService.add"),
    };

    TestBed.configureTestingModule({
      providers: [provideZonelessChangeDetection(), ToastService, { provide: MessageService, useValue: messageSpy }],
    });

    service = TestBed.inject(ToastService);
    messageServiceSpy = TestBed.inject(MessageService) as MockedObject<MessageService>;
  });

  it("should be created", () => {
    expect(service).toBeTruthy();
  });

  describe("toast notifications", () => {
    it("should call MessageService.add with correct parameters for success", () => {
      const message = "Success message";
      const title = "Success";

      service.success(message);

      expect(messageServiceSpy.add).toHaveBeenCalledWith({
        key: "global",
        detail: message,
        severity: "success",
        summary: title,
      });
    });

    it("should call MessageService.add with correct parameters for info", () => {
      const message = "Info message";
      const title = "Info";

      service.info(message);

      expect(messageServiceSpy.add).toHaveBeenCalledWith({
        key: "global",
        detail: message,
        severity: "info",
        summary: title,
      });
    });

    it("should call MessageService.add with correct parameters for error", () => {
      const message = "Error message";
      const title = "Error";

      service.error(message);

      expect(messageServiceSpy.add).toHaveBeenCalledWith({
        key: "global",
        detail: message,
        severity: "error",
        summary: title,
      });
    });

    it("should call MessageService.add with correct parameters for warn", () => {
      const message = "Warning message";
      const title = "Warning";

      service.warn(message);

      expect(messageServiceSpy.add).toHaveBeenCalledWith({
        key: "global",
        detail: message,
        severity: "warn",
        summary: title,
      });
    });

    it("should call MessageService.add with custom key", () => {
      const message = "Custom message";
      const title = "Custom";
      const severity = "info";
      const key = "customKey";

      service.show(message, title, severity, key);

      expect(messageServiceSpy.add).toHaveBeenCalledWith({
        key: key,
        detail: message,
        severity: severity,
        summary: title,
      });
    });
  });
});
