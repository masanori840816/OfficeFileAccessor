import { useEffect, useState } from "react";
import { useAuthentication } from "./auth/authenticationContext";
import * as authStatusChecker from "./auth/authenticationStatusChecker";

export function UserPage(): JSX.Element {
    const [organization, setOrganization] = useState<string>("");
    const [userName, setUserName] = useState<string>("");
    const [email, setEmail] = useState<string>("");
    const [password, setPassword] = useState<string>("");
    const authContext = useAuthentication();
    useEffect(() => {
        authStatusChecker.checkStatus(authContext);
    }, [authContext]);
    const handleOrganizationChanged = (event: React.ChangeEvent<HTMLInputElement>) => {
        setOrganization(event.target.value);
    };
    const handleUserNameChanged = (event: React.ChangeEvent<HTMLInputElement>) => {
        setUserName(event.target.value);
    };
    const handleEmailChanged = (event: React.ChangeEvent<HTMLInputElement>) => {
        setEmail(event.target.value);
    };
    const handlePasswordChanged = (event: React.ChangeEvent<HTMLInputElement>) => {
        setPassword(event.target.value);
    };
    const createUser = () => {
        
    };
    return <div className="flex justify-center items-start mt-[2%] h-[80%] w-full">
            <div className="h-[80%] w-[96%] max-w-[2048px] max-h-[800px]">
                <h1 className="page-title">Create User</h1>
                <div className="flex flex-col justify-around items-start w-[80%] h-[60%] border rounded-lg shadow-lg pl-[3%]">
                    <div className="flex flex-row justify-start items-center w-[70%] h-[30%]">
                        <div className="w-[30%] h-full mr-[2%]">
                            <div>Organization</div>
                            <input type="text" className="border w-full h-[40%] mt-[2%] pl-[0.5em]" placeholder="Organization"
                                value={organization} onChange={handleOrganizationChanged}></input>
                        </div>
                        <div className="w-[30%] h-full mr-[2%]">
                            <div>User Name</div>
                            <input type="text" className="border w-full h-[40%] mt-[2%] pl-[0.5em]" placeholder="User Name"
                                value={userName} onChange={handleUserNameChanged}></input>
                        </div>
                    </div>
                    <div className="flex flex-row justify-start items-center w-[70%] h-[30%]">
                        <div className="w-[30%] h-full mr-[2%]">
                            <div>E-Mail</div>
                            <input type="text" className="border w-full h-[40%] mt-[2%] pl-[0.5em]" placeholder="E-Mail"
                                value={email} onChange={handleEmailChanged}></input>
                        </div>
                        <div className="w-[30%] h-full mr-[2%]">
                            <div>Password</div>
                            <input type="password" className="border w-full h-[40%] mt-[2%] pl-[0.5em]" placeholder="Password"
                                value={password} onChange={handlePasswordChanged}></input>
                        </div>
                    </div>
                    <div className="flex flex-row justify-end items-center w-[90%] h-[30%]">
                        <button className="min-w-[120px]" onClick={createUser}>Create</button>
                    </div>
                </div>
            </div>
        </div>
}