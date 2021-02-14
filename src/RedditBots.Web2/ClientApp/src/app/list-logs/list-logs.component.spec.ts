import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ListLogsComponent } from './list-logs.component';

describe('ListLogsComponent', () => {
  let component: ListLogsComponent;
  let fixture: ComponentFixture<ListLogsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ListLogsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListLogsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
