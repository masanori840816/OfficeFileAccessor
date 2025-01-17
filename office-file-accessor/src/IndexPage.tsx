import { useEffect } from "react"
import { getServerUrl } from "./web/serverUrlGetter";
import { useAuthentication } from "./auth/authenticationContext";
import { useNavigate } from "react-router-dom";

export function IndexPage(): JSX.Element {
    const authContext = useAuthentication();
    const navigate = useNavigate();
    useEffect(() => {
        authContext?.check()
            .then(res => {
                if(res !== true) {
                    navigate("/signin");
                }
            })
            .catch(err => {
                console.error(err);
                navigate("/signin");
            })
    }, [authContext, navigate]);
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