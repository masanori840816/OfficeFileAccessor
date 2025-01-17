import { useEffect, useState } from "react";
import { useAuthentication } from "./auth/authenticationContext";
import { useNavigate } from "react-router-dom";

export function UserPage(): JSX.Element {
    const [email, setEmail] = useState<string>("");
    const [password, setPassword] = useState<string>("");
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
    const handleEmailChanged = (event: React.ChangeEvent<HTMLInputElement>) => {
        setEmail(event.target.value);
    };
    const handlePasswordChanged = (event: React.ChangeEvent<HTMLInputElement>) => {
        setPassword(event.target.value);
    };
    const createUser = () => {

    };
    return <div>
        <h1>User</h1>
        <div>
        <input type="text" placeholder="Email" value={email}
            onChange={handleEmailChanged}></input>
        <input type="password" value={password}
            onChange={handlePasswordChanged}></input>
            <button onClick={createUser}>Create</button>
        </div>
    </div>
}