import { createContext, useContext } from "react";
import { AuthenticationType } from "./authenticationType";

export const AuthenticationContext = createContext<AuthenticationType|null>(null);
export const useAuthentication = (): AuthenticationType|null => useContext(AuthenticationContext);
