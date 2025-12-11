import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { ShowByPermissionsDirective } from './show-by-permissions.directive';
import { MockIdentityService } from '@testing';
import { IdentityService } from '@core/services/identity.service';
import { ClaimDto } from '@core/backend-api';
import { describe, beforeEach, it, expect } from 'vitest';

@Component({
    selector: 'app-test',
    imports: [ShowByPermissionsDirective],
    template: `<div showByPermissions [roles]="roles" [claims]="claims" id="testDiv">Content</div>`,
})
class TestComponent {
    roles?: string | string[];
    claims?: ClaimDto | ClaimDto[];
}

describe('ShowByPermissionsDirective', () => {
    let fixture: ComponentFixture<TestComponent>;
    let component: TestComponent;
    let mockIdentityService: MockIdentityService;
    let element: HTMLElement;

    beforeEach(() => {
        mockIdentityService = new MockIdentityService();
        mockIdentityService.setLoggedOut(); // Start logged out

        TestBed.configureTestingModule({
            imports: [TestComponent],
            providers: [
                provideZonelessChangeDetection(),
                { provide: IdentityService, useValue: mockIdentityService },
            ],
        });

        fixture = TestBed.createComponent(TestComponent);
        component = fixture.componentInstance;
        element = fixture.nativeElement.querySelector('#testDiv');
    });

    it('should show element when user has required role', async () => {
        mockIdentityService.setUserRoles(['Admin']);
        component.roles = 'Admin';
        fixture.detectChanges();
        await fixture.whenStable();

        expect(element.style.display).not.toBe('none');
    });

    it('should handle array of roles', async () => {
        mockIdentityService.setUserRoles(['User', 'Moderator']);
        component.roles = ['Admin', 'Moderator'];
        fixture.detectChanges();
        await fixture.whenStable();

        expect(element.style.display).not.toBe('none');
    });

    it('should show element when user has required claim', async () => {
        const claim: ClaimDto = { type: 'Permission', value: 'read' };
        mockIdentityService.setUserClaims([claim]);
        component.claims = claim;
        fixture.detectChanges();
        await fixture.whenStable();

        expect(element.style.display).not.toBe('none');
    });

    it('should handle array of claims', async () => {
        const claim1: ClaimDto = { type: 'Permission', value: 'read' };
        const claim2: ClaimDto = { type: 'Permission', value: 'write' };
        mockIdentityService.setUserClaims([claim1]);
        component.claims = [claim1, claim2];
        fixture.detectChanges();
        await fixture.whenStable();

        expect(element.style.display).not.toBe('none');
    });
});
