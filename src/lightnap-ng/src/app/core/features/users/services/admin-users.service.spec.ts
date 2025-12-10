import { provideZonelessChangeDetection } from "@angular/core";
import { TestBed } from "@angular/core/testing";
import { AdminUserDto, RoleDto, SearchClaimRequestDto } from "@core/backend-api";
import {
    getGetRolesResponseMock,
    getGetUserByUserNameResponseMock,
    getGetUserResponseAdminUserDtoMock,
    getGetUserResponseMock,
    getGetUsersInRoleResponseMock,
    getGetUsersWithClaimResponseMock,
    getSearchUserClaimsResponseMock,
    LightNapWebApiService
} from "@core/backend-api/services/lightnap-api";
import { createLightNapWebApiServiceSpy } from "@testing/helpers";
import { SearchRequestBuilder } from "@testing/builders";
import { of } from "rxjs";
import { AdminUsersService } from "./admin-users.service";

describe("AdminUsersService", () => {
  let service: AdminUsersService;
  let webApiServiceSpy: jasmine.SpyObj<LightNapWebApiService>;

  beforeEach(() => {
    const spy = createLightNapWebApiServiceSpy(jasmine);
    const rolesResponse = getGetRolesResponseMock() || [];
    spy.getRoles.and.returnValue(of(rolesResponse) as any);

    TestBed.configureTestingModule({
      providers: [provideZonelessChangeDetection(), AdminUsersService, { provide: LightNapWebApiService, useValue: spy }],
    });
    webApiServiceSpy = TestBed.inject(LightNapWebApiService) as jasmine.SpyObj<LightNapWebApiService>;
    service = TestBed.inject(AdminUsersService);
  });

  it("should be created", () => {
    expect(service).toBeTruthy();
  });

  describe("getUserByUserName", () => {
    it("should get user by username", done => {
      const userName = "testuser";
      const expectedUser = getGetUserByUserNameResponseMock() as AdminUserDto;
      webApiServiceSpy.getUserByUserName.and.returnValue(of(expectedUser) as any);

      service.getUserByUserName(userName).subscribe(user => {
        expect(user).toEqual(expectedUser);
        expect(webApiServiceSpy.getUserByUserName).toHaveBeenCalledWith(userName);
        done();
      });
    });
  });

  describe("getUserRoles", () => {
    it("should map role names to role objects correctly", done => {
      const userId = "user-id";

      // Generate realistic role data
      const allRoles = getGetRolesResponseMock() || [];

      // Pick specific role names from the generated roles to ensure they exist
      const userRoleNames = allRoles.slice(0, 2).map(role => role.name || "");

      webApiServiceSpy.getRoles.and.returnValue(of(allRoles) as any);
      webApiServiceSpy.getRolesForUser.and.returnValue(of(userRoleNames) as any);

      service.getUserRoles(userId).subscribe(roles => {
        expect(roles.length).toBe(2);
        expect(roles[0].name).toBe(userRoleNames[0]);
        expect(roles[1].name).toBe(userRoleNames[1]);
        done();
      });
    });
  });

  describe("getRoles caching", () => {
    it("should cache roles and not call data service on subsequent calls", done => {
      const rolesResponse = getGetRolesResponseMock() || [];
      webApiServiceSpy.getRoles.and.returnValue(of(rolesResponse) as any);

      service.getRoles().subscribe(() => {
        service.getRoles().subscribe(() => {
          service.getRoles().subscribe(() => {
            expect(webApiServiceSpy.getRoles).toHaveBeenCalledTimes(1);
            done();
          });
        });
      });
    });

    it("should return same cached instance across multiple subscriptions", done => {
      const rolesResponse = getGetRolesResponseMock() || [];
      webApiServiceSpy.getRoles.and.returnValue(of(rolesResponse) as any);

      let firstResult: Array<RoleDto>;
      service.getRoles().subscribe(roles => {
        firstResult = roles;
        service.getRoles().subscribe(roles2 => {
          expect(roles2).toBe(firstResult);
          done();
        });
      });
    });
  });

  describe("getRoleWithUsers", () => {
    it("should combine role and users correctly", done => {
      const allRoles = (getGetRolesResponseMock() || []).slice(0, 1);
      const role = allRoles[0];
      const users = getGetUsersInRoleResponseMock() || [];

      webApiServiceSpy.getRoles.and.returnValue(of(allRoles) as any);
      webApiServiceSpy.getUsersInRole.and.returnValue(of(users) as any);

      service = TestBed.inject(AdminUsersService);

      service.getRoleWithUsers(role.name || "admin").subscribe(result => {
        expect(result.role).toEqual(role);
        expect(result.users).toEqual(users);
        done();
      });
    });
  });

  describe("getUserWithRoles", () => {
    it("should combine user data with their roles", done => {
      const userResponse = getGetUserResponseMock() as AdminUserDto;
      const allRoles = getGetRolesResponseMock() || [];
      const userRoleNames = allRoles.slice(0, 2).map(role => role.name || "");

      webApiServiceSpy.getUser.and.returnValue(of(userResponse) as any);
      webApiServiceSpy.getRoles.and.returnValue(of(allRoles) as any);
      webApiServiceSpy.getRolesForUser.and.returnValue(of(userRoleNames) as any);

      service.getUserWithRoles(userResponse.id || "user-id").subscribe(result => {
        expect(result.user).toEqual(userResponse);
        expect(result.roles.length).toBe(userRoleNames.length);
        expect(result.roles).toEqual(allRoles.slice(0, userRoleNames.length));
        done();
      });
    });
  });

  describe("getUsersById", () => {
    it("should return empty array for empty input without calling data service", done => {
      service.getUsersById([]).subscribe(result => {
        expect(result).toEqual([]);
        expect(webApiServiceSpy.getUsersByIds).not.toHaveBeenCalled();
        done();
      });
    });

    it("should return empty array for null input", done => {
      service.getUsersById(null as any).subscribe(result => {
        expect(result).toEqual([]);
        expect(webApiServiceSpy.getUsersByIds).not.toHaveBeenCalled();
        done();
      });
    });
  });

  describe("getUserClaims", () => {
    it("should transform user claims response correctly", done => {
      const userId = "user-id";
      const response = getSearchUserClaimsResponseMock({ data: (getSearchUserClaimsResponseMock().data || []).slice(0, 2) });

      webApiServiceSpy.searchUserClaims.and.returnValue(of(response) as any);

      service.getUserClaims({ userId, pageNumber: 1, pageSize: 10 }).subscribe(result => {
        expect(result.data).toEqual(response.data);
        expect(result.totalCount).toBe(response.totalCount);
        done();
      });
    });
  });

  describe("getUsersWithClaim", () => {
    it("should map user IDs to full user objects", done => {
      const claim = SearchRequestBuilder.createSearchClaimRequest();
      const claimResponse = getGetUsersWithClaimResponseMock();
      const userIds = (claimResponse.data || []).slice(0, 2);
      const users = userIds.length > 0 ? [getGetUserResponseAdminUserDtoMock(), getGetUserResponseAdminUserDtoMock()] : [];

      webApiServiceSpy.getUsersWithClaim.and.returnValue(of({ ...claimResponse, data: userIds }) as any);
      webApiServiceSpy.getUsersByIds.and.returnValue(of(users) as any);

      service.getUsersWithClaim(claim).subscribe(result => {
        expect(result.data).toEqual(users);
        expect(webApiServiceSpy.getUsersByIds).toHaveBeenCalledWith(userIds);
        done();
      });
    });

    it("should handle empty results correctly", done => {
      const claim = SearchRequestBuilder.createSearchClaimRequest();
      const emptyResponse = { ...getGetUsersWithClaimResponseMock(), data: [], totalCount: 0 };

      webApiServiceSpy.getUsersWithClaim.and.returnValue(of(emptyResponse) as any);
      webApiServiceSpy.getUsersByIds.and.returnValue(of([]) as any);

      service.getUsersWithClaim(claim).subscribe(result => {
        expect(result.data.length).toBe(0);
        expect(result.totalCount).toBe(0);
        done();
      });
    });
  });
});
