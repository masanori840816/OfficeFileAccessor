export function getDPI(): number {
    const div = document.createElement('div');
    div.style.width = '1in';
    document.body.appendChild(div);
    const dpi = div.offsetWidth;
    document.body.removeChild(div);
    return dpi;
}
export function convertCentimeterToInch(value: number): number {
    return value / 2.54;
}
export function convertCentimeterToPixel(value: number, dpi: number): number {
    // 1. convert to inch, 2. get pixels by DPI.
    return convertCentimeterToInch(value) * dpi;
}