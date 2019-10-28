function InteractionController(view) {
    Responder.call(this, 'interaction');

    this.container = view;
    this.prototype = view.querySelector('#prototype');
    this.crosshair = view.querySelector('#crosshair');
    this.cancel = view.querySelector('#cancel');

    this.currentMenu = null;

    this.crosshairAnim;
    this.cancelAnim;

    this.registerHandler('showCrosshair', (data) => this.showCrosshair());
    this.registerHandler('hideCrosshair', (data) => this.hideCrosshair());
    this.registerHandler('showMenu', (data) => this.showMenu(data.options));
    this.registerHandler('hideMenu', (data) => this.hideMenu());
}

InteractionController.prototype = Object.create(Responder.prototype);
InteractionController.prototype.constructor = Responder;

InteractionController.prototype.didStart = function () { }

InteractionController.prototype.didStop = function () {
    this.hideCrosshair();
    this.hideMenu();
}

InteractionController.prototype.showCrosshair = function () {
    let crosshair = this.crosshair;
    crosshair.style.display = 'block';

    if (this.crosshairAnim) this.crosshairAnim.pause();
    this.crosshairAnim = anime({
        targets: crosshair,
        opacity: [0, 1],
        scaleX: [0.25, 1],
        scaleY: [0.25, 1],
        duration: 250,
        easing: 'easeInOutCubic'
    });

    this.crosshairAnim.play();
}

InteractionController.prototype.hideCrosshair = function () {
    let crosshair = this.crosshair;

    if (this.crosshairAnim) this.crosshairAnim.pause();
    this.crosshairAnim = anime({
        targets: crosshair,
        opacity: [1, 0],
        scaleX: [1, 0.25],
        scaleY: [1, 0.25],
        duration: 250,
        easing: 'easeInOutCubic',
        complete: () => {
            crosshair.style.display = 'none';
        }
    });

    this.crosshairAnim.play();
}

InteractionController.prototype.showMenu = function (options) {
    if (this.currentMenu) this.currentMenu.hide();

    this.currentMenu = new Menu(options, this.prototype, this.container);
    this.currentMenu.show();

    this.showCancel();
}

InteractionController.prototype.hideMenu = function () {
    if (this.currentMenu) this.currentMenu.hide();
    this.currentMenu = null;

    this.hideCancel();
}

InteractionController.prototype.showCancel = function () {
    let cancel = this.cancel;
    cancel.style.display = 'block';

    if (this.cancelAnim) this.cancelAnim.pause();
    this.cancelAnim = anime({
        targets: cancel,
        opacity: [0, 1],
        scaleX: [0.25, 1],
        scaleY: [0.25, 1],
        duration: 250,
        easing: 'easeInOutCubic'
    });

    this.cancelAnim.play();
}

InteractionController.prototype.hideCancel = function () {
    let cancel = this.cancel;

    if (this.cancelAnim) this.cancelAnim.pause();
    this.cancelAnim = anime({
        targets: cancel,
        opacity: [1, 0],
        scaleX: [1, 0.25],
        scaleY: [1, 0.25],
        duration: 250,
        easing: 'easeInOutCubic',
        complete: () => {
            cancel.style.display = 'none';
        }
    });

    this.cancelAnim.play();
}