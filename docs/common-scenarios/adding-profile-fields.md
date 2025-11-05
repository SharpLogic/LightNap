---
title: Adding Profile Fields
layout: home
parent: Common Scenarios
nav_order: 500
---

# {{ page.title }}

The default LightNap profile isn't very interesting. It just has some default fields and timestamps. Let's take a look at how it can be extended to have new fields for first and last names.

- TOC
{:toc}

## Backend Changes

We'll start off by updating the backend by changing in layers from entity model to DTOs and then services. Most of the backend work that needs to be done to change the data model happens in the `LightNap.Core` project.

### Updating the Entity

1. Open `Data/Entities/ApplicationUser.cs`. This is the entity that represents a user.
2. Add properties for the first and last name.

    ```csharp
    public class ApplicationUser : IdentityUser
    {
      public required string FirstName { get; set; } = string.Empty;
      public required string LastName { get; set; } = string.Empty;
      ...
    ```

### Updating the Data Transfer Objects (DTOs)

Almost all access to the `ApplicationUser` class is restricted to the services exposed by the project. As a result, there are DTOs that need to be updated.

1. Open `Profile/Dto/Response/ProfileDto.cs`. This is the DTO used in responses to logged-in user requests for their own profile.
2. Add fields for the first and last name.

    ```csharp
    public class ProfileDto
    {
      public required string FirstName { get; set; }
      public required string LastName { get; set; }
      ...
    ```

3. Open `Profile/Dto/Request/UpdateProfileDto.cs`. This is the DTO used by users requesting updates to their profile.
4. Add fields for the first and last name.

    ```csharp
    public class UpdateProfileDto
    {
      public required string FirstName { get; set; }
      public required string LastName { get; set; }
      ...
    ```

5. Open `Identity/Dto/Request/RegisterRequestDto.cs`. This is the DTO submitted by users registering an account on the site.
6. Add fields for the first and last name.

    ```csharp
    public class RegisterRequestDto
    {
      public required string FirstName { get; set; }
      public required string LastName { get; set; }
      ...
    ```

### Updating the Extension Method Mappings

There is no direct mapping relationship between the `ApplicationUser` class and its related DTOs. That mapping is all performed by external extension methods added to `ApplicationUser`. Those methods need to be updated to account for the new fields.

1. Open `Extensions/ApplicationUserExtensions.cs`. This class contains all extension methods for converting `ApplicationUser` instances to DTOs and for applying changes from DTOs to an `ApplicationUser` instance.
2. Add fields for the first and last name to the `ToLoggedInUserDto` method.

    ```csharp
    public static ProfileDto ToLoggedInUserDto(this ApplicationUser user)
    {
      return new ProfileDto()
      {
        FirstName = user.FirstName,
        LastName = user.LastName,
        ...
    ```

3. Update fields for the first and last name to the `ToCreate` method.

    ```csharp
    public static ApplicationUser ToCreate(this RegisterRequestDto dto, bool twoFactorEnabled)
    {
      var user = new ApplicationUser()
      {
        FirstName = dto.FirstName,
        LastName = dto.LastName,
      ...
    ```

4. Add fields for the first and last name to the `UpdateLoggedInUser` method.

    ```csharp
    public static void UpdateLoggedInUser(this ApplicationUser user, UpdateProfileDto dto)
    {
      user.FirstName = dto.FirstName;
      user.LastName = dto.LastName;
      ...
    ```

### Add A Migration

1. Add an [Entity Framework migration](../getting-started/database-providers/ef-migrations) and update the database.

    {: .note}
    It's recommended to use the [in-memory data provider](../getting-started//database-providers/in-memory-provider) while working out the details of an entity model update, if feasible. Then a single migration can be created and applied once the design is finalized.

### Update Tests

For the sake of brevity, updates to the test project are not covered in this article. However, updating them should be straightforward as the API surface area limits changes exposed to the outside to the DTOs with new fields, such as `RegisterRequestDto`. It's also a good practice to add/update tests for the new fields and functionality.

### Additional Backend Changes

Because all profile manipulation is handled through DTOs and extension methods there is no need to make any other changes on the backend. The data will now flow from the REST API as request DTOs that validate input values as required.

If there is a need to enforce additional restrictions, such as length ranges, that can be done via attributes on the request DTOs (see `RegisterRequestDto` for examples on how this can be done). Otherwise all incoming request DTOs are passed by the controllers to their underlying services that call `ApplicationUser` extension methods to get or update database data. However, if there is a need to apply further rules or transformations, that can be done within the service methods.

## Frontend Changes

The frontend is also divided into areas that map directly to the backend areas including profile, administrator, and identity. We will approach them area by area so that a full data flow from API to component can be completed before moving to the next. Everything frontend is contained in the `lightnap-ng` project.

### Updating the Registration Frontend

1. Open `app/core/backend-api/identity/dtos/identity/request/register-request-dto.ts`. This is the model that maps to the backend `RegisterRequestDto`.
2. Add fields for the first and last names.

    ``` typescript
    export interface RegisterRequest {
      firstName: string;
      lastName: string;
      ...
    ```

