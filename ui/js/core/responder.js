function Responder(name) {
    this.name = name;
    this.handlers = [];
    this.children = [];
    this.parent = null;
    this.active = false;

    this.registerHandler('startChild', (data) => this.startChild(data.target));
    this.registerHandler('stopChild', (data) => this.stopChild(data.target));
    this.registerHandler('propagate', (data) => this.propagate(data.target, data.command));
}

Responder.prototype.start = function () {
    if (this.active) throw 'Cannot start responder because it is already started';
    this.active = true;

    this.didStart();
}

Responder.prototype.didStart = function () { }

Responder.prototype.stop = function () {
    if (!this.active) throw 'Cannot stop responder because it is already stopped';
    this.active = false;

    this.didStop();
}

Responder.prototype.didStop = function () { }

Responder.prototype.handle = function (command) {
    if (!this.active) throw 'Cannot handle command because of inactive state';

    let handler = this.findHandler(command.name);
    
    if (handler) handler.handler(command.data);
    else throw `Handler is not found for command '${command.name}'`;
}

Responder.prototype.propagate = function (target, command) {
    let child = this.findChild(target);

    if (child) child.handle(command);
    else throw `Target '${target}' is not found for propagation`;
}

Responder.prototype.startChild = function (target) {
    let child = this.findChild(target);

    if (child) child.start();
    else throw `Cannot find and start child '${target}'`;
}

Responder.prototype.stopChild = function (target) {
    let child = this.findChild(target);

    if (child) child.stop();
    else throw `Cannot find and stop child '${target}'`;
}

Responder.prototype.reply = function (command) {
    if (!this.active) throw 'Cannot send reply from inactive responder';
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

Responder.prototype.addChild = function (child) {
    this.children.push(child);
    child.parent = this;
}

Responder.prototype.findChild = function (name) {
    for (let child of this.children)
        if (child.name == name) return child;

    return null;
}

Responder.prototype.registerHandler = function (name, handler) {
    this.handlers.push({name: name, handler: handler});
}

Responder.prototype.findHandler = function (name) {
    for (let handler of this.handlers)
        if (handler.name == name) return handler;

    return null;
}