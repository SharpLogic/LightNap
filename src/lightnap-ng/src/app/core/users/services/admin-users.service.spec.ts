import { provideZonelessChangeDetection } from "@angular/core";
import { TestBed } from "@angular/core/testing";
import { of, throwError } from "rxjs";
import { AdminUsersService } from "./admin-users.service";
import { AdminUpdateUserRequestDto, AdminSearchUsersRequestDto, RoleDto, AdminUserDto, ClaimDto, PagedResponseDto, SearchClaimRequestDto, ErrorApiResponse } from "@core/backend-api";
import { UsersDataService } from "@core/backend-api/services/users-data.service";

describe("AdminUsersService", () => {
  let service: AdminUsersService;
  let dataServiceSpy: jasmine.SpyObj<UsersDataService>;

  beforeEach(() => {
    const spy = jasmine.createSpyObj("UsersDataService", [
      "getUser",
      "getUserByUserName",
      "updateUser",
      "deleteUser",
      "searchUsers",
      "getRoles",
      "getUserRoles",
      "getUsersInRole",
      "addUserToRole",
      "removeUserFromRole",
      "lockUserAccount",
      "unlockUserAccount",
      "getUsersById",
      "searchClaims",
      "searchUserClaims",
      "getUsersWithClaim",
      "addUserClaim",
      "removeUserClaim",
    ]);

    TestBed.configureTestingModule({
      providers: [provideZonelessChangeDetection(), AdminUsersService, { provide: UsersDataService, useValue: spy }],
    });

    service = TestBed.inject(AdminUsersService);
    dataServiceSpy = TestBed.inject(UsersDataService) as jasmine.SpyObj<UsersDataService>;
  });

  it("should be created", () => {
    expect(service).toBeTruthy();
  });

  describe("getUserByUserName", () => {
    it("should get user by username", () => {
      const userName = "testuser";
      const expectedUser = { id: "1", userName } as AdminUserDto;
      dataServiceSpy.getUserByUserName.and.returnValue(of(expectedUser));

      service.getUserByUserName(userName).subscribe(user => {
        expect(user).toEqual(expectedUser);
      });

      expect(dataServiceSpy.getUserByUserName).toHaveBeenCalledWith(userName);
    });
  });

  describe("getUserRoles", () => {
    it("should map role names to role objects correctly", () => {
      const userId = "user-id";
      const allRoles = [
        { name: "Admin" } as RoleDto,
        { name: "User" } as RoleDto,
        { name: "Guest" } as RoleDto
      ];
      const userRoleNames = ["Admin", "Guest"];

      dataServiceSpy.getRoles.and.returnValue(of(allRoles));
      dataServiceSpy.getUserRoles.and.returnValue(of(userRoleNames));

      service.getUserRoles(userId).subscribe(roles => {
        expect(roles.length).toBe(2);
        expect(roles[0].name).toBe("Admin");
        expect(roles[1].name).toBe("Guest");
      });
    });
  });

  describe("getRoles caching", () => {
    it("should cache roles and not call data service on subsequent calls", () => {
      const rolesResponse = [{ name: "admin" } as RoleDto];
      dataServiceSpy.getRoles.and.returnValue(of(rolesResponse));

      service.getRoles().subscribe();
      service.getRoles().subscribe();
      service.getRoles().subscribe();

      expect(dataServiceSpy.getRoles).toHaveBeenCalledTimes(1);
    });

    it("should return same cached instance across multiple subscriptions", done => {
      const rolesResponse = [{ name: "admin" } as RoleDto];
      dataServiceSpy.getRoles.and.returnValue(of(rolesResponse));

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

  describe("getRole", () => {
    it("should return undefined for non-existent role", () => {
      const rolesResponse = [{ name: "admin" } as RoleDto];
      dataServiceSpy.getRoles.and.returnValue(of(rolesResponse));

      service.getRole("nonexistent").subscribe(role => {
        expect(role).toBeUndefined();
      });
    });

    it("should be case-sensitive when matching role names", () => {
      const rolesResponse = [{ name: "Admin" } as RoleDto];
      dataServiceSpy.getRoles.and.returnValue(of(rolesResponse));

      service.getRole("admin").subscribe(role => {
        expect(role).toBeUndefined();
      });
    });
  });

  describe("getRoleWithUsers", () => {
    it("should return ErrorApiResponse when role does not exist", done => {
      dataServiceSpy.getRoles.and.returnValue(of([]));

      service.getRoleWithUsers("notfound").subscribe({
        error: (error: ErrorApiResponse<string>) => {
          expect(error.errorMessages).toContain("Role 'notfound' not found");
          done();
        },
      });
    });

    it("should combine role and users correctly", () => {
      const role = { name: "admin", description: "Administrator" } as RoleDto;
      const users = [
        { id: "1", userName: "user1" } as AdminUserDto,
        { id: "2", userName: "user2" } as AdminUserDto
      ];
      dataServiceSpy.getRoles.and.returnValue(of([role]));
      dataServiceSpy.getUsersInRole.and.returnValue(of(users));

      service.getRoleWithUsers("admin").subscribe(result => {
        expect(result.role).toEqual(role);
        expect(result.users.length).toBe(2);
        expect(result.users).toEqual(users);
      });
    });
  });

  describe("getUserWithRoles", () => {
    it("should combine user data with their roles", () => {
      const userId = "user-id";
      const userResponse = { id: userId, userName: "testUser", email: "test@test.com" } as AdminUserDto;
      const rolesResponse = [
        { name: "admin" } as RoleDto,
        { name: "user" } as RoleDto
      ];
      const userRolesResponse = ["admin", "user"];

      dataServiceSpy.getUser.and.returnValue(of(userResponse));
      dataServiceSpy.getUserRoles.and.returnValue(of(userRolesResponse));
      dataServiceSpy.getRoles.and.returnValue(of(rolesResponse));

      service.getUserWithRoles(userId).subscribe(result => {
        expect(result.user).toEqual(userResponse);
        expect(result.roles.length).toBe(2);
        expect(result.roles).toEqual(rolesResponse);
      });
    });
  });

  describe("getUsersById", () => {
    it("should return empty array for empty input without calling data service", () => {
      service.getUsersById([]).subscribe(result => {
        expect(result).toEqual([]);
        expect(dataServiceSpy.getUsersById).not.toHaveBeenCalled();
      });
    });

    it("should return empty array for null input", () => {
      service.getUsersById(null as any).subscribe(result => {
        expect(result).toEqual([]);
        expect(dataServiceSpy.getUsersById).not.toHaveBeenCalled();
      });
    });
  });

  describe("getUserClaims", () => {
    it("should transform user claims response correctly", () => {
      const userId = "user-id";
      const claimsData = [
        { type: "email", value: "test@test.com", userId },
        { type: "role", value: "admin", userId }
      ];
      dataServiceSpy.searchUserClaims.and.returnValue(of(<any>{ data: claimsData, totalCount: 2 }));

      service.getUserClaims({ userId }).subscribe(result => {
        expect(result.data).toEqual(claimsData);
        expect(result.totalCount).toBe(2);
      });
    });
  });

  describe("getUsersWithClaim", () => {
    it("should map user IDs to full user objects", () => {
      const claim: SearchClaimRequestDto = { type: "role", value: "admin" };
      const claimResults: PagedResponseDto<string> = { data: ["1", "2"], totalCount: 2, pageNumber: 1, pageSize: 2, totalPages: 1 };
      const users = [
        { id: "1", userName: "user1" } as AdminUserDto,
        { id: "2", userName: "user2" } as AdminUserDto
      ];

      dataServiceSpy.getUsersWithClaim.and.returnValue(of(claimResults));
      dataServiceSpy.getUsersById.and.returnValue(of(users));

      service.getUsersWithClaim(claim).subscribe(result => {
        expect(result.totalCount).toBe(2);
        expect(result.data.length).toBe(2);
        expect(result.data).toEqual(users);
      });

      expect(dataServiceSpy.getUsersById).toHaveBeenCalledWith(["1", "2"]);
    });

    it("should handle empty results correctly", () => {
      const claim: SearchClaimRequestDto = { type: "role", value: "admin" };
      dataServiceSpy.getUsersWithClaim.and.returnValue(of(<any>{ data: [], totalCount: 0 }));
      dataServiceSpy.getUsersById.and.returnValue(of([]));

      service.getUsersWithClaim(claim).subscribe(result => {
        expect(result.data.length).toBe(0);
        expect(result.totalCount).toBe(0);
      });
    });
  });
});
