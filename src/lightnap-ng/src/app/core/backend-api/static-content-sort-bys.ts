/*
 * Static content types used for sorting static contents.
 */
export type StaticContentSortBys = "Key" | "Status" | "Type" | "ReadAccess" | "CreatedDate" | "LastModifiedDate";

export const StaticContentSortBy = {
    Key: "Key",
    Status: "Status",
    Type: "Type",
    ReadAccess: "ReadAccess",
    CreatedDate: "CreatedDate",
    LastModifiedDate: "LastModifiedDate",
} as const;
