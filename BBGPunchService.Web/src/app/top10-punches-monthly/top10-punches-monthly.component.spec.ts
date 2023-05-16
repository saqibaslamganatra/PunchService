import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Top10PunchesMonthlyComponent } from './top10-punches-monthly.component';

describe('Top10PunchesMonthlyComponent', () => {
  let component: Top10PunchesMonthlyComponent;
  let fixture: ComponentFixture<Top10PunchesMonthlyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ Top10PunchesMonthlyComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Top10PunchesMonthlyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
