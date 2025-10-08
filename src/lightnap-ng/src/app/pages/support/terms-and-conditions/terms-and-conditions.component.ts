import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { BrandedCardComponent } from '@core';

@Component({
    standalone: true,
    templateUrl: './terms-and-conditions.component.html',
    imports: [RouterModule, BrandedCardComponent]
})
export class TermsAndConditionsComponent {
}
