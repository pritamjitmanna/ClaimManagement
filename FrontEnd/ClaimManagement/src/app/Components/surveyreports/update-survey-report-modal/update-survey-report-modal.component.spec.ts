import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateSurveyReportModalComponent } from './update-survey-report-modal.component';

describe('UpdateSurveyReportModalComponent', () => {
  let component: UpdateSurveyReportModalComponent;
  let fixture: ComponentFixture<UpdateSurveyReportModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateSurveyReportModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UpdateSurveyReportModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
