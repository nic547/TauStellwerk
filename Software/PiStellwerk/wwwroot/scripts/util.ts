export function getRequestInit(method: string, body?: BodyInit):RequestInit {
    var req: RequestInit =  {
        method: method,
        headers: {
            "Content-Type": "application/json",
        },
        body: body,
        credentials: "same-origin"
    };
    return req;
}