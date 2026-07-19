import { ClaimStatus, WITHDRAWSTATUS } from "./e.enum";


export class ClaimDetail{
    constructor(
        public claimId:string,
        public policyNo:string,
        public estimatedLoss:number,
        public dateOfAccident:Date,
        public amtApprovedBySurveyor:number|null=null, 
        public insuranceCompanyApproval:boolean, 
        public withdrawClaim:WITHDRAWSTATUS, 
        public claimStatus:ClaimStatus,
        public surveyorUserId:string|null=null,
        public surveyorFees:number|null=null
    ){}
}