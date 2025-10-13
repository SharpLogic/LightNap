import { CommonModule } from "@angular/common";
import { Component, computed, ElementRef, inject, input } from "@angular/core";
import { ApiResponseComponent, PublicUserDto, PublicUsersService, TypeHelpers } from "@core";
import { Observable, of } from "rxjs";

@Component({
  selector: "user-id",
  standalone: true,
  templateUrl: "./user-id.component.html",
  imports: [CommonModule, ApiResponseComponent],
})
export class UserIdComponent {
  readonly #usersService = inject(PublicUsersService);
  readonly #elementRef = inject(ElementRef);

  readonly userName = input.required<string>();

  user = computed(() => {
    // This is a hack to ignore the scenario where we're setting up a custom element
    // and the input binding hasn't happened yet. In that case, just return an empty observable
    // since it won't get used anyway.
    try {
        const un = this.#elementRef.nativeElement.getAttribute("userName");
        if (!un) {
            return of<PublicUserDto>(undefined as any);
        }
      return this.#usersService.getUserByUserName(un);
    } catch {
      return new Observable<PublicUserDto>(undefined as any);
    }
  });

  readonly asUser = TypeHelpers.cast<PublicUserDto>;
}
