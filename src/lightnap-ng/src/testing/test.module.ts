import { CommonModule } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { BrowserAnimationsModule, NoopAnimationsModule } from '@angular/platform-browser/animations';
import { RouterTestingModule } from '@angular/router/testing';

/**
 * Shared testing module with common imports
 * 
 * Import this module in your test's TestBed configuration to get
 * commonly used testing modules without repeating imports
 * 
 * Usage:
 * ```typescript
 * TestBed.configureTestingModule({
 *   imports: [SharedTestingModule, YourComponentModule],
 *   // ... other config
 * });
 * ```
 */
@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    HttpClientTestingModule,
    RouterTestingModule,
    NoopAnimationsModule,
  ],
  exports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    HttpClientTestingModule,
    RouterTestingModule,
    NoopAnimationsModule,
  ],
})
export class SharedTestingModule {}

/**
 * Testing module with real animations (for animation testing)
 */
@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    HttpClientTestingModule,
    RouterTestingModule,
    BrowserAnimationsModule,
  ],
  exports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    HttpClientTestingModule,
    RouterTestingModule,
    BrowserAnimationsModule,
  ],
})
export class SharedTestingWithAnimationsModule {}
