export enum RESULT{
    FAILURE="FAILURE",
    SUCCESS="SUCCESS"
}

export enum ClaimStatus
{
    Open = "Open",
    Closed = "Closed"
}

export enum WITHDRAWSTATUS
{
    NOSTATUS = 0,
    ACCEPTED = 1,
    WITHDRAWN = 2
}

export enum Stages
{
	NewClaims,
	PendingClaims,
	FinalizedClaims
}

export enum Roles{
    Admin="Admin",
    InsuranceCompany="InsuranceCompany",
    Surveyor="Surveyor",
    IRDA="IRDA",
    Insurer="Insurer"
}