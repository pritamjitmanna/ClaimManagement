export class Surveyor{
    constructor(
        public surveyorUserId:string,
        public firstName:string,
        public lastName:string,
        public estimateLimit:number,
        public timesAllocated:number
    ){}
}