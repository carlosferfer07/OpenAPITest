import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TheChartGenerator } from './the-chart-generator';

describe('TheChartGenerator', () => {
  let component: TheChartGenerator;
  let fixture: ComponentFixture<TheChartGenerator>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TheChartGenerator]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TheChartGenerator);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
