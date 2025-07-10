import { useEffect } from 'react';
import { useAuthentication } from './auth/authenticationContext';
import * as authStatusChecker from './auth/authenticationStatusChecker';

export function IndexPage(): JSX.Element {
    const authContext = useAuthentication();
    useEffect(() => {
        console.log("check login fron index");
            authStatusChecker.checkStatus(authContext);
        }, [authContext]);
    const requestNotificationPermission = async () => {
        const permission = await Notification.requestPermission();
        if (permission === 'granted') {
            console.log('Notification permission granted.');
            navigator.serviceWorker.ready.then(function(registration) {
                registration.showNotification('Hello Web notifications!', {
                        body: 'This is a notification with Service Worker',
                        icon: 'img/home.png'
                    });
            });
        } else {
            console.log('Notification permission denied.');
        }
    };

    return <>
        <h1>Hello World!</h1>
        <button onClick={requestNotificationPermission}>Enable Notifications</button>
    </>
}