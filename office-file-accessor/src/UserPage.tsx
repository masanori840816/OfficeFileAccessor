import { useEffect, useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { useAuthentication } from './auth/authenticationContext';
import { getServerUrl } from './web/serverUrlGetter';
import * as numbers from './numbers/parseNumbers';
import * as authStatusChecker from './auth/authenticationStatusChecker';
import { DisplayUser, UpdateUser } from './officeFileAccessor.type';
import { hasAnyTexts } from './texts/hasAnyTexts';

export function UserPage(): JSX.Element {
    const [userId, setUserId] = useState(-1);
    const [organization, setOrganization] = useState<string>('');
    const [userName, setUserName] = useState<string>('');
    const [email, setEmail] = useState<string>('');
    const [password, setPassword] = useState<string>('');
    const authContext = useAuthentication();
    const search = useLocation().search;
    const navigate = useNavigate();
    useEffect(() => {
        authStatusChecker.checkStatus(authContext);
    }, [authContext]);useEffect(() => {
        const query = new URLSearchParams(search);
        const newUserId = numbers.tryParseInt(query.get('userid'));
        if(newUserId != null) {
            setUserId(newUserId);
        } else {
            setUserId(-1);
        }
    }, [search]);
    useEffect(() => {
        if(userId < 0) {
            setOrganization('');
            setUserName('');
            setEmail('');
            setPassword('');
        } else {
            fetch(`${getServerUrl()}/api/users/edit?userId=${userId}`, {
                mode: 'cors',
                method: 'GET'
            })
            .then(res => res.json())
            .then(res => {
                const newUser = JSON.parse(JSON.stringify(res)) as DisplayUser;                
                setOrganization(newUser?.organization ?? ''),
                setUserName(newUser?.userName ?? '');
                setEmail(newUser?.email ?? '');
                setPassword('');
            })
            .catch(err => {
                console.error(err);
                alert('Failed updating user');
            });
        }
    }, [userId]);
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
    const saveUser = () => {
        if(!hasAnyTexts(userName)) {
            alert('User name is required');
            return;
        }
        if(!hasAnyTexts(email)) {
            alert('Email is required');
            return;
        }
        if(!hasAnyTexts(password)) {
            alert('Password is required');
            return;
        }
        const newUser: UpdateUser = {
            id: userId,
            userName,
            organization,
            email,
            password,
        }
        fetch(`${getServerUrl()}/api/users/edit`, {
            mode: 'cors',
            method: 'POST',
            body: JSON.stringify(newUser),
        })
        .then(res => res.json())
        .then(res => {
            const result = JSON.parse(JSON.stringify(res));
            if(result?.succeeded === true) {
                alert('User updated');
            } else if(hasAnyTexts(result?.errorMessage)) {
                alert(result.errorMessage);
            } else {
                alert('Failed getting user');
            }
        })
        .catch(err => {
            console.error(err);
            alert('Failed updating user');
        });
    };
    const cancel = () => {
        navigate('/pages/user/');
    }
    return <div className='flex justify-center items-start mt-[2%] h-[80%] w-full'>
            <div className='h-[80%] w-[96%] max-w-[2048px] max-h-[800px]'>
                {(userId < 0)? (
                    <h1 className='page-title'>Create User</h1>
                ):(
                    <h1 className='page-title'>Edit User</h1>
                )}                
                <div className='flex flex-col justify-around items-start w-[80%] h-[60%] border rounded-lg shadow-lg pl-[3%]'>
                    <div className='flex flex-row justify-start items-center w-[70%] h-[30%]'>
                        <div className='w-[30%] h-full mr-[2%]'>
                            <div>Organization</div>
                            <input type='text' className='border w-full h-[40%] mt-[2%] pl-[0.5em]' placeholder='Organization'
                                value={organization} onChange={handleOrganizationChanged}></input>
                        </div>
                        <div className='w-[30%] h-full mr-[2%]'>
                            <div>User Name</div>
                            <input type='text' className='border w-full h-[40%] mt-[2%] pl-[0.5em]' placeholder='User Name'
                                value={userName} onChange={handleUserNameChanged}></input>
                        </div>
                    </div>
                    <div className='flex flex-row justify-start items-center w-[70%] h-[30%]'>
                        <div className='w-[30%] h-full mr-[2%]'>
                            <div>E-Mail</div>
                            <input type='text' className='border w-full h-[40%] mt-[2%] pl-[0.5em]' placeholder='E-Mail'
                                value={email} onChange={handleEmailChanged}></input>
                        </div>
                        <div className='w-[30%] h-full mr-[2%]'>
                            <div>Password</div>
                            <input type='password' className='border w-full h-[40%] mt-[2%] pl-[0.5em]' placeholder='Password'
                                value={password} onChange={handlePasswordChanged}></input>
                        </div>
                    </div>
                    <div className='flex flex-row justify-end items-center w-[90%] h-[30%]'>
                        <div className='flex flex-row justify-between items-center w-[20%] h-[80%]'>
                            <button className='min-w-[120px]' onClick={saveUser}>Save</button>
                            <button className='min-w-[120px]' onClick={cancel}>Cancel</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
}