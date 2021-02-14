import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SettingsbarComponent } from './settingsbar.component';

describe('SettingsbarComponent', () => {
  let component: SettingsbarComponent;
  let fixture: ComponentFixture<SettingsbarComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SettingsbarComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SettingsbarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
