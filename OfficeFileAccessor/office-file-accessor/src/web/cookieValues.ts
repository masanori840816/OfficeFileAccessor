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