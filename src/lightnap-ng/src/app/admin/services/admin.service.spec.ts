import { AdminUserDto, RoleDto, SearchAdminUsersRequestDto, UpdateAdminUserRequestDto } from "@admin/models";
import { TestBed } from "@angular/core/testing";
import { SuccessApiResponse } from "@core";
import { of } from "rxjs";
import { AdminService } from "./admin.service";
import { DataService } from "./data.service";
import { provideZonelessChangeDetection } from "@angular/core";

describe("AdminService", () => {
  let service: AdminService;
  let dataServiceSpy: jasmine.SpyObj<DataService>;

  beforeEach(() => {
    const spy = jasmine.createSpyObj("DataService", [
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
      providers: [provideZonelessChangeDetection(), AdminService, { provide: DataService, useValue: spy }],
    });

    service = TestBed.inject(AdminService);
    dataServiceSpy = TestBed.inject(DataService) as jasmine.SpyObj<DataService>;
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
    const updateRequest: UpdateAdminUserRequestDto = {};
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
    const searchRequest: SearchAdminUsersRequestDto = { sortBy: "userName", reverseSort: false };
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
});
