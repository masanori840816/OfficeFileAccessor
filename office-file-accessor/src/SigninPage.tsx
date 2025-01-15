import { useState } from "react"

export function SigninPage(): JSX.Element {
    const [userName, setUserName] = useState<string>("");
    const [password, setPassword] = useState<string>("");
    const signin = () => {

    }
    const handleUserNameChanged = (event: React.ChangeEvent<HTMLInputElement>) => {
        setUserName(event.target.value);
    }
    const handlePasswordChanged = (event: React.ChangeEvent<HTMLInputElement>) => {
        setPassword(event.target.value);
    }
    return <div>
        <input type="text" placeholder="Username" value={userName}
            onChange={handleUserNameChanged}></input>
        <input type="password" value={password}
            onChange={handlePasswordChanged}></input>
        <button onClick={signin}>Signin</button>
    </div>
}