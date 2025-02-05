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
        <div className='h-[90%]'>{authContext?.signedIn != null ? (
            <button className="h-full p-[0em_1.0em]" onClick={signOut}>Sign Out</button>
        ):(
            <div></div>
        )}</div>
        
    </>
}