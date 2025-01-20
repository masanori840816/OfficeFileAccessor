import { ApplicationResult } from "../officeFileAccessor.type"

export type AuthenticationType = {
    signedIn: boolean,
    signIn: (email: string, password: string) => Promise<ApplicationResult>,
    signOut: () => Promise<boolean>,
    check: () => Promise<boolean>
}