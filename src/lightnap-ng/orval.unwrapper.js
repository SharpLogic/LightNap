function unwrapResultProperty(spec) {
  const WRAPPER_SCHEMA_NAME = 'ApiResponseDto';
  Object.values(spec.paths).forEach((pathItem) => {
    Object.values(pathItem).forEach((operation) => {
      Object.values(operation.responses || {}).forEach((response) => {
        if (response.content && response.content['application/json']) {
          const schema = response.content['application/json'].schema;

          // Check if the schema references the wrapper
          if (schema.$ref) {
            const refName = schema.$ref.split('/').pop();

            if (refName.endsWith(WRAPPER_SCHEMA_NAME)) {
                console.log(`Unwrapping response schema for reference: ${refName}`);
              // Find the original wrapper schema definition
              const wrapperSchema = spec.components.schemas[refName];

              if (wrapperSchema && wrapperSchema.properties && wrapperSchema.properties.result) {
                console.log(` - Found wrapper schema. Unwrapping 'result' property: ${JSON.stringify(wrapperSchema.properties.result)}`);
                // IMPORTANT: Overwrite the *response schema* with the schema of the inner 'result' property
                Object.assign(schema, wrapperSchema.properties.result);
                delete schema.$ref; // Remove the old $ref to avoid conflicts
              }
            }
          }
        }
      });
    });
  });

  return spec;
}

module.exports = unwrapResultProperty;
