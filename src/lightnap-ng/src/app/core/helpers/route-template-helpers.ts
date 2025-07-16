export class RouteTemplateHelpers {
  /**
   * Process a template string, replacing parameter placeholders with actual parameter values
   * @param template The template string (e.g. "user:view:userId")
   * @param params The route parameters
   * @returns The processed string with parameters replaced
   */
  static processTemplate(template: string, params: any): string {
    return template.replace(/:([\w\.]+)/g, (_, paramName) => params[paramName] ?? `:${paramName}`);
  }
}
