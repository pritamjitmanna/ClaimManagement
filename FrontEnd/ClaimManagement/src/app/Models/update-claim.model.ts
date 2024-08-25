import { ClaimStatus } from "./e.enum";

export class UpdateClaim{
    constructor(
        public InsuranceCompanyApproval:boolean|null=null, 
        public ClaimStatus:ClaimStatus|null=null,
        public SurveyorID:number|null=null,
    ){}
}