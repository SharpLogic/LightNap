import { describe, expect, it } from 'vitest';
import { throwInlineError } from './error-helpers';

describe('ErrorHelpers', () => {
    describe('throwInlineError', () => {
        it('should throw an error with the provided message', () => {
            const errorMessage = 'Test error message';

            expect(() => throwInlineError(errorMessage)).toThrowError(errorMessage);
        });

        it('should throw Error type', () => {
            expect(() => throwInlineError('Any message')).toThrow();
        });

        it('should be usable inline with nullish coalescing operator', () => {
            const config: {
                value?: string;
            } = {};

            expect(() => {
                const value = config.value ?? throwInlineError('Missing value');
            }).toThrowError('Missing value');
        });

        it('should allow execution to continue when not invoked', () => {
            const config = { value: 'exists' };

            expect(() => {
                const value = config.value ?? throwInlineError('Missing value');
                expect(value).toBe('exists');
            }).not.toThrow();
        });
    });
});
