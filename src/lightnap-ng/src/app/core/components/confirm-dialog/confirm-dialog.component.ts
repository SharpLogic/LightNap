import { Component, ElementRef, TemplateRef, input } from "@angular/core";
import { PrimeIcons } from "primeng/api";
import { ButtonModule, ButtonSeverity } from "primeng/button";
import { ConfirmDialogModule } from "primeng/confirmdialog";

@Component({
  selector: "ln-confirm-dialog",
  templateUrl: "./confirm-dialog.component.html",
  imports: [ConfirmDialogModule, ButtonModule],
  standalone: true,
})
export class ConfirmDialogComponent {
    readonly confirmText = input("Confirm");
    readonly confirmSeverity = input<ButtonSeverity>("danger");
    readonly confirmIcon = input(PrimeIcons.TRASH);
    readonly rejectText = input("Cancel");
    readonly rejectSeverity = input<ButtonSeverity>("secondary");
    readonly rejectIcon = input(PrimeIcons.UNDO);
    readonly key = input("");
    readonly appendTo = input<HTMLElement | ElementRef | TemplateRef<any> | string | null | undefined | any>(undefined);
}
