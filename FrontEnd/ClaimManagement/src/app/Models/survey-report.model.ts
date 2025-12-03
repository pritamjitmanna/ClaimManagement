export class SurveyReport{
    constructor(
        public claimId:string,
        public policyNo:string,
        public labourCharges:number,
        public partsCost:number,
        public policyClause:number,
        public depreciationCost:number,
        public totalAmount:number,
        public accidentDetails:string|null=null
    ){}
}