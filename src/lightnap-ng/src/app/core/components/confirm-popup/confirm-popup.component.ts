import { Component, input } from "@angular/core";
import { PrimeIcons } from "primeng/api";
import { ButtonModule, ButtonSeverity } from "primeng/button";
import { ConfirmPopupModule } from "primeng/confirmpopup";

@Component({
  selector: "ln-confirm-popup",
  templateUrl: "./confirm-popup.component.html",
  imports: [ConfirmPopupModule, ButtonModule],
})
export class ConfirmPopupComponent {
  readonly confirmText = input("Confirm");
  readonly confirmSeverity = input<ButtonSeverity>("danger");
  readonly confirmIcon = input(PrimeIcons.TRASH);
  readonly rejectText = input("Cancel");
  readonly rejectSeverity = input<ButtonSeverity>("secondary");
  readonly rejectIcon = input(PrimeIcons.UNDO);
  readonly key = input("");
}
