import { useAuthentication } from "../auth/authenticationContext";

export function SignOutButton(): JSX.Element {
    const authContext = useAuthentication();
    const signOut = () => {
        if(authContext == null) {
            console.error("No context");
            return;
        }
        authContext.signOut()
            .then(res => {
                if(res === true) {
                    // reload & open the sign in page
                    location.href = "/officefiles/signin/";
                } else {
                    console.error("Something wrong");                    
                }
                
            })
            .catch(err => console.error(err));
    };
    return <>
        <div>{authContext?.signedIn === true ? (
            <button onClick={signOut}>Sign Out</button>
        ):(
            <div></div>
        )}</div>
        
    </>
}