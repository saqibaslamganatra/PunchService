import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Top10PunchesQuarterlyComponent } from './top10-punches-quarterly.component';

describe('Top10PunchesQuarterlyComponent', () => {
  let component: Top10PunchesQuarterlyComponent;
  let fixture: ComponentFixture<Top10PunchesQuarterlyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ Top10PunchesQuarterlyComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Top10PunchesQuarterlyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
