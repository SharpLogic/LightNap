import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { RoutePipe } from '@routing';
import { LayoutService } from '@core/layout/services/layout.service';

@Component({
    standalone: true,
    templateUrl: './error.component.html',
    imports: [RouterLink, RoutePipe]
})
export class ErrorComponent {
    layoutService = inject(LayoutService);
}
