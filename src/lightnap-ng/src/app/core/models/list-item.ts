/**
 * Represents an item in a list to make it easier to work with PrimeNG collection controls.
 *
 * @template T - The type of the value.
 */
export class ListItem<T> {
  /**
   * Creates an instance of ListItem.
   *
   * @param value - The value of the list item.
   * @param label - The label of the list item.
   * @param description - An optional description of the list item.
   */
  constructor(
    public value: T,
    public label: string,
    public description?: string
  ) {}
}
