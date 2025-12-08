
import { Component, OnChanges, SimpleChanges, input, signal } from "@angular/core";
import { MessageModule } from "primeng/message";
import { ApiResponseDto } from "@core";

@Component({
  selector: "ln-error-list",
  templateUrl: "./error-list.component.html",
  imports: [MessageModule],
})
export class ErrorListComponent implements OnChanges {
  readonly error = input<string | undefined>(undefined);
  readonly errors = input<Array<string> | undefined>(undefined);
  readonly apiResponse = input<ApiResponseDto<any> | undefined>(undefined);

  errorList = signal<Array<string>>([]);

  ngOnChanges(changes: SimpleChanges) {
    const error = this.error();
    const errors = this.errors();
    const apiResponse = this.apiResponse();
    if (error) {
      this.errorList.set([error]);
    } else if (errors?.length) {
      this.errorList.set([...errors]);
    } else if (apiResponse?.errorMessages?.length) {
      this.errorList.set([...apiResponse.errorMessages]);
    } else {
      this.errorList.set([]);
    }
  }

  close(error: string) {
    this.errorList.set(this.errorList().filter(e => e !== error));
  }
}
