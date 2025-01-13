import './App.css'
import {
  BrowserRouter as Router,
  Route,
  Routes,
  Link
} from "react-router-dom";
import { IndexPage } from './IndexPage';
import { RegisterPage } from './RegisterPage';
import { ApplicationUserProvider } from './ApplicationUserContext';

function App() {

  return (
    <>
      <ApplicationUserProvider>
        <Router basename='/officefiles'>

        <Link to="/">TOP</Link>
        <p>|</p>
        <Link to="/register">Register</Link>
        <Routes>
          <Route path="/" element={<IndexPage />} />
          <Route path="/register" element={<RegisterPage />} />
        </Routes  >
        </Router>
      </ApplicationUserProvider>
    </>
  )
}

export default App
