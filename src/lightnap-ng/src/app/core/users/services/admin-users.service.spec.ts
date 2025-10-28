import { provideZonelessChangeDetection } from "@angular/core";
import { TestBed } from "@angular/core/testing";
import { of } from "rxjs";
import { AdminUsersService } from "./admin-users.service";
import { AdminUpdateUserRequestDto, AdminSearchUsersRequestDto, RoleDto, AdminUserDto, ClaimDto, ApiResponseDto } from "@core/backend-api";
import { UsersDataService } from "@core/backend-api/services/users-data.service";

describe("UsersService", () => {
  let service: AdminUsersService;
  let dataServiceSpy: jasmine.SpyObj<UsersDataService>;

  beforeEach(() => {
    const spy = jasmine.createSpyObj("UsersDataService", [
      "getUser",
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

  it("should get user by ID", () => {
    const userId = "user-id";
    dataServiceSpy.getUser.and.returnValue(of({} as any));

    service.getUser(userId).subscribe();

    expect(dataServiceSpy.getUser).toHaveBeenCalledWith(userId);
  });

  it("should update user", () => {
    const userId = "user-id";
    const updateRequest: AdminUpdateUserRequestDto = {};
    dataServiceSpy.updateUser.and.returnValue(of({} as any));

    service.updateUser(userId, updateRequest).subscribe();

    expect(dataServiceSpy.updateUser).toHaveBeenCalledWith(userId, updateRequest);
  });

  it("should delete user", () => {
    const userId = "user-id";
    dataServiceSpy.deleteUser.and.returnValue(of({} as any));

    service.deleteUser(userId).subscribe();

    expect(dataServiceSpy.deleteUser).toHaveBeenCalledWith(userId);
  });

  it("should search users", () => {
    const searchRequest: AdminSearchUsersRequestDto = { sortBy: "userName", reverseSort: false };
    dataServiceSpy.searchUsers.and.returnValue(of({} as any));

    service.searchUsers(searchRequest).subscribe();

    expect(dataServiceSpy.searchUsers).toHaveBeenCalledWith(searchRequest);
  });

  it("should get roles", () => {
    dataServiceSpy.getRoles.and.returnValue(of({} as any));

    service.getRoles().subscribe();

    expect(dataServiceSpy.getRoles).toHaveBeenCalled();
  });

  it("should get role by name", () => {
    dataServiceSpy.getRoles.and.returnValue(of({} as any));

    service.getRoles().subscribe();

    expect(dataServiceSpy.getRoles).toHaveBeenCalled();
  });

  it("should get user roles", () => {
    const role = "Administrator";
    const userId = "user-id";
    const rolesResponse = new Array<RoleDto>({ name: role } as RoleDto);
    const userRolesResponse = new Array<string>(role);
    dataServiceSpy.getRoles.and.returnValue(of(rolesResponse));
    dataServiceSpy.getUserRoles.and.returnValue(of(userRolesResponse));

    service.getUserRoles(userId).subscribe();

    expect(dataServiceSpy.getRoles).toHaveBeenCalled();
    expect(dataServiceSpy.getUserRoles).toHaveBeenCalledWith(userId);
  });

  it("should get users in role", () => {
    const role = "Administrator";
    dataServiceSpy.getUsersInRole.and.returnValue(of({} as any));

    service.getUsersInRole(role).subscribe();

    expect(dataServiceSpy.getUsersInRole).toHaveBeenCalledWith(role);
  });

  it("should add user to role", () => {
    const userId = "user-id";
    const role = "admin";
    dataServiceSpy.addUserToRole.and.returnValue(of({} as any));

    service.addUserToRole(userId, role).subscribe();

    expect(dataServiceSpy.addUserToRole).toHaveBeenCalledWith(userId, role);
  });

  it("should remove user from role", () => {
    const userId = "user-id";
    const role = "admin";
    dataServiceSpy.removeUserFromRole.and.returnValue(of({} as any));

    service.removeUserFromRole(userId, role).subscribe();

    expect(dataServiceSpy.removeUserFromRole).toHaveBeenCalledWith(userId, role);
  });

  it("should lock user account", () => {
    const userId = "user-id";
    dataServiceSpy.lockUserAccount.and.returnValue(of({} as any));

    service.lockUserAccount(userId).subscribe();

    expect(dataServiceSpy.lockUserAccount).toHaveBeenCalledWith(userId);
  });

  it("should unlock user account", () => {
    const userId = "user-id";
    dataServiceSpy.unlockUserAccount.and.returnValue(of({} as any));

    service.unlockUserAccount(userId).subscribe();

    expect(dataServiceSpy.unlockUserAccount).toHaveBeenCalledWith(userId);
  });

  it("should get user with roles", () => {
    const userId = "user-id";
    const userResponse = { id: userId, userName: "testUser" } as AdminUserDto;
    const rolesResponse = [{ name: "admin" } as RoleDto];
    const userRolesResponse = ["admin"];

    dataServiceSpy.getUser.and.returnValue(of(userResponse));
    dataServiceSpy.getUserRoles.and.returnValue(of(userRolesResponse));
    dataServiceSpy.getRoles.and.returnValue(of(rolesResponse));

    service.getUserWithRoles(userId).subscribe(userWithRoles => {
      expect(userWithRoles).toBeDefined();
      expect(userWithRoles.user).toEqual(userResponse);
      expect(userWithRoles.roles).toEqual(rolesResponse);
    });

    expect(dataServiceSpy.getUser).toHaveBeenCalledWith(userId);
    expect(dataServiceSpy.getUserRoles).toHaveBeenCalledWith(userId);
    expect(dataServiceSpy.getRoles).toHaveBeenCalled();
  });

  it("should return empty array when getUsersById is called with empty array", () => {
    service.getUsersById([]).subscribe(result => {
      expect(result).toEqual([]);
    });
  });

  it("should call getUsersById on dataService when userIds are provided", () => {
    const userIds = ["1", "2"];
    dataServiceSpy.getUsersById = jasmine.createSpy().and.returnValue(of([{ id: "1" }, { id: "2" }]));
    service.getUsersById(userIds).subscribe();
    expect(dataServiceSpy.getUsersById).toHaveBeenCalledWith(userIds);
  });

  it("should cache roles after first getRoles call", () => {
    const rolesResponse = [{ name: "admin" } as RoleDto];
    dataServiceSpy.getRoles.and.returnValue(of(rolesResponse));
    service.getRoles().subscribe(roles => {
      expect(roles).toEqual(rolesResponse);
      // Call again, should not call dataServiceSpy.getRoles again
      service.getRoles().subscribe(roles2 => {
        expect(roles2).toEqual(rolesResponse);
        expect(dataServiceSpy.getRoles).toHaveBeenCalledTimes(1);
      });
    });
  });

  it("should get role by name", () => {
    const rolesResponse = [{ name: "admin" } as RoleDto, { name: "user" } as RoleDto];
    dataServiceSpy.getRoles.and.returnValue(of(rolesResponse));
    service.getRole("admin").subscribe(role => {
      expect(role).toEqual(rolesResponse[0]);
    });
  });

  it("should return error when getRoleWithUsers is called with non-existent role", done => {
    dataServiceSpy.getRoles.and.returnValue(of([]));
    service.getRoleWithUsers("notfound").subscribe({
      error: (error: ApiResponseDto<Array<RoleDto>>) => {
        expect(error.errorMessages?.length).toBeGreaterThan(0);
        done();
      },
    });
  });

  it("should get role with users", () => {
    const role = { name: "admin" } as RoleDto;
    const users = [{ id: "1" } as AdminUserDto];
    dataServiceSpy.getRoles.and.returnValue(of([role]));
    dataServiceSpy.getUsersInRole.and.returnValue(of(users));
    service.getRoleWithUsers("admin").subscribe(result => {
      expect(result.role).toEqual(role);
      expect(result.users).toEqual(users);
    });
  });

  it("should search claims", () => {
    const searchClaims = { type: "email" };
    dataServiceSpy.searchClaims = jasmine.createSpy().and.returnValue(of({ data: [] }));
    service.searchClaims(searchClaims as any).subscribe();
    expect(dataServiceSpy.searchClaims).toHaveBeenCalledWith(searchClaims);
  });

  it("should get user claims", () => {
    const userId = "user-id";
    const claims = [{ type: "email", value: "test@test.com", userId }];
    dataServiceSpy.searchUserClaims = jasmine.createSpy().and.returnValue(of({ data: claims }));
    service.getUserClaims({userId}).subscribe(result => {
      expect(result.data).toEqual(claims);
    });
    expect(dataServiceSpy.searchUserClaims).toHaveBeenCalledWith({ userId });
  });

  it("should return empty array if getUsersWithClaim finds no users", () => {
    const claim = { type: "role", value: "admin" } as any;
    dataServiceSpy.searchUserClaims = jasmine.createSpy().and.returnValue(of({ data: [] }));
    service.getUsersWithClaim(claim).subscribe(result => {
      expect(result.totalCount).toEqual(0);
    });
  });

  it("should get users with claim", () => {
    const claim: ClaimDto = { type: "role", value: "admin" };
    const userIds = [{ userId: "1" }, { userId: "2" }];
    const users = new Array<Partial<AdminUserDto>>({ id: "1" }, { id: "2" });
    dataServiceSpy.searchUserClaims = jasmine.createSpy().and.returnValue(of({ data: userIds }));
    dataServiceSpy.getUsersById = jasmine.createSpy().and.returnValue(of(users));
    service.getUsersWithClaim(claim).subscribe(result => {
      expect(result).toEqual(jasmine.arrayContaining(users));
    });
  });

  it("should add user claim", () => {
    const userId = "user-id";
    const claim = { type: "role", value: "admin" } as any;
    dataServiceSpy.addUserClaim = jasmine.createSpy().and.returnValue(of(true));
    service.addUserClaim(userId, claim).subscribe(result => {
      expect(result).toBeTrue();
    });
    expect(dataServiceSpy.addUserClaim).toHaveBeenCalledWith(userId, claim);
  });

  it("should remove user claim", () => {
    const userId = "user-id";
    const claim = { type: "role", value: "admin" } as any;
    dataServiceSpy.removeUserClaim = jasmine.createSpy().and.returnValue(of(true));
    service.removeUserClaim(userId, claim).subscribe(result => {
      expect(result).toBeTrue();
    });
    expect(dataServiceSpy.removeUserClaim).toHaveBeenCalledWith(userId, claim);
  });
});
