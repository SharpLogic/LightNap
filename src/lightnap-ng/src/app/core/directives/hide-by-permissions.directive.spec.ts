import { Component } from "@angular/core";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { provideZonelessChangeDetection } from "@angular/core";
import { HideByPermissionsDirective } from "./hide-by-permissions.directive";
import { MockIdentityService } from "@testing";
import { IdentityService } from "@core/services/identity.service";
import { ClaimDto } from "@core/backend-api";
import { describe, beforeEach, it, expect } from "vitest";

@Component({
  selector: "app-test",
  imports: [HideByPermissionsDirective],
  template: `<div hideByPermissions [roles]="roles" [claims]="claims" id="testDiv">Content</div>`,
})
class TestComponent {
  roles?: string | string[];
  claims?: ClaimDto | ClaimDto[];
}

describe("HideByPermissionsDirective", () => {
  let fixture: ComponentFixture<TestComponent>;
  let component: TestComponent;
  let mockIdentityService: MockIdentityService;
  let element: HTMLElement;

  beforeEach(() => {
    mockIdentityService = new MockIdentityService();
    mockIdentityService.setLoggedOut(); // Start logged out

    TestBed.configureTestingModule({
      imports: [TestComponent],
      providers: [provideZonelessChangeDetection(), { provide: IdentityService, useValue: mockIdentityService }],
    });

    fixture = TestBed.createComponent(TestComponent);
    component = fixture.componentInstance;
    element = fixture.nativeElement.querySelector("#testDiv");
  });

  it("should hide element when user has specified role", async () => {
    mockIdentityService.setUserRoles(["Admin"]);
    component.roles = "Admin";
    fixture.detectChanges();
    await fixture.whenStable();

    expect(element.style.display).toBe("none");
  });

  it("should handle array of roles", async () => {
    mockIdentityService.setUserRoles(["User", "Moderator"]);
    component.roles = ["Admin", "Moderator"];
    fixture.detectChanges();
    await fixture.whenStable();

    expect(element.style.display).toBe("none");
  });

  it("should hide element when user has specified claim", async () => {
    const claim: ClaimDto = { type: "Permission", value: "read" };
    mockIdentityService.setUserClaims([claim]);
    component.claims = claim;
    fixture.detectChanges();
    await fixture.whenStable();

    expect(element.style.display).toBe("none");
  });

  it("should handle array of claims", async () => {
    const claim1: ClaimDto = { type: "Permission", value: "read" };
    const claim2: ClaimDto = { type: "Permission", value: "write" };
    mockIdentityService.setUserClaims([claim1]);
    component.claims = [claim1, claim2];
    fixture.detectChanges();
    await fixture.whenStable();

    expect(element.style.display).toBe("none");
  });
});
