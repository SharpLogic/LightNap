
import { Component, inject } from "@angular/core";
import { LayoutService } from "@core/features/layout/services/layout.service";

@Component({
  selector: 'ln-branded-card',
  templateUrl: './branded-card.component.html',
  imports: [],
})
export class BrandedCardComponent {
  layoutService = inject(LayoutService);
}
