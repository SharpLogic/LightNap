import { ListItem } from "@core/models";
import { ApplicationUserSortBy } from "./models";

export const SearchUsersSortByListItems = [
  new ListItem<ApplicationUserSortBy>(ApplicationUserSortBy.UserName, "User Name", "Sort by user name."),
  new ListItem<ApplicationUserSortBy>(ApplicationUserSortBy.Email, "Email", "Sort by email."),
  new ListItem<ApplicationUserSortBy>(ApplicationUserSortBy.CreatedDate, "Created", "Sort by created date."),
  new ListItem<ApplicationUserSortBy>(ApplicationUserSortBy.LastModifiedDate, "Last Modified", "Sort by last modified date."),
];
