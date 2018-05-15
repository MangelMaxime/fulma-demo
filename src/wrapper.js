export function importLibB () {
    return import("./LibB.fs").then(module => { return module.default});
}
