import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Top10PunchesYearlyComponent } from './top10-punches-yearly.component';

describe('Top10PunchesYearlyComponent', () => {
  let component: Top10PunchesYearlyComponent;
  let fixture: ComponentFixture<Top10PunchesYearlyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ Top10PunchesYearlyComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Top10PunchesYearlyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
