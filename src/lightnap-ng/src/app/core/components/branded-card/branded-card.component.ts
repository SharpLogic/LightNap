import { CommonModule } from "@angular/common";
import { Component, inject } from "@angular/core";
import { LayoutService } from "@core/layout/services/layout.service";

@Component({
  selector: 'ln-branded-card',
  standalone: true,
  templateUrl: './branded-card.component.html',
  imports: [CommonModule],
})
export class BrandedCardComponent {
  layoutService = inject(LayoutService);
}
