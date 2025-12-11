import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { ErrorListComponent } from './error-list.component';
import { ErrorApiResponse } from '@core/backend-api';

describe('ErrorListComponent', () => {
    let component: ErrorListComponent;
    let fixture: ComponentFixture<ErrorListComponent>;

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [ErrorListComponent],
            providers: [
                provideZonelessChangeDetection(),
                provideNoopAnimations(),
            ],
        });

        fixture = TestBed.createComponent(ErrorListComponent);
        component = fixture.componentInstance;
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should display single error from error input', () => {
        fixture.componentRef.setInput('error', 'Test error message');
        fixture.detectChanges();

        expect(component.errorList()).toEqual(['Test error message']);
    });

    it('should display multiple errors from errors input', () => {
        fixture.componentRef.setInput('errors', ['Error 1', 'Error 2', 'Error 3']);
        fixture.detectChanges();

        expect(component.errorList()).toEqual(['Error 1', 'Error 2', 'Error 3']);
    });

    it('should display errors from apiResponse input', () => {
        const apiResponse = new ErrorApiResponse(['API Error 1', 'API Error 2']);
        fixture.componentRef.setInput('apiResponse', apiResponse);
        fixture.detectChanges();

        expect(component.errorList()).toEqual(['API Error 1', 'API Error 2']);
    });

    it('should prioritize error over errors input', () => {
        fixture.componentRef.setInput('error', 'Single error');
        fixture.componentRef.setInput('errors', ['Error 1', 'Error 2']);
        fixture.detectChanges();

        expect(component.errorList()).toEqual(['Single error']);
    });

    it('should prioritize errors over apiResponse input', () => {
        const apiResponse = new ErrorApiResponse(['API Error']);
        fixture.componentRef.setInput('errors', ['Direct Error']);
        fixture.componentRef.setInput('apiResponse', apiResponse);
        fixture.detectChanges();

        expect(component.errorList()).toEqual(['Direct Error']);
    });

    it('should show empty list when no errors provided', () => {
        fixture.detectChanges();

        expect(component.errorList()).toEqual([]);
    });

    it('should remove error when onClose is called', () => {
        fixture.componentRef.setInput('errors', ['Error 1', 'Error 2', 'Error 3']);
        fixture.detectChanges();

        component.close('Error 2');

        expect(component.errorList()).toEqual(['Error 1', 'Error 3']);
    });

    it('should handle closing non-existent error gracefully', () => {
        fixture.componentRef.setInput('errors', ['Error 1', 'Error 2']);
        fixture.detectChanges();

        component.close('Non-existent error');

        expect(component.errorList()).toEqual(['Error 1', 'Error 2']);
    });

    it('should update error list when input changes', () => {
        fixture.componentRef.setInput('errors', ['Error 1']);
        fixture.detectChanges();
        expect(component.errorList()).toEqual(['Error 1']);

        fixture.componentRef.setInput('errors', ['Error 2', 'Error 3']);
        fixture.detectChanges();
        expect(component.errorList()).toEqual(['Error 2', 'Error 3']);
    });

    it('should handle empty errors array', () => {
        fixture.componentRef.setInput('errors', []);
        fixture.detectChanges();

        expect(component.errorList()).toEqual([]);
    });

    it('should handle apiResponse with undefined errorMessages', () => {
        const apiResponse = new ErrorApiResponse(undefined as any);
        fixture.componentRef.setInput('apiResponse', apiResponse);
        fixture.detectChanges();

        expect(component.errorList()).toEqual([]);
    });
});
