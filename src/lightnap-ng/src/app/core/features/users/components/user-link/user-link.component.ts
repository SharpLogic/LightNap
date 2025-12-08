
import { Component, computed, inject, input } from "@angular/core";
import { PublicUserDto } from "@core/backend-api/dtos/users/response/public-user-dto";
import { TypeHelpers } from "@core/helpers/type-helpers";
import { PublicUsersService } from "@core/features/users/services/public-users.service";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";

@Component({
  selector: "ln-user-link",
  templateUrl: "./user-link.component.html",
  imports: [ApiResponseComponent],
})
export class UserLinkComponent {
  readonly #usersService = inject(PublicUsersService);

  readonly userId = input.required<string>();

  user = computed(() => {
    return this.#usersService.getUser(this.userId());
  });

  readonly asUser = TypeHelpers.cast<PublicUserDto>;
}
