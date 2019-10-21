function Event(sender) {
    this.sender = sender;
    this.handlers = [];
}

Event.prototype.attach = function (handler) {
    this.handlers.push(handler);
}

Event.prototype.notify = function (args) {
    for (let handler of this.handlers) handler(this.sender, args);
}