import { RouteTemplateHelpers } from "./route-template-helpers";

describe("RouteTemplateHelpers", () => {
  describe("processTemplate", () => {
    it("should replace a single parameter", () => {
      const template = "user:view::userId";
      const params = { userId: 42 };
      expect(RouteTemplateHelpers.processTemplate(template, params)).toBe("user:view:42");
    });

    it("should replace multiple parameters", () => {
      const template = "user:view::userId::sectionId";
      const params = { userId: 42, sectionId: "profile" };
      expect(RouteTemplateHelpers.processTemplate(template, params)).toBe("user:view:42:profile");
    });

    it("should leave missing parameters as placeholders", () => {
      const template = "user:view::userId:sectionId";
      const params = { userId: 42 };
      expect(RouteTemplateHelpers.processTemplate(template, params)).toBe("user:view:42:sectionId");
    });

    it("should handle templates with no parameters", () => {
      const template = "user:view";
      const params = { userId: 42 };
      expect(RouteTemplateHelpers.processTemplate(template, params)).toBe("user:view");
    });

    it("should handle empty params object", () => {
      const template = "user:view:userId";
      const params = {};
      expect(RouteTemplateHelpers.processTemplate(template, params)).toBe("user:view:userId");
    });

    it("should handle params with falsy values", () => {
      const template = "user:view::userId::active";
      const params = { userId: 0, active: false };
      expect(RouteTemplateHelpers.processTemplate(template, params)).toBe("user:view:0:false");
    });

    it("should handle nested parameter names", () => {
      const template = "user:view::user.id";
      const params = { "user.id": 123 };
      expect(RouteTemplateHelpers.processTemplate(template, params)).toBe("user:view:123");
    });

    it("should not replace partial matches", () => {
      const template = "user:view:userIdExtra";
      const params = { userId: 42 };
      expect(RouteTemplateHelpers.processTemplate(template, params)).toBe("user:view:userIdExtra");
    });
  });
});
