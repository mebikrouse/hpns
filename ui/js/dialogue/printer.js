function Printer(content, container, audio) {
    this.content = content;
    this.container = container;
    this.audio = audio;

    this.printing = false;

    this.onprint = new Event(this);
}

Printer.prototype.prepare = function () {
    this.container.innerHTML = `<span style='opacity: 0;'>${this.content}</span>`;
}

Printer.prototype.start = function () {
    if (this.printing) return;
    this.printing = true;

    let content = this.content;
    let container = this.container;
    let audio = this.audio;
    let printer = this;
    let position = 0;
    function printing() {
        if (!printer.printing) return;

        let {start, end} = slice(content, position);
        container.innerHTML = start + ((end.length > 0) ? `<span style='opacity: 0;'>${end}</span>` : '');

        audio.currentTime = 0;
        audio.play();

        let printedChar = content[position];
        let delay = getDelayMultiplier(printedChar) * 35;

        position++;

        if (position >= content.length) {
            printer.printing = false;
            printer.onprint.notify();
        }

        setTimeout(printing, delay);
    }

    printing();
}

Printer.prototype.stop = function () {
    this.printing = false;
}

Printer.prototype.skip = function () {
    if (!this.printing) return;

    this.stop();
    this.container.textContent = this.content;

    this.onprint.notify();
}

function getDelayMultiplier(char) {
    switch (char) {
        case '.':
        case '!':
        case '?':
            return 10;
        case ',':
            return 5;
        default:
            return 1;
    }
}