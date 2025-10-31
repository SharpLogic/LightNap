import { CommonModule } from "@angular/common";
import { Component, computed, inject, input } from "@angular/core";
import { ApiResponseComponent, PublicUserDto, PublicUsersService, TypeHelpers } from "@core";

@Component({
  selector: "user-link",
  standalone: true,
  templateUrl: "./user-link.component.html",
  imports: [CommonModule, ApiResponseComponent],
})
export class UserLinkComponent {
  readonly #usersService = inject(PublicUsersService);

  readonly userId = input.required<string>();

  user = computed(() => {
    return this.#usersService.getUser(this.userId());
});

  readonly asUser = TypeHelpers.cast<PublicUserDto>;
}
