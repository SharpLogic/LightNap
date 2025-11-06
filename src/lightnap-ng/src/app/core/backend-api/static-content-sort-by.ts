/*
 * Static content types used for sorting static contents.
 */
export type StaticContentSortBy = "Key" | "Status" | "Type" | "ReadAccess" | "CreatedDate" | "LastModifiedDate";

export const StaticContentSortBys = {
    Key: "Key",
    Status: "Status",
    Type: "Type",
    ReadAccess: "ReadAccess",
    CreatedDate: "CreatedDate",
    LastModifiedDate: "LastModifiedDate",
} as const;
