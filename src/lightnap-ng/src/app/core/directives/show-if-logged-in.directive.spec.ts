import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { ShowIfLoggedInDirective } from './show-if-logged-in.directive';
import { MockIdentityService } from '@testing';
import { IdentityService } from '@core/services/identity.service';

@Component({
  selector: 'app-test',
  imports: [ShowIfLoggedInDirective],
  template: `<div showIfLoggedIn id="testDiv">Content</div>`,
})
class TestComponent {}

describe('ShowIfLoggedInDirective', () => {
  let fixture: ComponentFixture<TestComponent>;
  let mockIdentityService: MockIdentityService;
  let element: HTMLElement;

  beforeEach(() => {
    mockIdentityService = new MockIdentityService();

    TestBed.configureTestingModule({
      imports: [TestComponent],
      providers: [
        provideZonelessChangeDetection(),
        { provide: IdentityService, useValue: mockIdentityService },
      ],
    });

    fixture = TestBed.createComponent(TestComponent);
    element = fixture.nativeElement.querySelector('#testDiv');
  });

  it('should hide element when user is not logged in', () => {
    mockIdentityService.setLoggedOut();
    fixture.detectChanges();

    expect(element.style.display).toBe('none');
  });

  it('should show element when user is logged in', () => {
    mockIdentityService.setLoggedIn('test-token');
    fixture.detectChanges();

    expect(element.style.display).not.toBe('none');
  });

  it('should toggle visibility based on login state', () => {
    mockIdentityService.setLoggedOut();
    fixture.detectChanges();
    expect(element.style.display).toBe('none');

    mockIdentityService.setLoggedIn('test-token');
    fixture.detectChanges();
    expect(element.style.display).not.toBe('none');

    mockIdentityService.setLoggedOut();
    fixture.detectChanges();
    expect(element.style.display).toBe('none');
  });

  it('should restore display when showing after hiding', () => {
    mockIdentityService.setLoggedIn('test-token');
    fixture.detectChanges();
    expect(element.style.display).not.toBe('none');

    mockIdentityService.setLoggedOut();
    fixture.detectChanges();
    expect(element.style.display).toBe('none');

    mockIdentityService.setLoggedIn('test-token');
    fixture.detectChanges();
    expect(element.style.display).not.toBe('none');
  });
});
