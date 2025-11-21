import { CommonModule } from "@angular/common";
import { Component, OnDestroy, Renderer2, ViewChild, inject } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { NavigationEnd, Router, RouterModule } from "@angular/router";
import { LayoutService } from "@core/features/layout/services/layout.service";
import { filter } from "rxjs";
import { AppFooterComponent } from "../../controls/app-footer/app-footer.component";
import { AppSidebarComponent } from "../../controls/app-sidebar/app-sidebar.component";
import { AppTopBarComponent } from "../../controls/app-top-bar/app-top-bar.component";
import { BreadcrumbComponent } from "../../controls/breadcrumb/breadcrumb.component";

@Component({
  selector: "ln-app-layout",
  templateUrl: "./app-layout.component.html",
  imports: [CommonModule, AppTopBarComponent, AppSidebarComponent, RouterModule, AppFooterComponent, BreadcrumbComponent],
})
export class AppLayoutComponent implements OnDestroy {
  #menuOutsideClickListener: any;

  @ViewChild(AppSidebarComponent) appSidebar!: AppSidebarComponent;

  @ViewChild(AppTopBarComponent) appTopBar!: AppTopBarComponent;

  public layoutService = inject(LayoutService);
  public renderer = inject(Renderer2);
  public router = inject(Router);

  // Run these subscriptions during field initialization to ensure they are invoked within an injection context
  // so takeUntilDestroyed() can be used safely.
  readonly #_init = (() => {
    this.layoutService.overlayOpen$.pipe(takeUntilDestroyed()).subscribe({
      next: () => {
        if (!this.#menuOutsideClickListener) {
          this.#menuOutsideClickListener = this.renderer.listen("document", "click", (event: any) => {
            if (this.isOutsideClicked(event)) {
              this.hideMenu();
            }
          });
        }

        if (this.layoutService.layoutState().staticMenuMobileActive) {
          this.blockBodyScroll();
        }
      },
    });

    this.router.events
      .pipe(
        takeUntilDestroyed(),
        filter(event => event instanceof NavigationEnd)
      )
      .subscribe({
        next: () => {
          this.hideMenu();
        },
      });

    return null;
  })();

  isOutsideClicked(event: MouseEvent) {
    const sidebarEl = document.querySelector(".layout-sidebar");
    const topbarEl = document.querySelector(".layout-menu-button");
    const eventTarget = event.target as Node;

    return !(
      sidebarEl?.isSameNode(eventTarget) ||
      sidebarEl?.contains(eventTarget) ||
      topbarEl?.isSameNode(eventTarget) ||
      topbarEl?.contains(eventTarget)
    );
  }

  hideMenu() {
    this.layoutService.layoutState.update((prev: any) => ({ ...prev, overlayMenuActive: false, staticMenuMobileActive: false, menuHoverActive: false }));
    if (this.#menuOutsideClickListener) {
      this.#menuOutsideClickListener();
      this.#menuOutsideClickListener = null;
    }
    this.unblockBodyScroll();
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
      "layout-overlay": this.layoutService.layoutConfig().menuMode === "overlay",
      "layout-static": this.layoutService.layoutConfig().menuMode === "static",
      "layout-static-inactive": this.layoutService.layoutState().staticMenuDesktopInactive && this.layoutService.layoutConfig().menuMode === "static",
      "layout-overlay-active": this.layoutService.layoutState().overlayMenuActive,
      "layout-mobile-active": this.layoutService.layoutState().staticMenuMobileActive,
    };
  }

  ngOnDestroy() {
    if (this.#menuOutsideClickListener) {
      this.#menuOutsideClickListener();
    }
  }
}
