import { ReactNode } from "react";
import { getServerUrl } from "../web/serverUrlGetter";
import { AuthenticationContext } from "./authenticationContext";
import { getCookieValue } from "../web/cookieValues";
import { hasAnyTexts } from "../texts/hasAnyTexts";

export const AuthenticationProvider = ({children}: { children: ReactNode }) => {
    const signin = async (email: string, password: string) => {
        const cookieValue = getCookieValue("XSRF-TOKEN"); 
        
        if(!hasAnyTexts(cookieValue)) {
            throw Error("Invalid token");
        }
        //setCookieValue("X-CSRF-TOKEN-HEADERNAME", cookieValue);
        const res = await fetch(`${getServerUrl()}/api/users/signin`, {
            mode: "cors",
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "X-XSRF-TOKEN": cookieValue,
            },
            body: JSON.stringify({ email, password })
        });
        return await res.json();
    };
        
    const signout = () =>
        fetch(`${getServerUrl()}/api/users/signin`, {
            mode: "cors",
            method: "GET",
        }).then(res => res.json());
    const check = () => 
        fetch(`${getServerUrl()}/api/auth`, {
            mode: "cors",
            method: "GET",
        })
        .then(res => res.ok);
    return <AuthenticationContext.Provider value={{ signin, signout, check }}>
        {children}
    </AuthenticationContext.Provider>
}
