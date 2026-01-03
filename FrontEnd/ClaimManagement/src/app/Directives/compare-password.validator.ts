import { AbstractControl, FormGroup, ValidationErrors } from "@angular/forms";



export function comparePasswordValidator(group:AbstractControl):ValidationErrors | null{
    const password=group.get('Password')?.value;
    const cPassword=group.get('cPassword')?.value;
    return password===cPassword ? null : {notMatching:true}
    
}