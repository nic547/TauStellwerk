import * as User from "./user.js";
export function getRequestInit(method, body) {
    var req = {
        method: method,
        headers: {
            "Content-Type": "application/json",
            "Session-Id": User.getSessionId()
        },
        body: body,
    };
    return req;
}
export function groupBy(list, getKey) {
    list.reduce((previous, currentItem) => {
        const group = getKey(currentItem);
        if (!previous[group])
            previous[group] = [];
        previous[group].push(currentItem);
        return previous;
    }, {});
    return list;
}
//# sourceMappingURL=util.js.map