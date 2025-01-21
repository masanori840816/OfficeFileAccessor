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
