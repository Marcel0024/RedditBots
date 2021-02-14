import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OverviewbarComponent } from './overviewbar.component';

describe('OverviewbarComponent', () => {
  let component: OverviewbarComponent;
  let fixture: ComponentFixture<OverviewbarComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OverviewbarComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OverviewbarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
