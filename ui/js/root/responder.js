function RootResponder() {
    Responder.call(this, 'root');

    this.resourceName;

    this.registerHandler('init', (data) => this.init(data.resourceName));
}

RootResponder.prototype = Object.create(Responder.prototype);
RootResponder.prototype.constructor = Responder;

RootResponder.prototype.reply = function (command) {
    if (!this.resourceName) throw 'Cannot reply from root because it is not initialized with target resource';
    post(`http://${this.resourceName}/message`, command);
}

RootResponder.prototype.init = function (resourceName) {
    if (this.resourceName) throw 'Cannot initialize root because it is already initialized';
    this.resourceName = resourceName;
}