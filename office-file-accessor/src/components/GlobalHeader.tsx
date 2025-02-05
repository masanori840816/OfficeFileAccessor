
import { Link } from "react-router-dom";
import { useAuthentication } from "../auth/authenticationContext";
import "./GlobalHeader.css";
import { SignOutButton } from "./SignOutButton";

export function GlobalHeader(): JSX.Element {
    
    const authContext = useAuthentication();
    return <>
        {authContext?.signedIn != null ? (
            <header className='w-full h-[14%] min-h-[100px] shadow-xl'>
            <div className='flex flex-row w-full justify-between h-[60%]'>
                <div className='text-3xl p-[1%] font-bold'>OfficeFileAccessor</div>
                <div className='flex flex-col items-center justify-around h-full pr-[1%]'>
                <div>
                    Hello {authContext.signedIn.userName}!
                </div>
                <div className='h-[50%]'>
                    <SignOutButton />
                </div>
                </div>
            </div>
            <div className='w-full h-[38%] flex flex-row items-center justify-center'>
                <Link to='/pages/register' className='h-full w-[18%]'>
                    <div className='header-tab mr-[2%]'>File</div>
                </Link>
                <Link to='/pages/' className='h-full w-[18%]'>
                    <div className='header-tab mr-[2%]'>Output</div>
                </Link>
                <Link to='/pages/user' className='h-full w-[18%]'>
                    <div className='header-tab'>User</div>
                </Link>                
            </div>
            </header>
        ):(
            <div></div>
        )}
    </>
}