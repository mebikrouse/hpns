function InputHandler() {
    this.pressedKeys = new Set();

    this.keyPressed = new Event(this);
    this.keyReleased = new Event(this);
}

InputHandler.prototype.keyDown = function (key) {
    if (this.pressedKeys.has(key)) return;
    this.pressedKeys.add(key);

    this.keyPressed.notify(key);
}

InputHandler.prototype.keyUp = function (key) {
    if (!this.pressedKeys.has(key)) return;
    this.pressedKeys.delete(key);

    this.keyReleased.notify(key);
}