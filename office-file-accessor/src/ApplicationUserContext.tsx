import { createContext, ReactNode, useContext, useEffect, useState } from "react";
import { getServerUrl } from "./web/serverUrlGetter";
import { hasAnyTexts } from "./texts/hasAnyTexts";
import { ApplicationResult } from "./officeFileAccessor.type";

type LoginToken = {
    token: string | null;
    setToken: (token: string) => void;
}
export const ApplicationUserContext = createContext<LoginToken|null>(null);

export const ApplicationUserProvider = ({children}: { children: ReactNode }) => {
    const [token, setToken] = useState<string|null>(null);
    useEffect(() => {
        fetch(`${getServerUrl()}/api/users/signin`, {
            mode: "cors",
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({ email: "default@example.com", password: "oXc5rZbz"})
        }).then(res => getLoginResult(res))
        .then(res => {
            if(res.result.succeeded) {
                if(hasAnyTexts(res.token)) {
                    setToken(res.token);
                } else {
                    console.log("Failed login: No tokens");
                    setToken(null);
                }
            } else {
                console.log("Failed login");
                setToken(null);
            }
        })

        
    }, []);
    return <ApplicationUserContext.Provider value={{ token, setToken }}>
        {children}
    </ApplicationUserContext.Provider>
}
export const useApplicationUser = (): LoginToken|null => useContext(ApplicationUserContext);

async function getLoginResult(res: Response): Promise<{result: ApplicationResult, token: string|null}> {
    try {
        const result = JSON.parse(JSON.stringify(await res.json()));
        return {
            result,
            token: res.headers.get("User-Token"),
        };
    }catch(err) {
        console.error(err);
        return {
            result: {
                succeeded: false,
                errorMessage: "Something wrong"
            },
            token: null,
        };
    }
}