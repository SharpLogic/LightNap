import { CommonModule } from "@angular/common";
import { Component, computed, inject, input } from "@angular/core";
import { PublicUserDto } from "@core/backend-api/dtos/users/response/public-user-dto";
import { TypeHelpers } from "@core/helpers/type-helpers";
import { PublicUsersService } from "@core/users/services/public-users.service";
import { ApiResponseComponent } from "../api-response/api-response.component";

@Component({
  selector: "ln-user-link",
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
