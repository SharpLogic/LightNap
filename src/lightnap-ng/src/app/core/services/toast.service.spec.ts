import { TestBed } from '@angular/core/testing';
import { MessageService } from 'primeng/api';
import { ToastService } from './toast.service';
import { provideZonelessChangeDetection } from '@angular/core';

describe('ToastService', () => {
    let service: ToastService;
    let messageServiceSpy: jasmine.SpyObj<MessageService>;

    beforeEach(() => {
        const spy = jasmine.createSpyObj('MessageService', ['add']);

        TestBed.configureTestingModule({
            providers: [
                provideZonelessChangeDetection(),
                ToastService,
                { provide: MessageService, useValue: spy }
            ]
        });

        service = TestBed.inject(ToastService);
        messageServiceSpy = TestBed.inject(MessageService) as jasmine.SpyObj<MessageService>;
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should call MessageService.add with correct parameters for success', () => {
        const message = 'Success message';
        const title = 'Success';

        service.success(message);

        expect(messageServiceSpy.add).toHaveBeenCalledWith({
            key: 'global',
            detail: message,
            severity: 'success',
            summary: title
        });
    });

    it('should call MessageService.add with correct parameters for info', () => {
        const message = 'Info message';
        const title = 'Info';

        service.info(message);

        expect(messageServiceSpy.add).toHaveBeenCalledWith({
            key: 'global',
            detail: message,
            severity: 'info',
            summary: title
        });
    });

    it('should call MessageService.add with correct parameters for error', () => {
        const message = 'Error message';
        const title = 'Error';

        service.error(message);

        expect(messageServiceSpy.add).toHaveBeenCalledWith({
            key: 'global',
            detail: message,
            severity: 'error',
            summary: title
        });
    });

    it('should call MessageService.add with correct parameters for warn', () => {
        const message = 'Warning message';
        const title = 'Warning';

        service.warn(message);

        expect(messageServiceSpy.add).toHaveBeenCalledWith({
            key: 'global',
            detail: message,
            severity: 'warn',
            summary: title
        });
    });

    it('should call MessageService.add with custom key', () => {
        const message = 'Custom message';
        const title = 'Custom';
        const severity = 'info';
        const key = 'customKey';

        service.show(message, title, severity, key);

        expect(messageServiceSpy.add).toHaveBeenCalledWith({
            key: key,
            detail: message,
            severity: severity,
            summary: title
        });
    });
});
