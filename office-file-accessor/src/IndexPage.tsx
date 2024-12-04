import { useEffect } from "react"

export function IndexPage(): JSX.Element {
    useEffect(() => {
        fetch("/api/files", {
            mode: "cors",
            method: "GET"
        })
        .then(res => res.text())
        .then(res => console.log(res))
        .catch(err => console.error(err));
    }, []);


    return <h1>Hello World!</h1>
}