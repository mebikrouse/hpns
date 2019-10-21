function Controller(name) {
    Responder.call(this, name);

    this.active = false;

    this.registerHandler('start', (data) => this.start());
    this.registerHandler('stop', (data) => this.stop());
}

Controller.prototype = Object.create(Responder.prototype);
Controller.prototype.constructor = Responder;

Controller.prototype.start = function () {
    if (this.active) throw 'Cannot start controller because it is already started';
    this.active = true;

    this.didStart();
}

Controller.prototype.didStart = function () { }

Controller.prototype.stop = function () {
    if (!this.active) throw 'Cannot stop controller because it is already stopped';
    this.active = false;

    this.didStop();
}

Controller.prototype.didStop = function () { }