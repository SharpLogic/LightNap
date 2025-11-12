import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideZonelessChangeDetection, signal } from '@angular/core';
import { SelectListItemComponent } from './select-list-item.component';
import { ListItem } from '@core/models/list-item';

describe('SelectListItemComponent', () => {
  let component: SelectListItemComponent;
  let fixture: ComponentFixture<SelectListItemComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [SelectListItemComponent],
      providers: [provideZonelessChangeDetection()],
    });

    fixture = TestBed.createComponent(SelectListItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Default State', () => {
    it('should have empty label signal by default', () => {
      expect(component.label()).toBe('');
    });

    it('should have empty description signal by default', () => {
      expect(component.description()).toBe('');
    });

    it('should render empty label by default', () => {
      const compiled = fixture.nativeElement as HTMLElement;
      const labelSpan = compiled.querySelector('span');

      expect(labelSpan?.textContent?.trim()).toBe('');
    });
  });

  describe('Direct Input - Label', () => {
    it('should accept label as signal input', () => {
      const testLabel = signal('Test Label');

      // Create new fixture with label already set
      fixture = TestBed.createComponent(SelectListItemComponent);
      component = fixture.componentInstance;
      component.label = testLabel;
      fixture.detectChanges();

      expect(component.label()).toBe('Test Label');
    });

    it('should render the label text', () => {
      const testLabel = signal('Test Label');

      // Create new fixture with label already set
      fixture = TestBed.createComponent(SelectListItemComponent);
      component = fixture.componentInstance;
      component.label = testLabel;
      fixture.detectChanges();

      const compiled = fixture.nativeElement as HTMLElement;
      const labelSpan = compiled.querySelector('span');

      expect(labelSpan?.textContent?.trim()).toBe('Test Label');
    });

    it('should update rendered label when signal changes', () => {
      const testLabel = signal('Initial Label');

      // Create new fixture with label already set
      fixture = TestBed.createComponent(SelectListItemComponent);
      component = fixture.componentInstance;
      component.label = testLabel;
      fixture.detectChanges();

      let compiled = fixture.nativeElement as HTMLElement;
      let labelSpan = compiled.querySelector('span');
      expect(labelSpan?.textContent?.trim()).toBe('Initial Label');

      // Change the signal
      testLabel.set('Updated Label');
      fixture.detectChanges();

      compiled = fixture.nativeElement as HTMLElement;
      labelSpan = compiled.querySelector('span');
      expect(labelSpan?.textContent?.trim()).toBe('Updated Label');
    });
  });

  describe('Direct Input - Description', () => {
    it('should accept description as signal input', () => {
      const testDescription = signal('Test Description');

      // Create new fixture with description already set
      fixture = TestBed.createComponent(SelectListItemComponent);
      component = fixture.componentInstance;
      component.description = testDescription;
      fixture.detectChanges();

      expect(component.description()).toBe('Test Description');
    });

    it('should render description when provided', () => {
      // Create new fixture with signals already set
      fixture = TestBed.createComponent(SelectListItemComponent);
      component = fixture.componentInstance;
      component.label = signal('Label');
      component.description = signal('Test Description');
      fixture.detectChanges();

      const compiled = fixture.nativeElement as HTMLElement;
      const descriptionP = compiled.querySelector('p');

      expect(descriptionP?.textContent?.trim()).toBe('Test Description');
    });

    it('should not render description paragraph when description is empty', () => {
      // Create new fixture with signals already set
      fixture = TestBed.createComponent(SelectListItemComponent);
      component = fixture.componentInstance;
      component.label = signal('Label');
      component.description = signal('');
      fixture.detectChanges();

      const compiled = fixture.nativeElement as HTMLElement;
      const descriptionP = compiled.querySelector('p');

      expect(descriptionP).toBeFalsy();
    });

    it('should not render description paragraph when description is undefined', () => {
      // Create new fixture with signals already set
      fixture = TestBed.createComponent(SelectListItemComponent);
      component = fixture.componentInstance;
      component.label = signal('Label');
      component.description = signal(undefined);
      fixture.detectChanges();

      const compiled = fixture.nativeElement as HTMLElement;
      const descriptionP = compiled.querySelector('p');

      expect(descriptionP).toBeFalsy();
    });
  });

  describe('ListItem Input', () => {
    it('should set label from listItem', () => {
      const listItem = new ListItem('value', 'List Item Label');

      // Create new fixture and set listItem before detectChanges
      fixture = TestBed.createComponent(SelectListItemComponent);
      component = fixture.componentInstance;
      component.listItem = listItem;
      fixture.detectChanges();

      expect(component.label()).toBe('List Item Label');
    });

    it('should set description from listItem', () => {
      const listItem = new ListItem('value', 'Label', 'List Item Description');

      // Create new fixture and set listItem before detectChanges
      fixture = TestBed.createComponent(SelectListItemComponent);
      component = fixture.componentInstance;
      component.listItem = listItem;
      fixture.detectChanges();

      expect(component.description()).toBe('List Item Description');
    });

    it('should render label from listItem', () => {
      const listItem = new ListItem('value', 'List Item Label');

      // Create new fixture and set listItem before detectChanges
      fixture = TestBed.createComponent(SelectListItemComponent);
      component = fixture.componentInstance;
      component.listItem = listItem;
      fixture.detectChanges();

      const compiled = fixture.nativeElement as HTMLElement;
      const labelSpan = compiled.querySelector('span');

      expect(labelSpan?.textContent?.trim()).toBe('List Item Label');
    });

    it('should render description from listItem', () => {
      const listItem = new ListItem('value', 'Label', 'List Item Description');

      // Create new fixture and set listItem before detectChanges
      fixture = TestBed.createComponent(SelectListItemComponent);
      component = fixture.componentInstance;
      component.listItem = listItem;
      fixture.detectChanges();

      const compiled = fixture.nativeElement as HTMLElement;
      const descriptionP = compiled.querySelector('p');

      expect(descriptionP?.textContent?.trim()).toBe('List Item Description');
    });

    it('should handle listItem without description', () => {
      const listItem = new ListItem('value', 'Label');

      // Create new fixture and set listItem before detectChanges
      fixture = TestBed.createComponent(SelectListItemComponent);
      component = fixture.componentInstance;
      component.listItem = listItem;
      fixture.detectChanges();

      const compiled = fixture.nativeElement as HTMLElement;
      const descriptionP = compiled.querySelector('p');

      expect(descriptionP).toBeFalsy();
      expect(component.description()).toBeUndefined();
    });

    it('should work with different value types', () => {
      const numberItem = new ListItem(42, 'Number Label', 'Number Description');

      // Create new fixture and set listItem before detectChanges
      fixture = TestBed.createComponent(SelectListItemComponent);
      component = fixture.componentInstance;
      component.listItem = numberItem;
      fixture.detectChanges();

      expect(component.label()).toBe('Number Label');
      expect(component.description()).toBe('Number Description');
    });

    it('should update when listItem changes', () => {
      const listItem1 = new ListItem('val1', 'First Label', 'First Description');

      // Create new fixture and set first listItem before detectChanges
      fixture = TestBed.createComponent(SelectListItemComponent);
      component = fixture.componentInstance;
      component.listItem = listItem1;
      fixture.detectChanges();

      expect(component.label()).toBe('First Label');
      expect(component.description()).toBe('First Description');

      const listItem2 = new ListItem('val2', 'Second Label', 'Second Description');
      component.listItem = listItem2;
      fixture.detectChanges();

      expect(component.label()).toBe('Second Label');
      expect(component.description()).toBe('Second Description');
    });
  });

  describe('Template Structure', () => {
    it('should have container div with correct classes', () => {
      const compiled = fixture.nativeElement as HTMLElement;
      const containerDiv = compiled.querySelector('div.inline-block.mx-1.w-full');

      expect(containerDiv).toBeTruthy();
    });

    it('should have label span with correct class', () => {
      const compiled = fixture.nativeElement as HTMLElement;
      const labelSpan = compiled.querySelector('span.m-0');

      expect(labelSpan).toBeTruthy();
    });

    it('should have description with text-ellipsis class when shown', () => {
      // Create new fixture with signals already set
      fixture = TestBed.createComponent(SelectListItemComponent);
      component = fixture.componentInstance;
      component.label = signal('Label');
      component.description = signal('Description');
      fixture.detectChanges();

      const compiled = fixture.nativeElement as HTMLElement;
      const descriptionP = compiled.querySelector('p.text-ellipsis');

      expect(descriptionP).toBeTruthy();
    });

    it('should have description with overflow-hidden class when shown', () => {
      // Create new fixture with signals already set
      fixture = TestBed.createComponent(SelectListItemComponent);
      component = fixture.componentInstance;
      component.label = signal('Label');
      component.description = signal('Description');
      fixture.detectChanges();

      const compiled = fixture.nativeElement as HTMLElement;
      const descriptionP = compiled.querySelector('p.overflow-hidden');

      expect(descriptionP).toBeTruthy();
    });
  });
});
