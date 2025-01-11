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
        .then(res => res.json().then(result => {
            return { key: res.headers.get("User-Token"), result: JSON.parse(JSON.stringify(result)) };
        }))
        .then(res => {
            if(res.result.succeeded === true) {
                fetch(`${getServerUrl()}/api/files`, {
                    mode: "cors",
                    method: "GET",
                    headers: {
                        "Authorization": `Bearer ${res.key}`
                    }
                })
                .then(res => res.text())
                .then(res => console.log(`Result: ${res}`))
                .catch(err => console.error(err));
            } else {
                console.log("Failed login");
                
            }
        })
        .catch(err => console.error(err));
    }, []);


    return <h1>Hello World!</h1>
}