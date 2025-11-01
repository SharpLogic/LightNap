import { ListItem } from "@core/models/list-item";

export type SearchUsersSortBy = "email" | "userName" | "createdDate" | "lastModifiedDate";

export const SearchUsersSortBy = {
  Email: "email",
  UserName: "userName",
  CreatedDate: "createdDate",
  LastModifiedDate: "lastModifiedDate",
} as const;

export const SearchUsersSortByListItems = [
  new ListItem<SearchUsersSortBy>(SearchUsersSortBy.UserName, "User Name", "Sort by user name."),
  new ListItem<SearchUsersSortBy>(SearchUsersSortBy.Email, "Email", "Sort by email."),
  new ListItem<SearchUsersSortBy>(SearchUsersSortBy.CreatedDate, "Created", "Sort by created date."),
  new ListItem<SearchUsersSortBy>(SearchUsersSortBy.LastModifiedDate, "Last Modified", "Sort by last modified date."),
];
