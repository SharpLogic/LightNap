/*
 * Static content types used for sorting static contents.
 */
export type StaticContentSortBys = "Key" | "Status" | "Type" | "CreatedDate" | "LastModifiedDate";

export const StaticContentSortBy = {
    Key: "Key",
    Status: "Status",
    Type: "Type",
    CreatedDate: "CreatedDate",
    LastModifiedDate: "LastModifiedDate",
} as const;
