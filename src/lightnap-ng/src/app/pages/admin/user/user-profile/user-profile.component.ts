import { CommonModule } from "@angular/common";
import { Component, inject, input, OnChanges, output } from "@angular/core";
import { FormBuilder, ReactiveFormsModule } from "@angular/forms";
import { AdminUserDto } from "@core";
import { ButtonModule } from "primeng/button";

@Component({
  standalone: true,
  selector: "user-profile",
  templateUrl: "./user-profile.component.html",
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ButtonModule,
  ],
})
export class UserProfileComponent implements OnChanges {
  readonly #fb = inject(FormBuilder);

  readonly user = input.required<AdminUserDto>();
  readonly updateProfile = output<any>();

  readonly form = this.#fb.group({
  });

  ngOnChanges() {
  }

}
