
import { Component, Input, signal } from "@angular/core";
import { ListItem } from "@core";

@Component({
    selector: 'dropdown-list-item',
    templateUrl: './dropdown-list-item.component.html',
    imports: [],
    standalone: true,
})
export class DropdownListItemComponent {
    @Input() label = signal("");
    @Input() description = signal<string | undefined>("");

    @Input() set listItem(value: ListItem<any>) {
        this.label.set(value.label);
        this.description.set(value.description);
    }
}
