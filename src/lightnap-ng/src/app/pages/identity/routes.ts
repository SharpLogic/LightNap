import { AppRoute } from "@core";

export const Routes: AppRoute[] = [
  { path: "login", title: "Log In", data: { alias: "login" }, loadComponent: () => import("./login/login.component").then(m => m.LoginComponent) },
  {
    path: "magic-link-sent",
    title: "Magic Link Sent",
    data: { alias: "magic-link-sent" },
    loadComponent: () => import("./magic-link-sent/magic-link-sent.component").then(m => m.MagicLinkSentComponent),
  },
  {
    path: "magic-link-login/:email/:code",
    title: "Magic Link Landing",
    loadComponent: () => import("./magic-link-login/magic-link-login.component").then(m => m.MagicLinkLoginComponent),
  },
  {
    path: "reset-password",
    title: "Reset Your Password",
    data: { alias: "reset-password" },
    loadComponent: () => import("./reset-password/reset-password.component").then(m => m.ResetPasswordComponent),
  },
  {
    path: "reset-instructions-sent",
    title: "Reset Instructions Sent",
    data: { alias: "reset-instructions-sent" },
    loadComponent: () => import("./reset-instructions-sent/reset-instructions-sent.component").then(m => m.ResetInstructionsSentComponent),
  },
  {
    path: "register",
    title: "Register",
    data: { alias: "register" },
    loadComponent: () => import("./register/register.component").then(m => m.RegisterComponent),
  },
  {
    path: "new-password/:email/:token",
    loadComponent: () => import("./new-password/new-password.component").then(m => m.NewPasswordComponent),
  },
  {
    path: "verify-code/:login",
    title: "Verify Code",
    data: { alias: "verify-code" },
    loadComponent: () => import("./verify-code/verify-code.component").then(m => m.VerifyCodeComponent),
  },
  {
    path: "email-verification-required",
    title: "Email Verification Required",
    data: { alias: "email-verification-required" },
    loadComponent: () =>
      import("./email-verification-required/email-verification-required.component").then(m => m.EmailVerificationRequiredComponent),
  },
  {
    path: "request-verification-email",
    title: "Request Verification Email",
    data: { alias: "request-verification-email" },
    loadComponent: () => import("./request-verification-email/request-verification-email.component").then(m => m.RequestVerificationEmailComponent),
  },
  {
    path: "confirm-email/:email/:code",
    title: "Confirm Your Email",
    loadComponent: () => import("./confirm-email/confirm-email.component").then(m => m.ConfirmEmailComponent),
  },
  {
    path: "external-login",
    title: "External Logins",
    data: { alias: "external-logins" },
    loadComponent: () => import("./external-login/external-login.component").then(m => m.ExternalLoginComponent),
  },
  {
    path: "external-login-error",
    title: "External Login Error",
    loadComponent: () => import("./external-login-error/external-login-error.component").then(m => m.ExternalLoginErrorComponent),
  },
  {
    path: "external-login-register",
    title: "External Login Registration",
    loadComponent: () => import("./external-login-register/external-login-register.component").then(m => m.ExternalLoginRegisterComponent),
  },
  {
    path: "external-login-complete/:token",
    title: "External Login Completion",
    data: { alias: "external-login-complete" },
    loadComponent: () => import("./external-login-complete/external-login-complete.component").then(m => m.ExternalLoginCompleteComponent),
  },
];
