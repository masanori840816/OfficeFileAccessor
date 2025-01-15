import './App.css'
import {
  BrowserRouter as Router,
  Route,
  Routes,
  Link
} from "react-router-dom";
import { IndexPage } from './IndexPage';
import { RegisterPage } from './RegisterPage';
import { AuthenticationProvider } from './AuthenticationContext';
import { SigninPage } from './SigninPage';

function App() {

  return (
    <>
      <AuthenticationProvider>
        <Router basename='/officefiles'>

        <Link to="/">TOP</Link>
        <p>|</p>
        <Link to="/register">Register</Link>
        <Routes>
          <Route path="/signin" element={<SigninPage />} />
          <Route path="/" element={<IndexPage />} />
          <Route path="/register" element={<RegisterPage />} />
        </Routes  >
        </Router>
      </AuthenticationProvider>
    </>
  )
}

export default App
