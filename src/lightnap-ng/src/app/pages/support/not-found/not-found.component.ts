import { Component, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { BrandedCardComponent } from '@core';
import { LayoutService } from '@core/layout/services/layout.service';

@Component({
    standalone: true,
    templateUrl: './not-found.component.html',
    imports: [RouterModule, BrandedCardComponent]
})
export class NotFoundComponent {
    layoutService = inject(LayoutService);
}
