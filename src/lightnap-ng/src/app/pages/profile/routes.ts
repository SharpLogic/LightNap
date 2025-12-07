import { AppRoute } from "@core";

export const Routes: AppRoute[] = [
  {
    path: "",
    title: "Profile | Home",
    data: { alias: "profile", breadcrumb: "" },
    loadComponent: () => import("./index/index.component").then(m => m.IndexComponent),
  },
  {
    path: "devices",
    title: "Profile | Devices",
    data: { alias: "my-devices", breadcrumb: "Devices" },
    loadComponent: () => import("./devices/devices.component").then(m => m.DevicesComponent),
  },
  {
    path: "notifications",
    title: "Profile | Notifications",
    data: { alias: "my-notifications", breadcrumb: "Notifications" },
    loadComponent: () => import("./notifications/notifications.component").then(m => m.NotificationsComponent),
  },
  {
    path: "external-logins",
    title: "Profile | External Logins",
    data: { alias: "my-external-logins", breadcrumb: "External Logins" },
    loadComponent: () => import("./external-logins/external-logins.component").then(m => m.ExternalLoginsComponent),
  },
  {
    path: "change-password",
    title: "Profile | Change Password",
    data: { alias: "change-password", breadcrumb: "Change Password" },
    loadComponent: () => import("./change-password/change-password.component").then(m => m.ChangePasswordComponent),
  },
  {
    path: "change-email",
    title: "Profile | Change Email",
    data: { alias: "change-email", breadcrumb: "Change Email" },
    loadComponent: () => import("./change-email/change-email.component").then(m => m.ChangeEmailComponent),
  },
  {
    path: "change-email-requested",
    title: "Profile | Change Email Requested",
    data: { alias: "change-email-requested", breadcrumb: "Email Change Requested" },
    loadComponent: () => import("./change-email-requested/change-email-requested.component").then(m => m.ChangeEmailRequestedComponent),
  },
  {
    path: "confirm-email-change/:newEmail/:code",
    title: "Profile | Confirm Change Email",
    data: { breadcrumb: "Confirm Email Change" },
    loadComponent: () => import("./confirm-email-change/confirm-email-change.component").then(m => m.ConfirmEmailChangeComponent),
  },
];
