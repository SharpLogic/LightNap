import { CommonModule } from "@angular/common";
import { Component, contentChild, input, OnChanges, signal, SimpleChanges, TemplateRef } from "@angular/core";
import { ApiResponse, ErrorApiResponse, SuccessApiResponse } from "@core";
import { ProgressSpinnerModule } from "primeng/progressspinner";
import { catchError, map, Observable, of } from "rxjs";
import { ErrorListComponent } from "../error-list/error-list.component";

@Component({
  selector: "api-response",
  standalone: true,
  templateUrl: "./api-response.component.html",
  imports: [CommonModule, ErrorListComponent, ProgressSpinnerModule],
})
export class ApiResponseComponent implements OnChanges {
  readonly apiResponse = input.required<Observable<any>>();
  readonly undefinedMessage = input<string>("This item was not found");
  readonly errorMessage = input<string>("An error occurred");
  readonly loadingMessage = input<string>("Loading...");

  readonly successTemplateRef = contentChild<TemplateRef<any>>("success");
  readonly nullTemplateRef = contentChild<TemplateRef<any>>("null");
  readonly errorTemplateRef = contentChild<TemplateRef<any>>("error");
  readonly loadingTemplateRef = contentChild<TemplateRef<any>>("loading");

  internalApiResponse$ = signal<Observable<ApiResponse<any>> | undefined>(undefined);

  ngOnChanges(changes: SimpleChanges): void {
    if (changes["apiResponse"].currentValue) {
      this.internalApiResponse$.set(
        this.apiResponse().pipe(
          map(result => new SuccessApiResponse(result)),
          catchError((error: ApiResponse<any>) => {
            if (!error.type) {
              console.error(`ApiResponseComponent expects an ApiResponse object to have been thrown in throwError, but received:`, error);
              throw Error("ApiResponseComponent expects an ApiResponse object to have been thrown in throwError");
            }

            if (error.errorMessages?.length ?? 0 > 0) {
              return of(error as ApiResponse<string>);
            }

            return of(new ErrorApiResponse<any>(["No error message provided"]));
          })
        )
      );
    }
  }
}
