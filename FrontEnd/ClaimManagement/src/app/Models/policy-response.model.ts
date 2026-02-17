export class PolicyResponse{
    constructor(
        public policyNo:string,
        public insuredFirstName:string,
        public insuredLastName:string,
        public dateOfInsurance:Date,
        public vehicleNo:string,
        public status:boolean,
    ){}
}