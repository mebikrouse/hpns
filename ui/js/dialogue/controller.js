function DialogueController(view) {
    Controller.call(this, 'dialogue');

    this.registerHandler('print', (data) => this.print(data.title, data.content));
    this.registerHandler('skip', (data) => this.skip());
    this.registerHandler('replytest', (data) => this.replytest());
}

DialogueController.prototype = Object.create(Controller.prototype);
DialogueController.prototype.constructor = Controller;

DialogueController.prototype.didStart = function () {
    console.log('didStart');
}

DialogueController.prototype.didStop = function () {
    console.log('didStop');
}

DialogueController.prototype.print = function (title, content) {
    console.log(`print: ${title}, ${content}`);
}

DialogueController.prototype.skip = function () {
    console.log('skip');
}

DialogueController.prototype.replytest = function () {
    this.reply({name: 'replytest'});
}