import { CommonModule } from "@angular/common";
import { Component, inject, OnDestroy, Renderer2, viewChild } from "@angular/core";
import { NavigationEnd, Router, RouterOutlet } from "@angular/router";
import { filter, Subscription } from "rxjs";
import { LayoutService } from "src/app/layout/services/layout.service";
import { AppConfigComponent } from "../../controls/app-config/app-config.component";
import { AppFooterComponent } from "../../controls/app-footer/app-footer.component";
import { AppSidebarComponent } from "../../controls/app-sidebar/app-sidebar.component";
import { AppTopBarComponent } from "../../controls/app-top-bar/app-top-bar.component";

@Component({
  standalone: true,
  templateUrl: "./app-layout.component.html",
  imports: [CommonModule, AppSidebarComponent, AppFooterComponent, AppConfigComponent, AppTopBarComponent, RouterOutlet],
})
export class AppLayoutComponent implements OnDestroy {
  readonly layoutService = inject(LayoutService);
  readonly renderer = inject(Renderer2);
  readonly router = inject(Router);

  overlayMenuOpenSubscription: Subscription;
  menuOutsideClickListener: any;
  profileMenuOutsideClickListener: any;

  readonly appSidebar = viewChild.required(AppSidebarComponent);
  readonly appTopbar = viewChild.required(AppTopBarComponent);

  constructor() {
    this.overlayMenuOpenSubscription = this.layoutService.overlayOpen$.subscribe(() => {
      if (!this.menuOutsideClickListener) {
        this.menuOutsideClickListener = this.renderer.listen("document", "click", event => {
          const menuButton = this.appTopbar().menuButton();
          const appSidebar = this.appSidebar();
          const isOutsideClicked = !(
            appSidebar.el.nativeElement.isSameNode(event.target) ||
            appSidebar.el.nativeElement.contains(event.target) ||
            menuButton.nativeElement.isSameNode(event.target) ||
            menuButton.nativeElement.contains(event.target)
          );

          if (isOutsideClicked) {
            this.hideMenu();
          }
        });
      }

      if (!this.profileMenuOutsideClickListener) {
        this.profileMenuOutsideClickListener = this.renderer.listen("document", "click", event => {
          const menu = this.appTopbar().menu();
          const topbarMenuButton = this.appTopbar().topbarMenuButton();
          const isOutsideClicked = !(
            menu.nativeElement.isSameNode(event.target) ||
            menu.nativeElement.contains(event.target) ||
            topbarMenuButton.nativeElement.isSameNode(event.target) ||
            topbarMenuButton.nativeElement.contains(event.target)
          );

          if (isOutsideClicked) {
            this.hideProfileMenu();
          }
        });
      }

      if (this.layoutService.state.staticMenuMobileActive) {
        this.blockBodyScroll();
      }
    });

    this.router.events.pipe(filter(event => event instanceof NavigationEnd)).subscribe(() => {
      this.hideMenu();
      this.hideProfileMenu();
    });
  }

  hideMenu() {
    this.layoutService.state.overlayMenuActive = false;
    this.layoutService.state.staticMenuMobileActive = false;
    this.layoutService.state.menuHoverActive = false;
    if (this.menuOutsideClickListener) {
      this.menuOutsideClickListener();
      this.menuOutsideClickListener = null;
    }
    this.unblockBodyScroll();
  }

  hideProfileMenu() {
    this.layoutService.state.profileSidebarVisible = false;
    if (this.profileMenuOutsideClickListener) {
      this.profileMenuOutsideClickListener();
      this.profileMenuOutsideClickListener = null;
    }
  }

  blockBodyScroll(): void {
    if (document.body.classList) {
      document.body.classList.add("blocked-scroll");
    } else {
      document.body.className += " blocked-scroll";
    }
  }

  unblockBodyScroll(): void {
    if (document.body.classList) {
      document.body.classList.remove("blocked-scroll");
    } else {
      document.body.className = document.body.className.replace(new RegExp("(^|\\b)" + "blocked-scroll".split(" ").join("|") + "(\\b|$)", "gi"), " ");
    }
  }

  get containerClass() {
    return {
      "layout-theme-light": this.layoutService.config().colorScheme === "light",
      "layout-theme-dark": this.layoutService.config().colorScheme === "dark",
      "layout-overlay": this.layoutService.config().menuMode === "overlay",
      "layout-static": this.layoutService.config().menuMode === "static",
      "layout-static-inactive": this.layoutService.state.staticMenuDesktopInactive && this.layoutService.config().menuMode === "static",
      "layout-overlay-active": this.layoutService.state.overlayMenuActive,
      "layout-mobile-active": this.layoutService.state.staticMenuMobileActive,
      "p-input-filled": this.layoutService.config().inputStyle === "filled",
      "p-ripple-disabled": !this.layoutService.config().ripple,
    };
  }

  ngOnDestroy() {
    if (this.overlayMenuOpenSubscription) {
      this.overlayMenuOpenSubscription.unsubscribe();
    }

    if (this.menuOutsideClickListener) {
      this.menuOutsideClickListener();
    }
  }
}
