import { ComponentFixture, TestBed } from "@angular/core/testing";
import { provideHttpClient } from "@angular/common/http";
import { provideNoopAnimations } from "@angular/platform-browser/animations";
import { provideRouter } from "@angular/router";
import { provideZonelessChangeDetection } from "@angular/core";
import { of, throwError } from "rxjs";
import { NotificationsComponent } from "./notifications.component";
import { IdentityService } from "@core/services/identity.service";
import { API_URL_ROOT, APP_NAME, NotificationItem, NotificationSearchResults } from "@core";
import { NotificationService } from "@core/features/notifications/services/notification.service";
import { ToastService } from "@core/services/toast.service";
import { Router } from "@angular/router";

describe("NotificationsComponent", () => {
  let component: NotificationsComponent;
  let fixture: ComponentFixture<NotificationsComponent>;
  let mockIdentityService: jasmine.SpyObj<IdentityService>;
  let mockNotificationService: jasmine.SpyObj<NotificationService>;
  let mockToastService: jasmine.SpyObj<ToastService>;
  let mockRouter: jasmine.SpyObj<Router>;

  const mockNotifications: NotificationItem[] = [
    {
      id: 1,
      timestamp: new Date("2025-11-01T10:00:00Z"),
      isUnread: true,
      title: "Test Notification 1",
      description: "This is a test notification",
      routerLink: ["/test", "1"],
    },
    {
      id: 2,
      timestamp: new Date("2025-11-02T15:30:00Z"),
      isUnread: false,
      title: "Test Notification 2",
      description: "This is another test notification",
      routerLink: ["/test", "2"],
    },
  ];

  const mockSearchResults: NotificationSearchResults = {
    data: [], // This will be populated by the service
    totalCount: 2,
    pageNumber: 1,
    pageSize: 10,
    totalPages: 1,
    unreadCount: 1,
    notifications: mockNotifications,
  };

  beforeEach(async () => {
    mockIdentityService = jasmine.createSpyObj("IdentityService", ["watchLoggedIn$"]);
    mockNotificationService = jasmine.createSpyObj("NotificationService", [
      "searchNotifications",
      "markNotificationAsRead",
      "markAllNotificationsAsRead",
    ]);
    mockToastService = jasmine.createSpyObj("ToastService", ["success"]);
    mockRouter = jasmine.createSpyObj("Router", ["navigate"]);

    mockIdentityService.watchLoggedIn$.and.returnValue(of(true));
    mockNotificationService.searchNotifications.and.returnValue(of(mockSearchResults));
    mockNotificationService.markNotificationAsRead.and.returnValue(of(true));
    mockNotificationService.markAllNotificationsAsRead.and.returnValue(of(true));

    await TestBed.configureTestingModule({
      imports: [NotificationsComponent],
      providers: [
        provideHttpClient(),
        provideNoopAnimations(),
        provideRouter([]),
        provideZonelessChangeDetection(),
        { provide: API_URL_ROOT, useValue: "http://localhost:5000/api/" },
        { provide: APP_NAME, useValue: "TestApp" },
        { provide: IdentityService, useValue: mockIdentityService },
        { provide: NotificationService, useValue: mockNotificationService },
        { provide: ToastService, useValue: mockToastService },
        { provide: Router, useValue: mockRouter },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(NotificationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // Component Initialization Tests
  describe("Component Initialization", () => {
    it("should create", () => {
      expect(component).toBeTruthy();
    });

    it("should initialize errors signal as empty array", () => {
      expect(component.errors()).toEqual([]);
    });

    it("should have pageSize set to 10", () => {
      expect(component.pageSize).toBe(10);
    });

    it("should have type helper functions", () => {
      expect(component.asNotifications).toBeDefined();
      expect(component.asNotification).toBeDefined();
    });

    it("should initialize currentPage to 0", () => {
      // Test that the first lazy load call uses pageNumber 1 (which corresponds to currentPage = 0 initially)
      const event = { first: 0 };
      component.onLazyLoad(event);
      expect(mockNotificationService.searchNotifications).toHaveBeenCalledWith({
        pageSize: 10,
        pageNumber: 1,
      });
    });
  });

  // Lazy Loading Tests
  describe("Lazy Loading", () => {
    it("should call searchNotifications on lazy load", () => {
      const event = { first: 0 };

      component.onLazyLoad(event);

      expect(mockNotificationService.searchNotifications).toHaveBeenCalledWith({
        pageSize: 10,
        pageNumber: 1,
      });
    });

    it("should calculate correct page number for lazy load", () => {
      const event = { first: 20 }; // Second page (pageSize = 10)

      component.onLazyLoad(event);

      expect(mockNotificationService.searchNotifications).toHaveBeenCalledWith({
        pageSize: 10,
        pageNumber: 3, // (20 / 10) + 1 = 3
      });
    });

    it("should handle first = 0 correctly", () => {
      const event = { first: 0 };

      component.onLazyLoad(event);

      expect(mockNotificationService.searchNotifications).toHaveBeenCalledWith({
        pageSize: 10,
        pageNumber: 1,
      });
    });
  });

  // Notification Interaction Tests
  describe("Notification Interaction", () => {
    it("should mark notification as read and navigate when clicked", () => {
      const notification = mockNotifications[0];

      component.notificationClicked(notification);

      expect(mockNotificationService.markNotificationAsRead).toHaveBeenCalledWith(notification.id);
      expect(mockRouter.navigate).toHaveBeenCalledWith(notification.routerLink);
    });

    it("should mark all notifications as read successfully", () => {
      component.markAllAsRead();

      expect(mockNotificationService.markAllNotificationsAsRead).toHaveBeenCalled();
      expect(mockToastService.success).toHaveBeenCalledWith("All notifications marked as read.");
    });

    it("should reload notifications after marking all as read", () => {
      const initialCallCount = mockNotificationService.searchNotifications.calls.count();

      component.markAllAsRead();

      expect(mockNotificationService.searchNotifications.calls.count()).toBe(initialCallCount + 1);
      expect(mockNotificationService.searchNotifications).toHaveBeenCalledWith({
        pageSize: 10,
        pageNumber: 1,
      });
    });

    it("should set errors on mark all as read failure", () => {
      const errorResponse = { errorMessages: ["Failed to mark notifications as read"] };

      mockNotificationService.markAllNotificationsAsRead.and.returnValue(throwError(() => errorResponse));

      component.markAllAsRead();

      expect(component.errors()).toEqual(["Failed to mark notifications as read"]);
    });
  });

  // DOM Rendering Tests
  describe("DOM Rendering", () => {
    it("should render panel component", () => {
      const panel = fixture.nativeElement.querySelector("p-panel");
      expect(panel).toBeTruthy();
    });

    it("should render table component", () => {
      const table = fixture.nativeElement.querySelector("p-table");
      expect(table).toBeTruthy();
    });

    it("should render error list component", () => {
      const errorList = fixture.nativeElement.querySelector("ln-error-list");
      expect(errorList).toBeTruthy();
    });

    it("should render api-response component", () => {
      const apiResponse = fixture.nativeElement.querySelector("ln-api-response");
      expect(apiResponse).toBeTruthy();
    });

    it("should render notification item components when data is loaded", () => {
      // Initially no notifications are rendered (starts with empty response)
      let notificationItems = fixture.nativeElement.querySelectorAll("ln-notification-item");
      expect(notificationItems.length).toBe(0);

      // Trigger lazy load to load notifications
      component.onLazyLoad({ first: 0 });
      fixture.detectChanges();

      // After lazy load, notifications should be rendered
      notificationItems = fixture.nativeElement.querySelectorAll("ln-notification-item");
      expect(notificationItems.length).toBeGreaterThan(0);
    });
  });

  // Data Display Tests
  describe("Data Display", () => {
    it("should use type helper to cast to notification search results", () => {
      const result = component.asNotifications(mockSearchResults);
      expect(result).toEqual(mockSearchResults);
    });

    it("should use type helper to cast to single notification", () => {
      const result = component.asNotification(mockNotifications[0]);
      expect(result).toEqual(mockNotifications[0]);
    });
  });

  // Error Handling Tests
  describe("Error Handling", () => {
    it("should display errors when present", () => {
      component.errors.set(["Test error"]);
      fixture.detectChanges();

      expect(component.errors()).toContain("Test error");
    });

    it("should handle multiple error messages", () => {
      const errors = ["Error 1", "Error 2", "Error 3"];
      component.errors.set(errors);
      fixture.detectChanges();

      expect(component.errors().length).toBe(3);
      expect(component.errors()).toEqual(errors);
    });

    it("should clear previous errors", () => {
      component.errors.set(["Previous error"]);
      expect(component.errors().length).toBe(1);

      component.errors.set([]);
      expect(component.errors().length).toBe(0);
    });
  });
});