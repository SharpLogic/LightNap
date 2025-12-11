import { canDeactivateGuard } from './can-deactivate.guard';
import { OnCanDeactivate } from './can-deactivate';
import { of } from 'rxjs';

describe('canDeactivateGuard', () => {
    it('should allow deactivation when component does not implement canDeactivate', () => {
        const component = {} as OnCanDeactivate;

        const result = canDeactivateGuard(component, null as any, null as any, null as any);

        expect(result).toBe(true);
    });

    it('should call component canDeactivate when implemented', () => {
        const component: OnCanDeactivate = {
            canDeactivate: vi.fn().mockReturnValue(true),
        };

        const result = canDeactivateGuard(component, null as any, null as any, null as any);

        expect(component.canDeactivate).toHaveBeenCalled();
        expect(result).toBe(true);
    });

    it('should return false when component canDeactivate returns false', () => {
        const component: OnCanDeactivate = {
            canDeactivate: vi.fn().mockReturnValue(false),
        };

        const result = canDeactivateGuard(component, null as any, null as any, null as any);

        expect(result).toBe(false);
    });

    it('should handle observable return from canDeactivate', () => {
        const component: OnCanDeactivate = {
            canDeactivate: vi.fn().mockReturnValue(of(true)),
        };

        const result = canDeactivateGuard(component, null as any, null as any, null as any);

        expect(result).toBeTruthy();
    });

    it('should handle promise return from canDeactivate', () => {
        const component: OnCanDeactivate = {
            canDeactivate: vi.fn().mockReturnValue(Promise.resolve(false)),
        };

        const result = canDeactivateGuard(component, null as any, null as any, null as any);

        expect(result).toBeTruthy();
    });
});
