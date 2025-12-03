export class UpdateSurveyReport{
    constructor(
        public labourCharges:number|null=null,
        public partsCost:number|null=null,
        public depreciationCost:number|null=null,
        public accidentDetails:string|null=null
    ){}
}