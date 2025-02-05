import { ApplicationResult } from "../officeFileAccessor.type"

export type AuthenticationType = {
    signedIn: SignedInUser|null,
    signIn: (email: string, password: string) => Promise<ApplicationResult>,
    signOut: () => Promise<boolean>,
    check: () => Promise<boolean>
}
export type SignInResult = {
    result: ApplicationResult,
    user: SignedInUser|null,
};
export type SignedInUser = {
    id: number,
    userName: string,
    organization: string|null,
    email: string, 
};