import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateClaimModalComponent } from './update-claim-modal.component';

describe('UpdateClaimModalComponent', () => {
  let component: UpdateClaimModalComponent;
  let fixture: ComponentFixture<UpdateClaimModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateClaimModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UpdateClaimModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
