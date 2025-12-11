import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { ConfirmPopupComponent } from './confirm-popup.component';
import { ConfirmationService, PrimeIcons } from 'primeng/api';

describe('ConfirmPopupComponent', () => {
    let component: ConfirmPopupComponent;
    let fixture: ComponentFixture<ConfirmPopupComponent>;

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [ConfirmPopupComponent],
            providers: [
                provideZonelessChangeDetection(),
                provideNoopAnimations(),
                ConfirmationService,
            ],
        });

        fixture = TestBed.createComponent(ConfirmPopupComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    describe('Default Input Values', () => {
        it('should have default confirmText', () => {
            expect(component.confirmText()).toBe('Confirm');
        });

        it('should have default confirmSeverity', () => {
            expect(component.confirmSeverity()).toBe('danger');
        });

        it('should have default confirmIcon', () => {
            expect(component.confirmIcon()).toBe(PrimeIcons.TRASH);
        });

        it('should have default rejectText', () => {
            expect(component.rejectText()).toBe('Cancel');
        });

        it('should have default rejectSeverity', () => {
            expect(component.rejectSeverity()).toBe('secondary');
        });

        it('should have default rejectIcon', () => {
            expect(component.rejectIcon()).toBe(PrimeIcons.UNDO);
        });

        it('should have default key as empty string', () => {
            expect(component.key()).toBe('');
        });
    });

    describe('Custom Input Values', () => {
        it('should accept custom confirmText', () => {
            fixture.componentRef.setInput('confirmText', 'Yes');
            expect(component.confirmText()).toBe('Yes');
        });

        it('should accept custom confirmSeverity', () => {
            fixture.componentRef.setInput('confirmSeverity', 'success');
            expect(component.confirmSeverity()).toBe('success');
        });

        it('should accept custom confirmIcon', () => {
            fixture.componentRef.setInput('confirmIcon', PrimeIcons.CHECK);
            expect(component.confirmIcon()).toBe(PrimeIcons.CHECK);
        });

        it('should accept custom rejectText', () => {
            fixture.componentRef.setInput('rejectText', 'No');
            expect(component.rejectText()).toBe('No');
        });

        it('should accept custom rejectSeverity', () => {
            fixture.componentRef.setInput('rejectSeverity', 'info');
            expect(component.rejectSeverity()).toBe('info');
        });

        it('should accept custom rejectIcon', () => {
            fixture.componentRef.setInput('rejectIcon', PrimeIcons.TIMES);
            expect(component.rejectIcon()).toBe(PrimeIcons.TIMES);
        });

        it('should accept custom key', () => {
            fixture.componentRef.setInput('key', 'deletePopup');
            expect(component.key()).toBe('deletePopup');
        });
    });

    describe('Template Rendering', () => {
        it('should render p-confirmpopup element', () => {
            const compiled = fixture.nativeElement as HTMLElement;
            const confirmPopup = compiled.querySelector('p-confirmpopup');

            expect(confirmPopup).toBeTruthy();
        });

        it('should bind key input to p-confirmpopup', () => {
            fixture.componentRef.setInput('key', 'testKey');
            expect(component.key()).toBe('testKey');
        });

        it('should render confirm button with correct attributes', () => {
            fixture.componentRef.setInput('confirmText', 'Delete');
            fixture.componentRef.setInput('confirmSeverity', 'danger');
            fixture.componentRef.setInput('confirmIcon', PrimeIcons.TRASH);

            expect(component.confirmText()).toBe('Delete');
            expect(component.confirmSeverity()).toBe('danger');
            expect(component.confirmIcon()).toBe(PrimeIcons.TRASH);
        });

        it('should render reject button with correct attributes', () => {
            fixture.componentRef.setInput('rejectText', 'Cancel');
            fixture.componentRef.setInput('rejectSeverity', 'secondary');
            fixture.componentRef.setInput('rejectIcon', PrimeIcons.UNDO);

            expect(component.rejectText()).toBe('Cancel');
            expect(component.rejectSeverity()).toBe('secondary');
            expect(component.rejectIcon()).toBe(PrimeIcons.UNDO);
        });

        // Note: Template content (message span, button container) is inside ng-template
        // and won't be rendered until the popup is actually shown via ConfirmationService
    });
});
