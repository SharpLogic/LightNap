import { DestroyRef, Directive, ElementRef, inject, Input, Renderer2, SimpleChanges } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { ClaimDto, IdentityService } from "@identity";
import { Subscription } from "rxjs";

@Directive({
  selector: "[showByPermissions]",
  standalone: true,
})
export class ShowByPermissionsDirective {
  #identityService = inject(IdentityService);
  #el = inject(ElementRef);
  #destroyRef = inject(DestroyRef);
  #renderer = inject(Renderer2);
  #subscription?: Subscription;
  #originalDisplay = this.#el.nativeElement.style.display;

  @Input() claims?: Array<ClaimDto> | ClaimDto;
  @Input() roles?: Array<string> | string;

  ngOnChanges(changes: SimpleChanges) {
    if (changes["claims"] || changes["roles"]) {
      if (this.#subscription) this.#subscription.unsubscribe();

      const claims = this.claims ? (Array.isArray(this.claims) ? this.claims : [this.claims]) : [];
      const roles = this.roles ? (Array.isArray(this.roles) ? this.roles : [this.roles]) : [];

      this.#subscription = this.#identityService
        .watchUserPermission$(roles, claims)
        .pipe(takeUntilDestroyed(this.#destroyRef))
        .subscribe({
          next: hasPermission => {
            if (hasPermission) {
              if (this.#originalDisplay?.length) {
                this.#renderer.setStyle(this.#el.nativeElement, "display", this.#originalDisplay);
              } else {
                this.#renderer.removeStyle(this.#el.nativeElement, "display");
              }
            } else {
              this.#renderer.setStyle(this.#el.nativeElement, "display", "none");
            }
          },
        });
    }
  }
}
