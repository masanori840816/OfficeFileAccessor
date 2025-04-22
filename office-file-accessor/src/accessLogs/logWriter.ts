import { getServerUrl } from '../web/serverUrlGetter';

export function writeAccessLog(page: string) {
    fetch(`${getServerUrl()}/api/logs/pageaccess?page=${page}`, {
            mode: 'cors',
            method: 'GET'
        })
        .catch(err => console.error(err));
}