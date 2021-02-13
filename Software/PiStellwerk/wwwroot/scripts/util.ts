import * as User from "./user.js"

export function getRequestInit(method: string, body?: BodyInit): RequestInit {
    var req: RequestInit =  {
        method: method,
        headers: {
            "Content-Type": "application/json",
            "Session-Id": User.getSessionId()
        },
        body: body,
    };
    return req;
}