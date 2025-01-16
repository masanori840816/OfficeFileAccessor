import { useEffect } from "react"
import { getServerUrl } from "./web/serverUrlGetter";

export function IndexPage(): JSX.Element {
    
    useEffect(() => {
        
        fetch(`${getServerUrl()}/api/files`, {
            mode: "cors",
            method: "GET",
        })
        .then(res => res.text())
        .then(res => console.log(`Result: ${res}`))
        .catch(err => console.error(err));
    }, []);
    


    return <h1>Hello World!</h1>
}