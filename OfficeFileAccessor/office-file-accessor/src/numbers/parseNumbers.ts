export function tryParseInt(value: unknown): number|null {
    if(value == null) {
        return null;
    }
    let textValue = '';
    if(typeof value == 'string') {
        textValue = value;
    } else {
        textValue = value.toString();
    }
    const result = parseInt(textValue);
    if(result == null || isNaN(result)) {
        return null;
    }
    return result;
}