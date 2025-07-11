import { Component, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { LayoutService } from '@core/layout/services/layout.service';

@Component({
    standalone: true,
    templateUrl: './terms-and-conditions.component.html',
    imports: [RouterModule]
})
export class TermsAndConditionsComponent {
    layoutService = inject(LayoutService);
}
