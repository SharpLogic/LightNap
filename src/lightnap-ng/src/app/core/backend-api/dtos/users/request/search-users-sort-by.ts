export type SearchUsersSortBy = "email" | "userName" | "createdDate" | "lastModifiedDate";

export const SearchUsersSortBy = {
    Email: "email",
    UserName: "userName",
    CreatedDate: "createdDate",
    LastModifiedDate: "lastModifiedDate",
} as const;
