import { inject, Injectable } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { RoleName } from "@core";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { IdentityService } from "@core/services/identity.service";
import { MenuItem } from "primeng/api";
import { BehaviorSubject, combineLatest, debounceTime, Subject, tap } from "rxjs";
import { MenuChangeEvent } from "../models/menu-change-event";

@Injectable({
  providedIn: "root",
})
export class MenuService {
  #routeAlias = inject(RouteAliasService);
  #identityService = inject(IdentityService);

  #menuSource = new Subject<MenuChangeEvent>();
  menuSource$ = this.#menuSource.asObservable();

  #defaultMenuItems = new Array<MenuItem>({
    label: "Home",
    items: [{ label: "Home", icon: "pi pi-fw pi-home", routerLink: this.#routeAlias.getRoute("user-home") }],
  });

  #loggedInMenuItems = new Array<MenuItem>({
    label: "Profile",
    items: [
      { label: "Profile", icon: "pi pi-fw pi-user", routerLink: this.#routeAlias.getRoute("profile") },
      { label: "Devices", icon: "pi pi-fw pi-mobile", routerLink: this.#routeAlias.getRoute("devices") },
      { label: "Change Password", icon: "pi pi-fw pi-lock", routerLink: this.#routeAlias.getRoute("change-password") },
    ],
  });

  #contentMenuItems = new Array<MenuItem>({
    label: "Content",
    items: [{ label: "Manage", icon: "pi pi-fw pi-cog", routerLink: this.#routeAlias.getRoute("manage-content") }],
  });

  #adminMenuItems = new Array<MenuItem>({
    label: "Admin",
    items: [
      { label: "Home", icon: "pi pi-fw pi-home", routerLink: this.#routeAlias.getRoute("admin-home") },
      { label: "Users", icon: "pi pi-fw pi-users", routerLink: this.#routeAlias.getRoute("admin-users") },
      { label: "Roles", icon: "pi pi-fw pi-lock", routerLink: this.#routeAlias.getRoute("admin-roles") },
      { label: "Claims", icon: "pi pi-fw pi-shield", routerLink: this.#routeAlias.getRoute("admin-claims") },
    ],
  });

  #menuItemSubject = new BehaviorSubject<Array<MenuItem>>(this.#defaultMenuItems);

  #isLoggedIn = false;
  #isContentEditorLoggedIn = false;
  #isAdminLoggedIn = false;

  constructor() {
    combineLatest([
      this.#identityService.watchLoggedIn$().pipe(tap(isLoggedIn => (this.#isLoggedIn = isLoggedIn))),
      this.#identityService
        .watchAnyUserRole$([RoleName.Administrator, RoleName.ContentEditor])
        .pipe(tap(isContentEditorLoggedIn => (this.#isContentEditorLoggedIn = isContentEditorLoggedIn))),
      this.#identityService.watchUserRole$(RoleName.Administrator).pipe(tap(isAdminLoggedIn => (this.#isAdminLoggedIn = isAdminLoggedIn))),
    ])
      .pipe(takeUntilDestroyed(), debounceTime(100))
      .subscribe({ next: () => this.#refreshMenuItems() });
  }

  onMenuStateChange(event: MenuChangeEvent) {
    this.#menuSource.next(event);
  }

  #refreshMenuItems() {
    var menuItems = [...this.#defaultMenuItems];

    if (this.#isLoggedIn) {
      menuItems.push(...this.#loggedInMenuItems);
    }

    if (this.#isContentEditorLoggedIn) {
      menuItems.push(...this.#contentMenuItems);
    }

    if (this.#isAdminLoggedIn) {
      menuItems.push(...this.#adminMenuItems);
    }

    this.#menuItemSubject.next(menuItems);
  }

  watchMenuItems$() {
    return this.#menuItemSubject.asObservable();
  }
}
