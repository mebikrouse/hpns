function Responder(name) {
    this.name = name;
    this.handlers = [];
    this.children = [];
    this.parent = undefined;

    this.registerHandler('propagate', (data) => this.propagate(data.target, data.command));
}

Responder.prototype.handle = function (command) {
    for (let handler of this.handlers) {
        if (handler.name != command.name) continue;

        handler.handler(command.data);
        return;
    }

    throw `Handler is not found for command '${command.name}'`;
}

Responder.prototype.registerHandler = function (name, handler) {
    this.handlers.push({name: name, handler: handler});
}

Responder.prototype.propagate = function (target, command) {
    for (let child of this.children) {
        if (child.name != target) continue;

        child.handle(command);
        return;
    }

    throw `Target '${target}' is not found for propagation`;
}

Responder.prototype.addChild = function (child) {
    this.children.push(child);
    child.parent = this;
}

Responder.prototype.reply = function (command) {
    if (!this.parent) throw 'Cannot send reply because there is no appropriate parent';

    let name = this.name;
    this.parent.reply({
        name: 'propagate',
        data: {
            target: name,
            command: command
        }
    });
}