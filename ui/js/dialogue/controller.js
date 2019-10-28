function DialogueController(view) {
    Responder.call(this, 'dialogue');

    this.prototype = view.querySelector('#prototype');
    this.fade = view.querySelector('#fade');
    this.audio = view.querySelector('#audio');

    this.registerHandler('print', (data) => this.print(data.title, data.content));
    this.registerHandler('skip', (data) => this.skip());

    this.currentDialogue = null;
}

DialogueController.prototype = Object.create(Responder.prototype);
DialogueController.prototype.constructor = Responder;

DialogueController.prototype.didStart = function () {
    let fade = this.fade;
    fade.style.display = 'block';
    anime({
        targets: fade,
        opacity: [0, 1],
        duration: 250,
        easing: 'easeInOutCubic'
    });
}

DialogueController.prototype.didStop = function () {
    if (this.currentDialogue) this.currentDialogue.hide();
    this.currentDialogue = null;

    let fade = this.fade;
    anime({
        targets: fade,
        opacity: [1, 0],
        duration: 250,
        easing: 'easeInOutCubic',
        complete: () => {
            if (this.active) return;
            fade.style.display = 'none';
        }
    });
}

DialogueController.prototype.print = function (title, content) {
    if (this.currentDialogue) this.currentDialogue.hide();

    this.currentDialogue = new Dialogue(title, content, this.prototype, this.fade, this.audio);
    this.currentDialogue.show();

    this.currentDialogue.printer.onprint.attach(() => {
        this.reply({name: 'printed'});
    });
}

DialogueController.prototype.skip = function () {
    if (this.currentDialogue) this.currentDialogue.skip();
}