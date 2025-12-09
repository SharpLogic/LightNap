// src/api/transformer-unwrap-api-response-dto.js

/**
 * Orval input transformer (JavaScript version)
 * Automatically unwraps any schema ending with "ApiResponseDto"
 * by replacing it with its .result property in all success responses.
 */
module.exports = function unwrapApiResponseDto(spec) {
  // Deep clone the spec (structuredClone is available in Node.js ≥ 17)
  const document = structuredClone(spec);

  const schemas = document.components?.schemas;
  if (!schemas) return document;

  // Step 1: Build a map: "MyEndpointApiResponseDto" → its "result" schema
  const unwrapMap = new Map();

  for (const [schemaName, schemaObj] of Object.entries(schemas)) {
    if (
      schemaName.endsWith('ApiResponseDto') &&
      schemaObj?.type === 'object' &&
      schemaObj?.properties?.result
    ) {
      const resultSchema = schemaObj.properties.result;
      unwrapMap.set(schemaName, resultSchema);
    }
  }

  if (unwrapMap.size === 0) return document; // nothing to unwrap

  // Step 2: Recursively replace any $ref pointing to a wrapper with its .result
  function replaceRefs(obj) {
    if (!obj || typeof obj !== 'object') return obj;

    if (obj.$ref) {
      const refName = obj.$ref.split('/').pop();
      if (unwrapMap.has(refName)) {
        // Replace with the inner result schema (and recurse in case it's also a ref)
        return replaceRefs(unwrapMap.get(refName));
      }
    }

    // Handle arrays and objects
    if (Array.isArray(obj)) {
      return obj.map(replaceRefs);
    }

    for (const key in obj) {
      if (Object.hasOwnProperty.call(obj, key)) {
        obj[key] = replaceRefs(obj[key]);
      }
    }

    return obj;
  }

  // Apply transformation to the entire document
  replaceRefs(document);

  return document;
};
