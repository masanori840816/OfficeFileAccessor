import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuthentication } from "./auth/authenticationContext";

export function SigninPage(): JSX.Element {
    const [email, setEmail] = useState<string>("");
    const [password, setPassword] = useState<string>("");
    const authContext = useAuthentication();
    const navigate = useNavigate();
    const handleEmailChanged = (event: React.ChangeEvent<HTMLInputElement>) => {
        setEmail(event.target.value);
    }
    const handlePasswordChanged = (event: React.ChangeEvent<HTMLInputElement>) => {
        setPassword(event.target.value);
    }
    const signin = () => {
        if(authContext == null) {
            console.error("No Auth context");
            return;
        }
        authContext.signIn(email, password)
            .then(res => {
                if(res.succeeded) {
                    navigate("/");
                }
                console.log(res);
                
            })
            .catch(err => console.error(err));
    };
    return <div>
        <h1>Signin</h1>
        <input type="text" placeholder="Email" value={email}
            onChange={handleEmailChanged}></input>
        <input type="password" value={password}
            onChange={handlePasswordChanged}></input>
        <button onClick={signin}>Signin</button>
    </div>
}