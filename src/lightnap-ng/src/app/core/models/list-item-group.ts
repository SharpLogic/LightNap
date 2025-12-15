import { ListItem } from "./list-item";

/**
 * Represents a group of list items with a label. This helps with PrimeNG collection controls using groups.
 *
 * @template T - The type of the items in the list.
 */
export class ListItemGroup<T> {
  /**
   * Creates an instance of ListItemGroup.
   *
   * @param label - The label for the group of list items.
   * @param listItems - An array of list items of type T.
   */
  constructor(
    public label: string,
    public listItems: Array<ListItem<T>>
  ) {}
}
