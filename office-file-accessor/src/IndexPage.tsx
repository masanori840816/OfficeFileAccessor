import { useEffect } from "react"
import { getServerUrl } from "./web/serverUrlGetter";

export function IndexPage(): JSX.Element {
    useEffect(() => {
        fetch(`${getServerUrl()}/api/users/signin`, {
            mode: "cors",
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({ email: "default@example.com", password: "oXc5rZbz"})
        })
        .then(res => res.json())
        .then(res => console.log(res))
        .catch(err => console.error(err));
    }, []);


    return <h1>Hello World!</h1>
}