import { TestBed } from '@angular/core/testing';

import { SignalrService } from './signalr.service';

describe('SignalrService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: SignalrService = TestBed.get(SignalrService);
    expect(service).toBeTruthy();
  });
});
