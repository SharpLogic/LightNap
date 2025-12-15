import { FormControl, FormGroup } from "@angular/forms";
import { confirmPasswordValidator } from "./form-helpers";

describe("FormHelpers", () => {
  describe("confirmPasswordValidator", () => {
    let form: FormGroup;

    beforeEach(() => {
      form = new FormGroup({
        password: new FormControl(""),
        confirmPassword: new FormControl(""),
      });
    });

    it("should return null when passwords match", () => {
      form.patchValue({
        password: "SecurePass123",
        confirmPassword: "SecurePass123",
      });

      const validator = confirmPasswordValidator("password", "confirmPassword");
      const result = validator(form);

      expect(result).toBeNull();
    });

    it("should return error object when passwords do not match", () => {
      form.patchValue({
        password: "SecurePass123",
        confirmPassword: "DifferentPass456",
      });

      const validator = confirmPasswordValidator("password", "confirmPassword");
      const result = validator(form);

      expect(result).toEqual({ passwordsDoNotMatch: true });
    });

    it("should return null when both fields are empty", () => {
      form.patchValue({
        password: "",
        confirmPassword: "",
      });

      const validator = confirmPasswordValidator("password", "confirmPassword");
      const result = validator(form);

      expect(result).toBeNull();
    });

    it("should throw error when first password control does not exist", () => {
      const validator = confirmPasswordValidator("nonexistent", "confirmPassword");

      expect(() => validator(form)).toThrowError(/Form control 'nonexistent' not found/);
    });

    it("should throw error when second password control does not exist", () => {
      const validator = confirmPasswordValidator("password", "nonexistent");

      expect(() => validator(form)).toThrowError(/Form control 'nonexistent' not found/);
    });

    it("should work with custom field names", () => {
      const customForm = new FormGroup({
        newPassword: new FormControl("Pass123"),
        repeatPassword: new FormControl("Pass123"),
      });

      const validator = confirmPasswordValidator("newPassword", "repeatPassword");
      const result = validator(customForm);

      expect(result).toBeNull();
    });

    it("should detect mismatch with whitespace differences", () => {
      form.patchValue({
        password: "Password123",
        confirmPassword: "Password123 ",
      });

      const validator = confirmPasswordValidator("password", "confirmPassword");
      const result = validator(form);

      expect(result).toEqual({ passwordsDoNotMatch: true });
    });
  });
});
