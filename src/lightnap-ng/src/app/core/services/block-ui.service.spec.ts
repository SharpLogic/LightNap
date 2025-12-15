import { TestBed } from "@angular/core/testing";
import { BlockUiService } from "./block-ui.service";
import { BlockUiParams } from "@core/models";
import { provideZonelessChangeDetection } from "@angular/core";

describe("BlockUiService", () => {
  let service: BlockUiService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideZonelessChangeDetection()],
    });
    service = TestBed.inject(BlockUiService);
  });

  it("should be created", () => {
    expect(service).toBeTruthy();
  });

  it("should emit show event with correct parameters", async () => {
    const blockUiParams: BlockUiParams = { message: "Loading..." };
    service.onShow$.subscribe(params => {
      expect(params).toEqual(blockUiParams);
    });
    service.show(blockUiParams);
  });

  it("should emit hide event", async () => {
    service.onHide$.subscribe({
      next: () => {
        expect(true).toBe(true);
      },
    });
    service.hide();
  });
});
