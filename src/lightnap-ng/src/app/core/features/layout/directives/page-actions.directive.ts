import { AfterViewInit, Directive, OnDestroy, TemplateRef, inject } from "@angular/core";
import { LayoutService } from "../services/layout.service";

/**
 * Marks the child template as the current page's actions slot. AppLayoutComponent renders
 * the template (a) inline under the breadcrumb on small screens, (b) in a fixed floating
 * panel top-right on md+ screens.
 */
@Directive({
  selector: "[pageActions]",
})
export class PageActionsDirective implements AfterViewInit, OnDestroy {
  #layoutService = inject(LayoutService);
  #templateRef = inject(TemplateRef<any>);

  ngAfterViewInit() {
    this.#layoutService.pageActionsTemplate.set(this.#templateRef);
  }

  ngOnDestroy() {
    if (this.#layoutService.pageActionsTemplate() === this.#templateRef) {
      this.#layoutService.pageActionsTemplate.set(null);
    }
  }
}
