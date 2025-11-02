import { animate, state, style, transition, trigger } from "@angular/animations";
import { CommonModule } from "@angular/common";
import { Component, HostBinding, inject, Input, input, OnInit } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { NavigationEnd, Router, RouterLink, RouterLinkActive } from "@angular/router";
import { LayoutService } from "@core/layout/services/layout.service";
import { MenuService } from "@core/layout/services/menu.service";
import { MenuItem } from "primeng/api";
import { RippleModule } from "primeng/ripple";
import { filter } from "rxjs/operators";

@Component({
  // eslint-disable-next-line @angular-eslint/component-selector
  selector: "[ln-app-menu-item]",
  templateUrl: "./app-menu-item.component.html",
  animations: [
    trigger("children", [
      state(
        "collapsed",
        style({
          height: "0",
        })
      ),
      state(
        "expanded",
        style({
          height: "*",
        })
      ),
      transition("collapsed <=> expanded", animate("400ms cubic-bezier(0.86, 0, 0.07, 1)")),
    ]),
  ],
  imports: [CommonModule, RouterLink, RippleModule, RouterLinkActive],
})
export class AppMenuItemComponent implements OnInit {
  readonly #menuService = inject(MenuService);
  readonly layoutService = inject(LayoutService);
  readonly router = inject(Router);

  readonly item = input.required<MenuItem>();
  readonly index = input.required<number>();
  @Input() @HostBinding("class.layout-root-menuitem") root!: boolean;
  readonly parentKey = input<string>();

  active = false;
  key: string = "";

  constructor() {
    this.#menuService.menuSource$.pipe(takeUntilDestroyed()).subscribe(value => {
      Promise.resolve(null).then(() => {
        if (value.routeEvent) {
          this.active = value.key === this.key || value.key.startsWith(this.key + "-") ? true : false;
        } else {
          if (value.key !== this.key && !value.key.startsWith(this.key + "-")) {
            this.active = false;
          }
        }
      });
    });

    this.router.events.pipe(filter(event => event instanceof NavigationEnd)).subscribe(params => {
      if (this.item().routerLink) {
        this.updateActiveStateFromRoute();
      }
    });
  }

  ngOnInit() {
    const parentKey = this.parentKey();
    this.key = parentKey ? parentKey + "-" + this.index() : String(this.index());

    if (this.item().routerLink) {
      this.updateActiveStateFromRoute();
    }
  }

  updateActiveStateFromRoute() {
    let activeRoute = this.router.isActive(this.item().routerLink[0], {
      paths: "exact",
      queryParams: "ignored",
      matrixParams: "ignored",
      fragment: "ignored",
    });

    if (activeRoute) {
      this.#menuService.onMenuStateChange({ key: this.key, routeEvent: true });
    }
  }

  itemClick(event: Event) {
    // avoid processing disabled items
    if (this.item().disabled) {
      event.preventDefault();
      return;
    }

    // execute command
    const command = this.item().command?.({ originalEvent: event, item: this.item });

    // toggle active state
    if (this.item().items) {
      this.active = !this.active;
    }

    this.#menuService.onMenuStateChange({ key: this.key });
  }

  get submenuAnimation() {
    return this.root ? "expanded" : this.active ? "expanded" : "collapsed";
  }

  @HostBinding("class.active-menuitem")
  get activeClass() {
    return this.active && !this.root;
  }
}
