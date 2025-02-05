import './App.css'
import {
  BrowserRouter as Router,
  Route,
  Routes
} from "react-router-dom";
import { IndexPage } from './IndexPage';
import { RegisterPage } from './RegisterPage';
import { SigninPage } from './SigninPage';
import { AuthenticationProvider } from './auth/AuthenticationProvider';
import { GlobalHeader } from './components/GlobalHeader';
import { UserPage } from './UserPage';

function App() {

  return (
    <>
      <AuthenticationProvider>
        <Router basename='/officefiles'>
        <GlobalHeader />
        <Routes>
          <Route path="/pages/signin" element={<SigninPage />} />
          <Route path="/" element={<IndexPage />} />
          <Route path="/pages/" element={<IndexPage />} />
          <Route path="/pages/register" element={<RegisterPage />} />
          <Route path="/pages/user" element={<UserPage />} />
        </Routes  >
        </Router>
      </AuthenticationProvider>
    </>
  )
}

export default App
