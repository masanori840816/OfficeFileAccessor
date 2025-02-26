import { CellBorders } from "../officeFileAccessor.type";

export function checkIsBordered(borders: CellBorders): boolean {
    if(borders.left > 0) {
        return true;
    }
    if(borders.top > 0) {
        return true;
    }
    if(borders.right > 0) {
        return true;
    }if(borders.bottom > 0) {
        return true;
    }
    return false;
}