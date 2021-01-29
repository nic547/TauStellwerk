import * as User from "./user.js";

export function getRequestInit(method: string, body?: BodyInit):RequestInit {
    var req: RequestInit =  {
        method: method,
        headers: {
            "Content-Type": "application/json",
            "username": User.getUsername(),
        },
        body: body,
    };
    return req;
}