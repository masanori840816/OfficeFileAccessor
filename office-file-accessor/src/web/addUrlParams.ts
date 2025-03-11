import { hasAnyTexts } from "../texts/hasAnyTexts";

export function addUrlParams(params: string, key: string, value: string): string {
    if(hasAnyTexts(params)) {
        params += '&';
    } else {
        params = '?';
    }
    params += `${key}=${value}`;
    return params;
}