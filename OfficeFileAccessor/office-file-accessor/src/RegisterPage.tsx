import { useEffect, useState } from 'react';
import { getServerUrl } from './web/serverUrlGetter';
import { getCookieValue } from './web/cookieValues';
import { useAuthentication } from './auth/authenticationContext';
import * as authStatusChecker from './auth/authenticationStatusChecker';
import * as logs from './accessLogs/logWriter';
import { hasAnyTexts } from './texts/hasAnyTexts';
import { ApplicationResult } from './officeFileAccessor.type';

export function RegisterPage(): JSX.Element {
  const [files, setFiles] = useState<FileList|null>(null);
  const authContext = useAuthentication();
  useEffect(() => {
    logs.writeAccessLog('RegisterPage');
}, []);
  useEffect(() => {
          authStatusChecker.checkStatus(authContext);
      }, [authContext]);
  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setFiles(event.target.files);
  };

  const handleUpload = async () => {
    if (!files) {
      alert('Please select files first!');
      return;
    }
    if(authContext == null) {
      console.error('No auth context');
      return;
    }
    try {
      // check sign-in and get XSRF-Token
      await authStatusChecker.checkStatus(authContext);
      const cookieValue = getCookieValue('XSRF-TOKEN');
      if(!hasAnyTexts(cookieValue)) {
          throw Error('Invalid token');
      }
      const formData = new FormData();
      Array.from(files).forEach((file) => {
        formData.append('files', file);
      });
      const res = await fetch(`${getServerUrl()}/api/files`, {
        mode: 'cors',
        method: 'POST',
        headers: {
          'X-XSRF-TOKEN': cookieValue,
        },
        body: formData,
      });
      const result = JSON.parse(JSON.stringify(await res.json())) as ApplicationResult;
      if(result?.succeeded === true) {
        console.log('OK');        
      } else if(hasAnyTexts(result?.errorMessage)){
        console.log(result?.errorMessage);
      } else {
        console.error('something wrong');
      }
    } catch(err) {
      console.error('Upload failed', err);
    }
  };

  return (
    <div>
        <h1>Register</h1>
      <input type='file' multiple onChange={handleFileChange} />
      <button onClick={handleUpload}>Upload</button>
      
    </div>
  );
}