import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PunchingDataComponent } from './punching-data.component';

describe('PunchingDataComponent', () => {
  let component: PunchingDataComponent;
  let fixture: ComponentFixture<PunchingDataComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PunchingDataComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PunchingDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
