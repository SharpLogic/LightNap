/// <reference types="jasmine" />

import { HttpClientTestingModule } from "@angular/common/http/testing";
import { provideZonelessChangeDetection } from "@angular/core";
import { TestBed } from "@angular/core/testing";
import { AdminUserDto, PagedResponseDto, RoleDto, SearchClaimRequestDto, UserClaimDto } from "@core/backend-api";
import { getGetRolesResponseMock, getGetUserByUserNameResponseMock, LightNapWebApiService } from "@core/backend-api/services/lightnap-api";
import { of } from "rxjs";
import { AdminUsersService } from "./admin-users.service";

describe("AdminUsersService", () => {
  let service: AdminUsersService;
  let webApiServiceSpy: jasmine.SpyObj<any>;

  beforeEach(() => {
    const spy = jasmine.createSpyObj("LightNapWebApiService", [
      "getUser",
      "getUserByUserName",
      "updateUser",
      "deleteUser",
      "searchUsers",
      "getRoles",
      "getRolesForUser",
      "getUsersInRole",
      "addUserToRole",
      "removeUserFromRole",
      "lockUserAccount",
      "unlockUserAccount",
      "getUsersByIds",
      "searchClaims",
      "searchUserClaims",
      "getUsersWithClaim",
      "addUserClaim",
      "removeUserClaim",
    ]);

    // Set up the default return value for getRoles BEFORE creating the service
    spy.getRoles.and.returnValue(of(getGetRolesResponseMock()));

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [provideZonelessChangeDetection(), AdminUsersService, { provide: LightNapWebApiService, useValue: spy }],
    });

    webApiServiceSpy = TestBed.inject(LightNapWebApiService) as jasmine.SpyObj<any>;
    service = TestBed.inject(AdminUsersService);
  });

  it("should be created", () => {
    expect(service).toBeTruthy();
  });

  describe("getUserByUserName", () => {
    it("should get user by username", done => {
      const userName = "testuser";
      const expectedUser = getGetUserByUserNameResponseMock() as AdminUserDto;
      webApiServiceSpy.getUserByUserName.and.returnValue(of(expectedUser));

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
      const allRoles = [{ name: "Admin" } as RoleDto, { name: "User" } as RoleDto, { name: "Guest" } as RoleDto];
      const userRoleNames = ["Admin", "Guest"];

      webApiServiceSpy.getRoles.and.returnValue(of(allRoles));
      webApiServiceSpy.getRolesForUser.and.returnValue(of(userRoleNames));

      service.getUserRoles(userId).subscribe(roles => {
        expect(roles.length).toBe(2);
        expect(roles[0].name).toBe("Admin");
        expect(roles[1].name).toBe("Guest");
        done();
      });
    });
  });

  describe("getRoles caching", () => {
    it("should cache roles and not call data service on subsequent calls", done => {
      const rolesResponse = [{ name: "admin" } as RoleDto];
      webApiServiceSpy.getRoles.and.returnValue(of(rolesResponse));

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
      const rolesResponse = [{ name: "admin" } as RoleDto];
      webApiServiceSpy.getRoles.and.returnValue(of(rolesResponse));

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
      const role = { name: "admin", description: "Administrator" } as RoleDto;
      const users = [{ id: "1", userName: "user1" } as AdminUserDto, { id: "2", userName: "user2" } as AdminUserDto];

      webApiServiceSpy.getRoles.and.returnValue(of([role]));
      webApiServiceSpy.getUsersInRole.and.returnValue(of(users));

      service.getRoleWithUsers("admin").subscribe(result => {
        expect(result.role).toEqual(role);
        expect(result.users.length).toBe(2);
        expect(result.users).toEqual(users);
        done();
      });
    });
  });

  describe("getUserWithRoles", () => {
    it("should combine user data with their roles", done => {
      const userId = "user-id";
      const userResponse = { id: userId, userName: "testUser", email: "test@test.com" } as AdminUserDto;
      const rolesResponse = [{ name: "admin" } as RoleDto, { name: "user" } as RoleDto];
      const userRolesResponse = ["admin", "user"];

      webApiServiceSpy.getUser.and.returnValue(of(userResponse));
      webApiServiceSpy.getRoles.and.returnValue(of(rolesResponse));
      webApiServiceSpy.getRolesForUser.and.returnValue(of(userRolesResponse));

      service.getUserWithRoles(userId).subscribe(result => {
        expect(result.user).toEqual(userResponse);
        expect(result.roles.length).toBe(2);
        expect(result.roles).toEqual(rolesResponse);
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
      const claimsData = [
        { type: "email", value: "test@test.com", userId },
        { type: "role", value: "admin", userId },
      ];
      const response: PagedResponseDto<UserClaimDto> = {
        data: claimsData,
        totalCount: 2,
        pageNumber: 1,
        pageSize: 2,
        totalPages: 1,
      };
      webApiServiceSpy.searchUserClaims.and.returnValue(of(response));

      service.getUserClaims({ userId, pageNumber: 1, pageSize: 10 }).subscribe(result => {
        expect(result.data).toEqual(claimsData);
        expect(result.totalCount).toBe(2);
        done();
      });
    });
  });

  describe("getUsersWithClaim", () => {
    it("should map user IDs to full user objects", done => {
      const claim: SearchClaimRequestDto = { type: "role", value: "admin", pageNumber: 1, pageSize: 10 };
      const claimResults: PagedResponseDto<string> = { data: ["1", "2"], totalCount: 2, pageNumber: 1, pageSize: 2, totalPages: 1 };
      const users = [{ id: "1", userName: "user1" } as AdminUserDto, { id: "2", userName: "user2" } as AdminUserDto];

      webApiServiceSpy.getUsersWithClaim.and.returnValue(of(claimResults));
      webApiServiceSpy.getUsersByIds.and.returnValue(of(users));

      service.getUsersWithClaim(claim).subscribe(result => {
        expect(result.totalCount).toBe(2);
        expect(result.data.length).toBe(2);
        expect(result.data).toEqual(users);
        expect(webApiServiceSpy.getUsersByIds).toHaveBeenCalledWith(["1", "2"]);
        done();
      });
    });

    it("should handle empty results correctly", done => {
      const claim: SearchClaimRequestDto = { type: "role", value: "admin", pageNumber: 1, pageSize: 10 };
      const emptyResponse: PagedResponseDto<string> = {
        data: [],
        totalCount: 0,
        pageNumber: 1,
        pageSize: 10,
        totalPages: 0,
      };
      webApiServiceSpy.getUsersWithClaim.and.returnValue(of(emptyResponse));
      webApiServiceSpy.getUsersByIds.and.returnValue(of([]));

      service.getUsersWithClaim(claim).subscribe(result => {
        expect(result.data.length).toBe(0);
        expect(result.totalCount).toBe(0);
        done();
      });
    });
  });
});
