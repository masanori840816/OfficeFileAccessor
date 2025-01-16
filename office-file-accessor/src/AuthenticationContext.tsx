import { createContext, ReactNode, useContext } from "react";
import { getServerUrl } from "./web/serverUrlGetter";
import { ApplicationResult } from "./officeFileAccessor.type";

type AuthenticationType = {
    signin: (email: string, password: string) => Promise<ApplicationResult>,
    signout: () => Promise<ApplicationResult>
}
export const AuthenticationContext = createContext<AuthenticationType|null>(null);

export const AuthenticationProvider = ({children}: { children: ReactNode }) => {
    const signin = (email: string, password: string) =>
        fetch(`${getServerUrl()}/api/users/signin`, {
            mode: "cors",
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({ email, password })
        })
        .then(res => res.json());
    const signout = () =>
        fetch(`${getServerUrl()}/api/users/signin`, {
            mode: "cors",
            method: "GET",
        }).then(res => res.json());
    return <AuthenticationContext.Provider value={{ signin, signout }}>
        {children}
    </AuthenticationContext.Provider>
}
export const useAuthentication = (): AuthenticationType|null => useContext(AuthenticationContext);
