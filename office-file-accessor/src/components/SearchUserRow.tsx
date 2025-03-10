import { SearchUser } from "../auth/authenticationType"

export interface SearchUserRowProps {
    user: SearchUser
}
export const SearchUserRow: React.FC<SearchUserRowProps> = ({user}) => {
    const openEditPage = (userId: number) => {
        console.log(userId);
    }
    return <div className='flex flex-row items-center justify-between border rounded-lg shadow-sm w-full h-[7vh] mb-[4px]'>
        <div className='w-[20%] ml-[2%]'>{user.organization}</div>
        <div className='w-[20%] ml-[2%]'>{user.userName}</div>
        <div className='w-[20%] ml-[2%]'>{user.updateDateText}</div>
        <div className='flex flex-row items-center justify-between w-[10%] ml-[2%] mr-[2%]'>
            <button className='min-w-[80px]' onClick={() => openEditPage(user.id)}>Edit</button>
            {(user.useCount <= 0)? (
                <button className='min-w-[80px]'>Delete</button>
            ):(<div></div>) }
            
        </div>
    </div>
}