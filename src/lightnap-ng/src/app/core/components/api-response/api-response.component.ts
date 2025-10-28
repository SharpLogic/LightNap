import { CommonModule } from "@angular/common";
import { Component, contentChild, input, OnChanges, signal, SimpleChanges, TemplateRef } from "@angular/core";
import { ApiResponseDto, ErrorApiResponse, SuccessApiResponse } from "@core/backend-api";
import { ProgressSpinnerModule } from "primeng/progressspinner";
import { catchError, map, Observable, of } from "rxjs";
import { ErrorListComponent } from "../error-list/error-list.component";

@Component({
  selector: "api-response",
  standalone: true,
  templateUrl: "./api-response.component.html",
  imports: [CommonModule, ErrorListComponent, ProgressSpinnerModule],
})
export class ApiResponseComponent<T> implements OnChanges {
  readonly apiResponse = input.required<Observable<T>>();
  readonly nullMessage = input<string>("This item was not found");
  readonly errorMessage = input<string>("An error occurred");
  readonly loadingMessage = input<string>("Loading...");

  // At the time of writing, the compiler was not able to infer the implicit types projected from this component into provided
  // templates. Hopefully at some point you will be able to define a #success template that knows it's getting an $implicit of
  // type T so you get support and enforcement without any additional work. In the meantime, you should use TypeHelpers.cast<T> to
  // case the any it hands you to the type you know it will be for a better markup experience.
  readonly successTemplateRef = contentChild<TemplateRef<{ $implicit: T }>>("success");
  readonly nullTemplateRef = contentChild<TemplateRef<void>>("null");
  readonly errorTemplateRef = contentChild<TemplateRef<{ $implicit: ApiResponseDto<T> }>>("error");
  readonly loadingTemplateRef = contentChild<TemplateRef<void>>("loading");

  internalApiResponse$ = signal<Observable<ApiResponseDto<T>> | undefined>(undefined);

  ngOnChanges(changes: SimpleChanges): void {
    if (changes["apiResponse"].currentValue) {
      this.internalApiResponse$.set(
        this.apiResponse().pipe(
          map(result => new SuccessApiResponse(result)),
          catchError((error: ApiResponseDto<T>) => {
            if (!error.type) {
              console.error(`ApiResponseComponent expects an ApiResponse object to have been thrown in throwError, but received:`, error);
              return of(new ErrorApiResponse([`Raw error: ${(error as any).message ?? JSON.stringify(error)}`]));
            }

            if (error.errorMessages?.length ?? 0 > 0) {
              return of(error as ApiResponseDto<T>);
            }

            return of(new ErrorApiResponse<T>(["No error message provided"]));
          })
        )
      );
    }
  }
}
