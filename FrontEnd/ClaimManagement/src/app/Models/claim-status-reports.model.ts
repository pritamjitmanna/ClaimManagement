import { Stages } from "./e.enum";

export class ClaimStatusReport
{
    constructor(public stage:Stages,public count:number){}
}