import { Component } from '@angular/core';
import { ZoneComponent } from '@core/components/zone/zone.component';

@Component({
    selector: 'app-home-index',
    standalone: true,
    templateUrl: './index.component.html',
    imports: [ZoneComponent]
})
export class IndexComponent {
}
