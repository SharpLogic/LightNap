import { provideZonelessChangeDetection } from "@angular/core";
import { TestBed } from "@angular/core/testing";
import { AdminUserDto, RoleDto } from "@core/backend-api";
import {
  getGetRolesResponseMock,
  getGetUserByUserNameResponseMock,
  getGetUserResponseAdminUserDtoMock,
  getGetUserResponseMock,
  getGetUsersInRoleResponseMock,
  getGetUsersWithClaimResponseMock,
  getSearchUserClaimsResponseMock,
  LightNapWebApiService,
} from "@core/backend-api/services/lightnap-api";
import { SearchRequestBuilder } from "@testing/builders";
import { createLightNapWebApiServiceSpy } from "@testing/helpers";
import { firstValueFrom, of } from "rxjs";
import { describe, beforeEach, expect, it, type MockedObject, vi } from "vitest";
import { AdminUsersService } from "./admin-users.service";

describe("AdminUsersService", () => {
  let service: AdminUsersService;
  let webApiServiceSpy: MockedObject<LightNapWebApiService>;

  beforeEach(() => {
    const spy = createLightNapWebApiServiceSpy() as any;
    const rolesResponse = getGetRolesResponseMock() || [];
    spy.getRoles = vi.fn().mockReturnValue(of(rolesResponse) as any);

    TestBed.configureTestingModule({
      providers: [provideZonelessChangeDetection(), AdminUsersService, { provide: LightNapWebApiService, useValue: spy }],
    });
    webApiServiceSpy = TestBed.inject(LightNapWebApiService) as MockedObject<LightNapWebApiService>;
    service = TestBed.inject(AdminUsersService);
  });

  it("should be created", () => {
    expect(service).toBeTruthy();
  });

  describe("getUserByUserName", () => {
    it("should get user by username", async () => {
      const userName = "testuser";
      const expectedUser = getGetUserByUserNameResponseMock() as AdminUserDto;
      webApiServiceSpy.getUserByUserName.mockReturnValue(of(expectedUser) as any);

      const user = await firstValueFrom(service.getUserByUserName(userName));
      expect(user).toEqual(expectedUser);
      expect(webApiServiceSpy.getUserByUserName).toHaveBeenCalledWith(userName);
    });
  });

  describe("getUserRoles", () => {
    it("should map role names to role objects correctly", async () => {
      const userId = "user-id";

      // Generate realistic role data
      const allRoles = getGetRolesResponseMock() || [];

      // Pick specific role names from the generated roles to ensure they exist
      const userRoleNames = allRoles.slice(0, 2).map(role => role.name || "");

      webApiServiceSpy.getRoles.mockReturnValue(of(allRoles) as any);
      webApiServiceSpy.getRolesForUser.mockReturnValue(of(userRoleNames) as any);

      const roles = await firstValueFrom(service.getUserRoles(userId));
      expect(roles.length).toBe(2);
      expect(roles[0].name).toBe(userRoleNames[0]);
      expect(roles[1].name).toBe(userRoleNames[1]);
    });
  });

  describe("getRoles caching", () => {
    it("should cache roles and not call data service on subsequent calls", async () => {
      const rolesResponse = getGetRolesResponseMock() || [];
      webApiServiceSpy.getRoles.mockReturnValue(of(rolesResponse) as any);

      await firstValueFrom(service.getRoles());
      await firstValueFrom(service.getRoles());
      await firstValueFrom(service.getRoles());

      expect(webApiServiceSpy.getRoles).toHaveBeenCalledTimes(1);
    });

    it("should return same cached instance across multiple subscriptions", async () => {
      const rolesResponse = getGetRolesResponseMock() || [];
      webApiServiceSpy.getRoles.mockReturnValue(of(rolesResponse) as any);

      const firstResult = await firstValueFrom(service.getRoles());
      const secondResult = await firstValueFrom(service.getRoles());

      expect(secondResult).toBe(firstResult);
    });
  });

  describe("getRoleWithUsers", () => {
    it("should combine role and users correctly", async () => {
      const allRoles = (getGetRolesResponseMock() || []).slice(0, 1);
      const role = allRoles[0];
      const users = getGetUsersInRoleResponseMock() || [];

      webApiServiceSpy.getRoles.mockReturnValue(of(allRoles) as any);
      webApiServiceSpy.getUsersInRole.mockReturnValue(of(users) as any);

      service = TestBed.inject(AdminUsersService);

      const result = await firstValueFrom(service.getRoleWithUsers(role.name || "admin"));
      expect(result.role).toEqual(role);
      expect(result.users).toEqual(users);
    });
  });

  describe("getUserWithRoles", () => {
    it("should combine user data with their roles", async () => {
      const userResponse = getGetUserResponseMock() as AdminUserDto;
      const allRoles = getGetRolesResponseMock() || [];
      const userRoleNames = allRoles.slice(0, 2).map(role => role.name || "");

      webApiServiceSpy.getUser.mockReturnValue(of(userResponse) as any);
      webApiServiceSpy.getRoles.mockReturnValue(of(allRoles) as any);
      webApiServiceSpy.getRolesForUser.mockReturnValue(of(userRoleNames) as any);

      const result = await firstValueFrom(service.getUserWithRoles(userResponse.id || "user-id"));
      expect(result.user).toEqual(userResponse);
      expect(result.roles.length).toBe(userRoleNames.length);
      expect(result.roles).toEqual(allRoles.slice(0, userRoleNames.length));
    });
  });

  describe("getUsersById", () => {
    it("should return empty array for empty input without calling data service", async () => {
      const result = await firstValueFrom(service.getUsersById([]));
      expect(result).toEqual([]);
      expect(webApiServiceSpy.getUsersByIds).not.toHaveBeenCalled();
    });

    it("should return empty array for null input", async () => {
      const result = await firstValueFrom(service.getUsersById(null as any));
      expect(result).toEqual([]);
      expect(webApiServiceSpy.getUsersByIds).not.toHaveBeenCalled();
    });
  });

  describe("getUserClaims", () => {
    it("should transform user claims response correctly", async () => {
      const userId = "user-id";
      const response = getSearchUserClaimsResponseMock({ data: (getSearchUserClaimsResponseMock().data || []).slice(0, 2) });

      webApiServiceSpy.searchUserClaims.mockReturnValue(of(response) as any);

      const result = await firstValueFrom(service.getUserClaims({ userId, pageNumber: 1, pageSize: 10 }));
      expect(result.data).toEqual(response.data);
      expect(result.totalCount).toBe(response.totalCount);
    });
  });

  describe("getUsersWithClaim", () => {
    it("should map user IDs to full user objects", async () => {
      const claim = SearchRequestBuilder.createSearchClaimRequest();
      const claimResponse = getGetUsersWithClaimResponseMock();
      const userIds = (claimResponse.data || []).slice(0, 2);
      const users = userIds.length > 0 ? [getGetUserResponseAdminUserDtoMock(), getGetUserResponseAdminUserDtoMock()] : [];

      webApiServiceSpy.getUsersWithClaim.mockReturnValue(of({ ...claimResponse, data: userIds }) as any);
      webApiServiceSpy.getUsersByIds.mockReturnValue(of(users) as any);

      const result = await firstValueFrom(service.getUsersWithClaim(claim));
      expect(result.data).toEqual(users);
      expect(webApiServiceSpy.getUsersByIds).toHaveBeenCalledWith(userIds);
    });

    it("should handle empty results correctly", async () => {
      const claim = SearchRequestBuilder.createSearchClaimRequest();
      const emptyResponse = { ...getGetUsersWithClaimResponseMock(), data: [], totalCount: 0 };

      webApiServiceSpy.getUsersWithClaim.mockReturnValue(of(emptyResponse) as any);
      webApiServiceSpy.getUsersByIds.mockReturnValue(of([]) as any);

      const result = await firstValueFrom(service.getUsersWithClaim(claim));
      expect(result.data.length).toBe(0);
      expect(result.totalCount).toBe(0);
    });
  });
});
