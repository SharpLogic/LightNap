import { StaticContentTypes } from "@core/backend-api/static-content-types";
import { UpdateStaticContentDto } from "./update-static-content-dto";

/**
 * Represents a request to create a new static content.
 */
export interface CreateStaticContentDto extends UpdateStaticContentDto {
    type: StaticContentTypes;
}
