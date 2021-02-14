import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChartbarComponent } from './chartbar.component';

describe('ChartbarComponent', () => {
  let component: ChartbarComponent;
  let fixture: ComponentFixture<ChartbarComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChartbarComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChartbarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
