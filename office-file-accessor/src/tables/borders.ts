import { DisplayOfficeFileCell } from "../officeFileAccessor.type";

export function checkIsBordered(cell: DisplayOfficeFileCell): boolean {
    if(cell.borderLeft != null &&
        cell.borderLeft > 0) {
        return true;
    }
    if(cell.borderTop != null &&
        cell.borderTop > 0) {
        return true;
    }
    if(cell.borderRight != null &&
        cell.borderRight > 0) {
        return true;
    }if(cell.borderBottom != null &&
        cell.borderBottom > 0) {
        return true;
    }
    return false;
}