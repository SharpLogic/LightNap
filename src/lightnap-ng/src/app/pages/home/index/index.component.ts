import { Component } from '@angular/core';
import { PanelModule } from 'primeng/panel';

@Component({
    selector: 'app-home-index',
    standalone: true,
    templateUrl: './index.component.html',
    imports: [PanelModule]
})
export class IndexComponent {
}
