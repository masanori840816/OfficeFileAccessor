import { AuthenticationType } from "./authenticationType";

export async function checkStatus(authContext: AuthenticationType|null): Promise<void> {
    if(authContext == null || authContext?.signedIn == null) {        
        location.href = "/officefiles/pages/signin/";
        return;
    }
    try {
        const res = await authContext.check();
        if(res !== true) {
            location.href = "/officefiles/pages/signin/";
        }
    }catch(err) {
        console.error(err);
        location.href = "/officefiles/pages/signin/";
    }
}