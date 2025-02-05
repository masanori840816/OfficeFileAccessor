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
    return <div className="flex justify-center items-center h-full w-full">
        <div className="flex flex-col justify-around items-center h-[46%]  w-[40%] rounded-lg shadow-xl max-w-[700px] max-h-[600px] border">
            <h1>Sign In</h1>
            <div className="flex flex-col justify-around items-center h-[70%] w-full">
                <div className="h-[26%] w-[60%]">
                    <div>Mail</div>
                    <input type="email" className="border h-[40%] w-full mt-[1%]" placeholder="Email" value={email}
                        onChange={handleEmailChanged}></input>
                </div>
                <div className="h-[26%] w-[60%]">
                    <div>Password</div>
                    <input type="password" className="border h-[40%] w-full mt-[1%]" value={password}
                        onChange={handlePasswordChanged}></input>
                </div>
                <div className="flex flex-col justify-end items-end h-[20%] w-[60%]">
                    <button className="w-[30%]" onClick={signin}>Sign in</button>
                </div>
            </div>        
        </div>
    </div>
}