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


export function groupBy<T, TK extends keyof any>(list: T[], getKey: (item: T) => TK) {
    list.reduce((previous, currentItem) => {
            const group = getKey(currentItem);
            if (!previous[group]) previous[group] = [];
            previous[group].push(currentItem);
            return previous;
        },
        {} as Record<TK, T[]>);
    return list;
}
