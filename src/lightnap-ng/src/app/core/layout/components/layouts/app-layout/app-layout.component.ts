import { CommonModule } from "@angular/common";
import { Component, Renderer2, ViewChild } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { NavigationEnd, Router, RouterModule } from "@angular/router";
import { LayoutService } from "@core/layout/services/layout.service";
import { filter } from "rxjs";
import { AppFooterComponent } from "../../controls/app-footer/app-footer.component";
import { AppSidebarComponent } from "../../controls/app-sidebar/app-sidebar.component";
import { AppTopBarComponent } from "../../controls/app-top-bar/app-top-bar.component";

@Component({
  selector: "app-layout",
  templateUrl: "./app-layout.component.html",
  imports: [CommonModule, AppTopBarComponent, AppSidebarComponent, RouterModule, AppFooterComponent],
})
export class AppLayoutComponent {
  menuOutsideClickListener: any;

  @ViewChild(AppSidebarComponent) appSidebar!: AppSidebarComponent;

  @ViewChild(AppTopBarComponent) appTopBar!: AppTopBarComponent;

  constructor(
    public layoutService: LayoutService,
    public renderer: Renderer2,
    public router: Router
  ) {
    this.layoutService.overlayOpen$.pipe(takeUntilDestroyed()).subscribe(() => {
      if (!this.menuOutsideClickListener) {
        this.menuOutsideClickListener = this.renderer.listen("document", "click", event => {
          if (this.isOutsideClicked(event)) {
            this.hideMenu();
          }
        });
      }

      if (this.layoutService.layoutState().staticMenuMobileActive) {
        this.blockBodyScroll();
      }
    });

    this.router.events.pipe(takeUntilDestroyed(), filter(event => event instanceof NavigationEnd)).subscribe(() => {
      this.hideMenu();
    });
  }

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
    this.layoutService.layoutState.update(prev => ({ ...prev, overlayMenuActive: false, staticMenuMobileActive: false, menuHoverActive: false }));
    if (this.menuOutsideClickListener) {
      this.menuOutsideClickListener();
      this.menuOutsideClickListener = null;
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
    if (this.menuOutsideClickListener) {
      this.menuOutsideClickListener();
    }
  }
}
