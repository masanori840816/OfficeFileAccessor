import { useEffect } from "react"
import { getServerUrl } from "./web/serverUrlGetter";
import { hasAnyTexts } from "./texts/hasAnyTexts";

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
        .then(res => res.json()
            .then(r => {
                const result = JSON.parse(JSON.stringify(r));
                if(result.succeeded === true) {
                    const token = res.headers.get("User-Token");
                    if(hasAnyTexts(token)) {
                        fetch(`${getServerUrl()}/api/files`, {
                            mode: "cors",
                            method: "GET",
                            headers: {
                                "Authorization": `Bearer ${token}`
                            }
                        })
                        .then(res => res.text())
                        .then(res => console.log(`Result: ${res}`))
                        .catch(err => console.error(err));
                    } else {
                        console.log("Failed login: No tokens");
                    }
                    
                } else {
                    console.log("Failed login");
                    
                }
            }))
        .catch(err => console.error(err));
    }, []);


    return <h1>Hello World!</h1>
}