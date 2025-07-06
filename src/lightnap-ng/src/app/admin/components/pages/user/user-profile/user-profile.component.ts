import { AdminUserDto } from "@admin/models";
import { CommonModule } from "@angular/common";
import { Component, inject, input, OnChanges, output } from "@angular/core";
import { FormBuilder, ReactiveFormsModule } from "@angular/forms";
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
  #fb = inject(FormBuilder);

  user = input.required<AdminUserDto>();
  updateProfile = output<any>();

  form = this.#fb.group({
  });

  ngOnChanges() {
  }

}
