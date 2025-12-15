import { ComponentFixture, TestBed } from "@angular/core/testing";
import { provideZonelessChangeDetection } from "@angular/core";
import { provideNoopAnimations } from "@angular/platform-browser/animations";
import { ConfirmDialogComponent } from "./confirm-dialog.component";
import { ConfirmationService, PrimeIcons } from "primeng/api";
import { describe, beforeEach, expect, it } from "vitest";

describe("ConfirmDialogComponent", () => {
  let component: ConfirmDialogComponent;
  let fixture: ComponentFixture<ConfirmDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ConfirmDialogComponent],
      providers: [provideZonelessChangeDetection(), provideNoopAnimations(), ConfirmationService],
    }).compileComponents();

    fixture = TestBed.createComponent(ConfirmDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });

  describe("Default Input Values", () => {
    it("should have default confirmText", () => {
      expect(component.confirmText()).toBe("Confirm");
    });

    it("should have default confirmSeverity", () => {
      expect(component.confirmSeverity()).toBe("danger");
    });

    it("should have default confirmIcon", () => {
      expect(component.confirmIcon()).toBe(PrimeIcons.TRASH);
    });

    it("should have default rejectText", () => {
      expect(component.rejectText()).toBe("Cancel");
    });

    it("should have default rejectSeverity", () => {
      expect(component.rejectSeverity()).toBe("secondary");
    });

    it("should have default rejectIcon", () => {
      expect(component.rejectIcon()).toBe(PrimeIcons.UNDO);
    });

    it("should have default key as empty string", () => {
      expect(component.key()).toBe("");
    });

    it("should have default appendTo as undefined", () => {
      expect(component.appendTo()).toBeUndefined();
    });
  });

  describe("Custom Input Values", () => {
    it("should accept custom confirmText", () => {
      fixture.componentRef.setInput("confirmText", "Yes");
      expect(component.confirmText()).toBe("Yes");
    });

    it("should accept custom confirmSeverity", () => {
      fixture.componentRef.setInput("confirmSeverity", "success");
      expect(component.confirmSeverity()).toBe("success");
    });

    it("should accept custom confirmIcon", () => {
      fixture.componentRef.setInput("confirmIcon", PrimeIcons.CHECK);
      expect(component.confirmIcon()).toBe(PrimeIcons.CHECK);
    });

    it("should accept custom rejectText", () => {
      fixture.componentRef.setInput("rejectText", "No");
      expect(component.rejectText()).toBe("No");
    });

    it("should accept custom rejectSeverity", () => {
      fixture.componentRef.setInput("rejectSeverity", "info");
      expect(component.rejectSeverity()).toBe("info");
    });

    it("should accept custom rejectIcon", () => {
      fixture.componentRef.setInput("rejectIcon", PrimeIcons.TIMES);
      expect(component.rejectIcon()).toBe(PrimeIcons.TIMES);
    });

    it("should accept custom key", () => {
      fixture.componentRef.setInput("key", "deleteDialog");
      expect(component.key()).toBe("deleteDialog");
    });

    it("should accept custom appendTo", () => {
      const mockElement = document.createElement("div");
      fixture.componentRef.setInput("appendTo", mockElement);
      expect(component.appendTo()).toBe(mockElement);
    });
  });

  describe("Template Rendering", () => {
    it("should render p-confirmDialog element", () => {
      const compiled = fixture.nativeElement as HTMLElement;
      const confirmDialog = compiled.querySelector("p-confirmDialog");

      expect(confirmDialog).toBeTruthy();
    });

    it("should bind key input to p-confirmDialog", () => {
      fixture.componentRef.setInput("key", "testKey");
      expect(component.key()).toBe("testKey");
    });

    it("should render confirm button with correct attributes", () => {
      fixture.componentRef.setInput("confirmText", "Delete");
      fixture.componentRef.setInput("confirmSeverity", "danger");
      fixture.componentRef.setInput("confirmIcon", PrimeIcons.TRASH);

      expect(component.confirmText()).toBe("Delete");
      expect(component.confirmSeverity()).toBe("danger");
      expect(component.confirmIcon()).toBe(PrimeIcons.TRASH);
    });

    it("should render reject button with correct attributes", () => {
      fixture.componentRef.setInput("rejectText", "Cancel");
      fixture.componentRef.setInput("rejectSeverity", "secondary");
      fixture.componentRef.setInput("rejectIcon", PrimeIcons.UNDO);

      expect(component.rejectText()).toBe("Cancel");
      expect(component.rejectSeverity()).toBe("secondary");
      expect(component.rejectIcon()).toBe(PrimeIcons.UNDO);
    });
  });
});
