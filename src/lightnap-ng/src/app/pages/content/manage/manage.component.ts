import { CommonModule } from "@angular/common";
import { Component, inject, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import {
  EmptyPagedResponse,
  PagedResponseDto,
  RoutePipe,
  StaticContentDto,
  StaticContentReadAccesses,
  StaticContentSortBy,
  StaticContentSortBys,
  StaticContentStatuses,
  StaticContentTypes,
  TypeHelpers,
} from "@core";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { ContentReadAccessPickerComponent } from "@core/components/content-read-access-picker/content-read-access-picker.component";
import { ContentStatusPickerComponent } from "@core/components/content-status-picker/content-status-picker.component";
import { ContentTypePickerComponent } from "@core/components/content-type-picker/content-type-picker.component";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { ContentService } from "@core/features/content/services/content.service";
import { InputTextModule } from "primeng/inputtext";
import { PanelModule } from "primeng/panel";
import { TableLazyLoadEvent, TableModule } from "primeng/table";
import { debounceTime, startWith, Subject, switchMap } from "rxjs";

@Component({
  templateUrl: "./manage.component.html",
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    RoutePipe,
    PanelModule,
    InputTextModule,
    ApiResponseComponent,
    ErrorListComponent,
    TableModule,
    ContentTypePickerComponent,
    ContentStatusPickerComponent,
    ContentReadAccessPickerComponent,
  ],
})
export class ManageComponent {
  readonly pageSize = 10;

  readonly #contentService = inject(ContentService);

  readonly #fb = inject(FormBuilder);

  readonly form = this.#fb.group({
    keyContains: this.#fb.nonNullable.control(""),
    status: this.#fb.control<StaticContentStatuses | null>(null),
    readAccess: this.#fb.control<StaticContentReadAccesses | null>(null),
    type: this.#fb.control<StaticContentTypes | null>(null),
  });

  readonly errors = signal(new Array<string>());

  readonly #lazyLoadEventSubject = new Subject<TableLazyLoadEvent>();
  readonly contents$ = this.#lazyLoadEventSubject.pipe(
    switchMap(event =>
      this.#contentService.searchStaticContent({
        keyContains: this.form.value.keyContains,
        readAccess: this.form.value.readAccess ?? undefined,
        status: this.form.value.status ?? undefined,
        type: this.form.value.type ?? undefined,
        pageSize: this.pageSize,
        pageNumber: (event.first ?? 0) / this.pageSize + 1,
        sortBy: (event.sortField as StaticContentSortBys) ?? StaticContentSortBy.Key,
        reverseSort: event.sortOrder === -1,
      })
    ),
    // We need to bootstrap the p-table with a response to get the whole process running. We do it this way to fake an empty response
    // so we can avoid a redundant call to the API.
    startWith(new EmptyPagedResponse<StaticContentDto>())
  );

  readonly asContentResults = TypeHelpers.cast<PagedResponseDto<StaticContentDto>>;
  readonly asContent = TypeHelpers.cast<StaticContentDto>;

  constructor() {
    this.form.valueChanges.pipe(debounceTime(300)).subscribe({ next: () => this.#lazyLoadEventSubject.next({ first: 0 }) });
  }

  loadContentsLazy(event: TableLazyLoadEvent) {
    this.#lazyLoadEventSubject.next(event);
  }
}
