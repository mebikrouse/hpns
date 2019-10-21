function Dialogue(title, content, prototype, container, audio) {
    this.view = prototype.cloneNode(true);
    this.container = container;

    let titleNode = this.view.querySelector('#title');
    titleNode.textContent = title;

    let contentNode = this.view.querySelector('#content');
    contentNode.textContent = content;

    this.printer = new Printer(content, contentNode, audio);
}

Dialogue.prototype.show = function () {
    this.container.appendChild(this.view);
    this.view.style.display = 'block';
    this.view.style.marginLeft = -this.view.offsetWidth / 2;

    this.printer.prepare();

    let view = this.view;
    let printer = this.printer;
    anime({
        targets: view,
        opacity: [0, 1],
        translateY: [32, 0],
        duration: 250,
        easing: 'easeInOutCubic',
        complete: () => {
            printer.start();
        }
    });
}

Dialogue.prototype.hide = function () {
    this.printer.stop();

    let container = this.container;
    let view = this.view;
    anime({
        targets: view,
        opacity: [1, 0],
        translateY: [0, -32],
        duration: 250,
        easing: 'easeInOutCubic',
        complete: () => {
            container.removeChild(view);
        }
    });
}

Dialogue.prototype.skip = function () {
    this.printer.skip();
}