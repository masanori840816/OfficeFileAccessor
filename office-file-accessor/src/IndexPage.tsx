import { useEffect } from 'react';
import { getServerUrl } from './web/serverUrlGetter';
import { useAuthentication } from './auth/authenticationContext';
import * as authStatusChecker from './auth/authenticationStatusChecker';
import * as logs from './accessLogs/logWriter';

export function IndexPage(): JSX.Element {
    const authContext = useAuthentication();
    useEffect(() => {
            authStatusChecker.checkStatus(authContext);
        }, [authContext]);
    useEffect(() => {
        logs.writeAccessLog('IndexPage');
        fetch(`${getServerUrl()}/api/files`, {
            mode: 'cors',
            method: 'GET',
        })
        .then(res => res.text())
        .then(res => console.log(`Result: ${res}`))
        .catch(err => console.error(err));
    }, []);
    


    return <h1>Hello World!</h1>
}