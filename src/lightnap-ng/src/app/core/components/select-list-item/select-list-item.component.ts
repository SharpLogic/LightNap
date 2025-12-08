
import { Component, Input, signal } from "@angular/core";
import { ListItem } from "@core/models/list-item";

@Component({
    selector: 'ln-select-list-item',
    templateUrl: './select-list-item.component.html',
    imports: [],
})
export class SelectListItemComponent {
    @Input() label = signal("");
    @Input() description = signal<string | undefined>("");

    @Input() set listItem(value: ListItem<any>) {
        this.label.set(value.label);
        this.description.set(value.description);
    }
}
