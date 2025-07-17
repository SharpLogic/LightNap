import { AppRoute } from "../../core/routing/models/app-route";

export const Routes: AppRoute[] = [
  {
    path: "",
    title: "Profile | Home",
    data: { alias: "profile" },
    loadComponent: () => import("./index/index.component").then(m => m.IndexComponent),
  },
  {
    path: "devices",
    title: "Profile | Devices",
    data: { alias: "devices" },
    loadComponent: () => import("./devices/devices.component").then(m => m.DevicesComponent),
  },
  {
    path: "notifications",
    title: "Profile | Notifications",
    data: { alias: "notifications" },
    loadComponent: () => import("./notifications/notifications.component").then(m => m.NotificationsComponent),
  },
  {
    path: "change-password",
    title: "Profile | Change Password",
    data: { alias: "change-password" },
    loadComponent: () => import("./change-password/change-password.component").then(m => m.ChangePasswordComponent),
  },
  {
    path: "change-email",
    title: "Profile | Change Email",
    data: { alias: "change-email" },
    loadComponent: () => import("./change-email/change-email.component").then(m => m.ChangeEmailComponent),
  },
  {
    path: "change-email-requested",
    title: "Profile | Change Email Requested",
    data: { alias: "change-email-requested" },
    loadComponent: () => import("./change-email-requested/change-email-requested.component").then(m => m.ChangeEmailRequestedComponent),
  },
  {
    path: "confirm-email-change/:newEmail/:code",
    title: "Profile | Confirm Change Email",
    loadComponent: () => import("./confirm-email-change/confirm-email-change.component").then(m => m.ConfirmEmailChangeComponent),
  },
];
