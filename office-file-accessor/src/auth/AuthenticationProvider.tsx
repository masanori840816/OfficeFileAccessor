import { ReactNode, useState } from "react";
import { getServerUrl } from "../web/serverUrlGetter";
import { AuthenticationContext } from "./authenticationContext";
import { getCookieValue } from "../web/cookieValues";
import { hasAnyTexts } from "../texts/hasAnyTexts";
import { SignedInUser } from "./authenticationType";

export const AuthenticationProvider = ({children}: { children: ReactNode }) => {
    const [signedIn, setSignedIn] = useState<SignedInUser|null>(null);
    const signIn = async (email: string, password: string) => {
        const cookieValue = getCookieValue("XSRF-TOKEN"); 
        
        if(!hasAnyTexts(cookieValue)) {
            throw Error("Invalid token");
        }
        const res = await fetch(`${getServerUrl()}/api/users/signin`, {
            mode: "cors",
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "X-XSRF-TOKEN": cookieValue,
            },
            body: JSON.stringify({ email, password })
        });
        if(res.ok) {
            const result = await res.json();
            if(result?.result?.succeeded === true &&
                hasAnyTexts(result?.user?.userName)) {
                setSignedIn(result.user);
                return result.result;
            } else if(result?.result != null) {
                setSignedIn(null);
                return result.result;
            }            
        }
        return {
            succeeded: false,
            errorMessage: "Something wrong"
        };
    };
        
    const signOut = async () => {
        const res = await fetch(`${getServerUrl()}/api/users/signout`, {
            mode: "cors",
            method: "GET",
        });
        if(res.ok) {
            setSignedIn(null);
            return true;
        };
        return false;
    };
        
    const check = () => 
        fetch(`${getServerUrl()}/api/auth`, {
            mode: "cors",
            method: "GET",
        })
        .then(res => res.ok);
    return <AuthenticationContext.Provider value={{ signedIn, signIn, signOut, check }}>
        {children}
    </AuthenticationContext.Provider>
}
