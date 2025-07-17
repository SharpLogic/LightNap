---
title: API Response Model
layout: home
parent: Concepts
nav_order: 400
---

# {{ page.title }}

LightNap relies on a REST API model for communication between the Angular frontend and the .NET backend. Both implementations use the standard models provided by their respective platforms, but with slight adjustments to make it easier for developers to elegantly implement new features.

## The API Response Object

All API responses should be returned as JSON using the following structure:

```json
{
  "type": "Success",
  "result": {},
  "errorMessages": []
}
```

| Field           | Purpose                       | Type                                                                                                                                           |
| :-------------- | :---------------------------- | :--------------------------------------------------------------------------------------------------------------------------------------------- |
| `type`          | The status of the operation.  | - `Success`: The operation succeeded and `result` contains the result.                                                                         |
|                 |                               | - `Error`: There was a handled error and `errorMessages` contains user-friendly error messages, such as feedback on invalid input.             |
|                 |                               | - `UnexpectedError`: There was an unexpected error and `errorMessages` contains a generic error (and debug details if not a production build). |
| `result`        | The payload of the response.  | The object, array, string, or number response. Assumed to be `null` if not returned. Not provided if the operation did not succeed.            |
| `errorMessages` | User-friendly error messages. | A list of user-friendly error messages. Not provided if the operation succeeded.                                                               |

The backend uses the `ApiResponseDto` object to represent responses while the frontend uses the `ApiResponse` interface.

## HTTP Codes

LightNap tries to return `200 OK` codes for all responses handled by the .NET pipeline. This makes it easier for API consumers, like the Angular frontend, to process responses without having to focus on the various ways errors can be returned and interpreted. In other words, API consumers can expect `200 OK` responses back from all API requests unless there is an error that results in the pipeline not being invoked.

The main exceptions are:

- `400 Bad Request` typically means there is an API access error, such as a required field not being provided in a POST object. These are viewed as design-time errors that the developer must address before deploying.
- `401 Unauthorized` is automatically sent when a user is trying to request an endpoint protected by `[Authorize]` but is not logged in.
- `403 Forbidden` is automatically sent when the user is logged in but does not meet the role and/or claim policy requirement for the endpoint.
- `404 Not Found` is automatically sent when the API URL requested does not exist.

  {: .note }
  `404 Not Found` should not sent when a requested item is not found during a `GET` query. Instead, `null` should be returned as the `Result` property payload of the `ApiResponseDto` object so that the API consumer understands that the API call succeeded and that the requested item does not exist.

- `405 Method Not Allowed` is automatically sent when an endpoint exists but the wrong verb is used. For example, using `POST` on a `PUT` endpoint.

There may also be 500-level codes thrown due to the backend having unrecoverable issues, such as the service being unavailable.

## Backend Practices

LightNap is architected to perform all major work in `LightNap.Core` service classes. These services expect to run in a trusted context where user access and permissions have already been validated. As a result, they accept and return DTOs and only perform validation work related to business rules and data integrity. Return errors from these services by throwing `UserFriendlyException`. These exceptions are caught by the Web API middleware, logged as applicable, and converted to `ApiResponse` objects with the provided user-friendly error messages.

Successful responses are returned from the core services as DTOs that are then wrapped by the invoking API controller as a properly-typed `ApiResponse` object.

## Frontend Practices

LightNap's `apiResponseInterceptor` automatically unwraps successful API responses and publishes the `result` property directly. This allows all pipelines to assume the underlying API calls have succeeded, significantly streamlining code because they don't have to deal with the wrapping `ApiResponse` object.

However, if the response indicates error--or if the HTTP code is not `200 OK`--it uses `throwError` to bypass the pipelines with the proper `ApiResponse` object. This allows streamlined error processing since you can handle the error via `catchError` in a pipeline if you need to. Otherwise, if you explicitly subscribed, you must provide an `error` handler in the subscription setup.

### The ApiResponse Component

In most UI scenarios you will want to allow Angular to manage your RxJS subscriptions via the `async` pipe. To make this process even simpler, LightNap provides an `ApiResponseComponent` that can be used in markup with a simple pattern that handles dealing with an observable returned from an API call.

``` html
<api-response [apiResponse]="apiResponse$">
    <ng-template #success let-result>
      ...
    </ng-template>
    <ng-template #null>
      ...
    </ng-template>
    <ng-template #error let-response>
      ...
    </ng-template>
    <ng-template #loading>
      ...
    </ng-template>
</api-response>
```

{: .note }
The `apiResponse` parameter must be an observable of the type you want to use in the template. If there was an error, an `ApiResponse` needs to have been thrown via `throwError` if the pipeline failed and was not handled along the way. This will just work as-is if you're using an observable that originated from an API call that was processed by `apiResponseInterceptor`.

`ApiResponseComponent` manages the different states of an API request via the user-provided templates.

- `#success`: The template to render when the response is successful. The unwrapped result is provided as the implicit template variable (`result` in this case).
- `#null`: The template to render when the response is successful but has a null result field. If you don't provide this template and the response's result is null, then your `#success` template will be used.
- `#error`: The optional template to render when the response fails. The full response is provided as the implicit template variable (`response` in this case). Alternatively, you can provide an `errorMessage` string on `api-response` to be shown along with a list of the user-friendly responses in the caught `ApiResponse`.
- `#loading`: The optional template to render when a response has not been received yet. Alternatively, you can provide a `loadingMessage` string on `api-response` to show alongside the default loading UI.
