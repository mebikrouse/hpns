function post(url, data) {
    var xhr = new XMLHttpRequest();
    xhr.open('POST', url, true);
    xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.send(JSON.stringify(data));
}

function slice(string, position) {
    return {start: string.substr(0, position + 1), end: string.substr(position + 1)};
}