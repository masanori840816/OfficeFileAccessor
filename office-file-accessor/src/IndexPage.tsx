import { useEffect } from "react"
import { getServerUrl } from "./web/serverUrlGetter";
import { hasAnyTexts } from "./texts/hasAnyTexts";
import { useAuthentication } from "./AuthenticationContext";

export function IndexPage(): JSX.Element {
    const users = useAuthentication();

    useEffect(() => {
        if(users == null || !hasAnyTexts(users.token)) {
            return;
        }
        fetch(`${getServerUrl()}/api/files`, {
            mode: "cors",
            method: "GET",
            headers: {
                "Authorization": `Bearer ${users.token}`
            }
        })
        .then(res => res.text())
        .then(res => console.log(`Result: ${res}`))
        .catch(err => console.error(err));
    }, [users]);
    


    return <h1>Hello World!</h1>
}