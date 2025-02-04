import './App.css'
import {
  BrowserRouter as Router,
  Route,
  Routes,
  Link
} from "react-router-dom";
import { IndexPage } from './IndexPage';
import { RegisterPage } from './RegisterPage';
import { SigninPage } from './SigninPage';
import { AuthenticationProvider } from './auth/AuthenticationProvider';
import { SignOutButton } from './components/SignOutButton';

function App() {

  return (
    <>
      <AuthenticationProvider>
        <Router basename='/officefiles'>
        <header className='w-full h-[14%] min-h-[100px] shadow-xl'>
          <div className='flex flex-row w-full justify-between h-[60%]'>
            <div className='text-3xl p-[1%] font-bold'>OfficeFileAccessor</div>
            <div className='flex flex-col items-center justify-around h-full pr-[1%]'>
              <div>
                Hello UserName!
              </div>
              <div className='h-[50%]'>
                <button className='h-[90%] p-[0em_1.0em]'>Signout</button>
              </div>
            </div>
          </div>
          <div className='w-full h-[38%] flex flex-row items-center justify-center'>
            <input type='radio' radioGroup='1' id='header-tab-file' className='hidden'></input>
            <label htmlFor='header-tab-file'>
              <div className='header-tab mr-[2%]'>File</div>
            </label>
            <input type='radio' radioGroup='1' id='header-tab-output' className='hidden'></input>
            <label htmlFor='header-tab-output'>
              <div className='header-tab mr-[2%]'>Output</div>
            </label>
            <input type='radio' radioGroup='1' id='header-tab-user' className='hidden'></input>
            <label htmlFor='header-tab-use'>
              <div className='header-tab'>User</div>
            </label>
          </div>
        </header>
        <SignOutButton />
        <Link to="/pages/">TOP</Link>
        <p>|</p>
        <Link to="/pages/register">Register</Link>
        <Routes>
          <Route path="/pages/signin" element={<SigninPage />} />
          <Route path="/" element={<IndexPage />} />
          <Route path="/pages/" element={<IndexPage />} />
          <Route path="/pages/register" element={<RegisterPage />} />
        </Routes  >
        </Router>
      </AuthenticationProvider>
    </>
  )
}

export default App
