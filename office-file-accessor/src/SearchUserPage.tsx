import { useEffect, useState } from 'react';
import { getServerUrl } from './web/serverUrlGetter';
import { SearchUser } from './auth/authenticationType';
import { SearchUserRow } from './components/SearchUserRow';

export function SearchUserPage(): JSX.Element {
    const [users, setUsers] = useState<SearchUser[]>([]);
    useEffect(() => {
        fetch(`${getServerUrl()}/api/users/search`, {
            mode: 'cors',
            method: 'GET',
        })
        .then(res => res.json())
        .then(res => {
            const newUsers = JSON.parse(JSON.stringify(res));
            if(newUsers?.length != null && newUsers.length <= 0) {
                setUsers([]);
            } else {
                setUsers(newUsers);
            }
        })
    }, []);
    return <>
        <section className='flex flex-row justify-between items-center w-[96%]  h-[24%] ml-[2%] mt-[2%]'>
            <div className='flex flex-row items-center border rounded-lg shadow-lg w-[86%] h-full'>
                <div className='w-[16%] h-[90%] ml-[1%]'>
                    <h2>Search User</h2>
                </div>
                <div className='flex flex-col justify-around items-start w-[80%] h-[90%] ml-[1%]'>
                    <div className='flex flex-row w-full h-[46%]'>
                        <div className='flex flex-col items-start justify-around w-[30%] h-full ml-[2%]'>
                            <div className='w-full h-[30%]'><label>Organization</label></div>
                            <div className='w-full h-[50%]'>
                                <input className='w-[90%] p-2 border rounded-sm' type='text' placeholder='Organization'></input></div>
                        </div>
                        <div className='flex flex-col items-start justify-around w-[30%] h-full ml-[2%]'>
                            <div className='w-full h-[30%]'><label>User Name</label></div>
                            <div className='w-full h-[50%]'>
                                <input className='w-[90%] p-2 border rounded-sm' type='text' placeholder='User Name'></input></div>
                        </div>
                        <div className='flex flex-col items-start justify-around w-[30%] h-full ml-[2%]'>
                            <div className='w-full h-[30%]'><label>Registered Date</label></div>
                            <div className='flex flex-row items-center justify-between w-full h-[50%]'>
                                <input className='w-[40%] p-2 border rounded-sm' type='date' placeholder='Registered from'></input>
                                <div>～</div>
                                <input className='w-[40%] p-2 border rounded-sm' type='date' placeholder='Registered to'></input>
                            </div>
                        </div>
                    </div>
                    
                    <div className='flex flex-row justify-end items-center w-full h-[46%]'>
                        <div>
                            <button className='min-w-[100px]'>Search</button>
                        </div>
                    </div>
                </div>
            </div>
            <div className='h-full'>
                <button className='min-w-[100px]'>Create</button>
            </div>
        </section>
        <section className='border rounded-lg shadow-lg w-[96%] h-[48%] p-[1%] ml-[2%] mt-[2%]'>
            <div className='flex flex-row items-center justify-between w-full h-[4vh]'>
                <div className='w-[20%] h-full ml-[2%]'>Organization</div>
                <div className='w-[20%] h-full ml-[2%]'>User Name</div>
                <div className='w-[20%] h-full ml-[2%]'>Last Update Date</div>
                <div className='w-[10%] h-full ml-[2%] mr-[2%]'></div>
            </div>
            {users.map((u, index) => (
                <SearchUserRow key={index} user={u}></SearchUserRow>
            ))}
        </section>
    </>
}