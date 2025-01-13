export function hasAnyTexts(value: any): value is string
{
    if(value == null) {
        return false;
    }
    if(typeof value === "string") {
        if(value.length > 0) {
            return true;
        }
    }
    const stringValue = value.toString();
    if(stringValue != null && stringValue.length > 0) {
        return true;
    }
    return false;
}