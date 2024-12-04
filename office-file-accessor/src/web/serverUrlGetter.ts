export function getServerUrl() {
    if(import.meta.env.MODE === "production") {
        return "/officefiles";
    }
    return "";
}