3. Open `app/pages/identity/register.component.ts`. This is the code for the page where users register.
4. Add fields for the first and last names to the form. This will allow easy binding in the reactive form markup.

    ``` typescript
    export class RegisterComponent {
      ...
      form = this.#fb.nonNullable.group({
        firstName: this.#fb.control("", [Validators.required]),
        lastName: this.#fb.control("", [Validators.required]),
      ...
    ```

5. Update the `#identityService.register()` parameter with fields for the names.

    ``` typescript
    register() {
      ...
      this.#identityService.register({
        firstName: this.form.value.firstName,
        lastName: this.form.value.lastName,
        ...
    ```

6. Open `app/pages/identity/register.component.html`. This is the markup for the page where users register.
7. Add input fields for the names before the password input.

    ``` html
    ...
    <label for="firstName" class="text-xl">First Name</label>
    <input id="firstName" type="text" placeholder="First Name" pInputText formControlName="firstName" />

    <label for="lastName" class="text-xl">Last Name</label>
    <input id="lastName" type="text" placeholder="Last Name" pInputText formControlName="lastName" />
    ...
    ```

### Updating the Profile Frontend

1. Open `app/core/backend-api/profile/response/profile-dto.ts`. This is the model that maps to the backend `ProfileDto`.
2. Add fields for the first and last names.

    ``` typescript
    export interface Profile {
      firstName: string;
      lastName: string;
      ...
    ```

3. Open `app/core/backend-api/profile/request/update-profile-request-dto.ts`. This is the model that maps to the backend `UpdateProfileDto`.
4. Add fields for the first and last names.

    ``` typescript
    export interface UpdateProfileRequest {
      firstName: string;
      lastName: string;
      ...
    ```

5. Open `app/pages/profile/index/index.component.ts`. This is the code for the page users see when they visit their profile. It includes a stub for a profile update form, but there are no fields by default.
6. Add fields for the first and last names to the form. This will allow easy binding in the reactive form markup.

    ``` typescript
    export class IndexComponent {
      ...
      form = this.#fb.group({
        firstName: this.#fb.control("", [Validators.required]),
        lastName: this.#fb.control("", [Validators.required]),
      });
      ...
    ```

7. Update the `getProfile` `tap` to update the values in the form after the profile has loaded.

    ``` typescript
    profile$ = this.#profileService.getProfile().pipe(
      tap(profile => {
        // Set form values.
        this.form.setValue({
          firstName: profile.firstName,
          lastName: profile.lastName,
          ...
    ```

8. Update the call to the `#profileService.updateProfile` parameter to include the new fields.

    ``` typescript
    updateProfile() {
      ...
      this.#profileService
        .updateProfile({
          firstName: this.form.value.firstName,
          lastName: this.form.value.lastName
      ...
    ```

9. Open `app/pages/profile/index/index.component.html`. This is the markup for the page users see when they visit their profile. It also includes a stub for a profile update form, but there are no fields by default.
10. Update the body of the form with some markup for the new fields.

    ``` html
    <form [formGroup]="form" (ngSubmit)="updateProfile()" autocomplete="off">
      ...
      <div class="flex flex-col gap-1">
        <label for="firstName" class="font-semibold">First Name</label>
        <input id="firstName" type="text" pInputText formControlName="firstName" class="w-full mb-2" />
      </div>
      <label for="lastName" class="font-semibold">Last Name</label>
      <input id="lastName" type="text" pInputText formControlName="lastName" class="w-full mb-2" />
    ...
    ```

### Updating the User Functionality

While the _profile_ functionality covers a given user and their own details, there is also a _user_ concept used throughout LightNap that covers how the data for a given account is rendered based on user permission. By default, the backend DTOs for this are:

- `PublicUserDto`: The minimum details accessible to a user who is not logged in.
- `PrivilegedUserDto`: An extension of `PublicUserDto` that extends with fields available to _privileged_ users. In the default implementation this applies to any authenticated user.
- `AdminUserDto`: An extension of the `PrivilegedUserDto` that includes the full set of fields available to users in the `Administrator` role.

Since these DTOs inherit from one another, each derived class automatically includes the properties of its base class. For example, if you want the first and last name properties to only be visible to privileged and administrator users, add them to `PrivilegedUserDto`. The same goes for the search DTOs like `PublicUsersSearchRequestDto` and so on. These are separated so that you can easily use a different set of search parameters from what is available in the response DTO.

If you'd like to continue the exercise to implement these new properties for user functionality throughout the app, the general steps are:

1. Update the backend DTOs to reflect the data available for retrieving and searching users.
2. Update `Extensions/ApplicationUserExtensions.cs` to populate the fields of the various DTO mappings.
3. Update `UsersService.SearchUsersAsync` in `Users/Services/UsersService.cs` to implement search on filters supported at the appropriate privilege level.
4. Update the frontend DTOs in `core/backend-api/dtos/users` to reflect the changes made to the backend DTOs.
5. Update page components in `pages/admin`  to render the new user fields in the appropriate places.

## See Also

- [Adding Entities](./adding-entities) - Working with Entity Framework models
- [Adding In-App Notification Types](./adding-notifications) - Another example of working with entities and DTOs
- [Solution & Project Structure](../concepts/project-structure) - Understanding the data flow pattern
