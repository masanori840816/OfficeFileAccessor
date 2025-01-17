import { ApplicationResult } from "../officeFileAccessor.type"

export type AuthenticationType = {
    signin: (email: string, password: string) => Promise<ApplicationResult>,
    signout: () => Promise<ApplicationResult>,
    check: () => Promise<boolean>
}