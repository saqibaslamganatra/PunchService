import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Top10PunchesWeeklyComponent } from './top10-punches-weekly.component';

describe('Top10PunchesWeeklyComponent', () => {
  let component: Top10PunchesWeeklyComponent;
  let fixture: ComponentFixture<Top10PunchesWeeklyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ Top10PunchesWeeklyComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Top10PunchesWeeklyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
