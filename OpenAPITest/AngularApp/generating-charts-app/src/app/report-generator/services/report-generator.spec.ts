import { TestBed } from '@angular/core/testing';

import { ReportGenerator } from './report-generator';

describe('ReportGenerator', () => {
  let service: ReportGenerator;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ReportGenerator);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
