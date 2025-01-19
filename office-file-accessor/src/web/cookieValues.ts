export function getCookieValue(name: string): string|null { 
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`); 
    if (parts.length === 2) {
        const result = parts.pop()?.split(';')?.shift();;
        if(result != null) {
            return result;
        }
    }
    return null; 
};
export function setCookieValue(key: string, value: string) {
    const date = new Date();
    date.setTime(date.getTime() + 1 * 24 * 60 * 60 * 1000);
    document.cookie = `${key}=${value}; expires=${date.toUTCString()}; path=/`;
}