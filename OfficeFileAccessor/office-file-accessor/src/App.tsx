import './App.css'
import {
  BrowserRouter as Router,
  Route,
  Routes
} from 'react-router-dom';
import { IndexPage } from './IndexPage';
import { RegisterPage } from './RegisterPage';
import { SigninPage } from './SigninPage';
import { AuthenticationProvider } from './auth/AuthenticationProvider';
import { GlobalHeader } from './components/GlobalHeader';
import { UserPage } from './UserPage';
import { PreviewPage } from './PreviewPage';
import { SearchUserPage } from './SearchUserPage';
import { SearchWorksheetPage } from './SearchWorksheetPage';
import { UIActionPage } from './UIActionPage';

function App() {

  return (
    <>
      <AuthenticationProvider>
        <Router basename='/officefiles/'>
        <GlobalHeader />
        <Routes>
          <Route path='/pages/signin/' element={<SigninPage />} />
          <Route path='/pages/' element={<IndexPage />} />
          <Route path='/' element={<SearchWorksheetPage />} />
          <Route path='/pages/officefiles/' element={<SearchWorksheetPage />} />
          <Route path='/pages/officefiles/register/' element={<RegisterPage />} />
          <Route path='/pages/officefiles/preview/' element={<PreviewPage />} />
          <Route path='/pages/users/edit/' element={<UserPage />} />
          <Route path='/pages/users/' element={<SearchUserPage />} />
          <Route path='/pages/uievents/' element={<UIActionPage />} /> 
        </Routes  >
        </Router>
      </AuthenticationProvider>
    </>
  )
}

export default App
