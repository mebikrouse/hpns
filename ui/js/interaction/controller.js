function InteractionController(view) {
    Responder.call(this, 'interaction');

    this.container = view;
    this.crosshair = view.querySelector('#crosshair');
    this.cancel = view.querySelector('#cancel');
    this.prototype = view.querySelector('#prototype');

    this.options = [];

    this.registerHandler('showMenu', (data) => this.showMenu(data.options));
    this.registerHandler('hideMenu', (data) => this.hideMenu());
}

InteractionController.prototype = Object.create(Responder.prototype);
InteractionController.prototype.constructor = Responder;

InteractionController.prototype.didStart = function () {
    this.showCrosshair();
}

InteractionController.prototype.didStop = function () {
    this.hideMenu();
    this.hideCrosshair();
}

InteractionController.prototype.showMenu = function (options) {
    let delay = 25;
    let stepLength = 60;
    let stepsCount = Math.ceil(options.length / 2);
    let radius = stepLength * stepsCount / 2;
    let currentHeight = stepLength / 2;
    
    for (let i = 0; i < stepsCount; i++) {
        let translateY = currentHeight - radius;
        let translateX = Math.cos(Math.asin((radius - currentHeight) / radius)) * radius;

        let optionNodeRight = this.appendOption(options[i], translateX, translateY);
        this.options[i] = optionNodeRight;

        this.showOption(optionNodeRight, delay * i);

        if (i + stepsCount < options.length) {
            let j = options.length - 1 - i;

            let optionNodeLeft = this.appendOption(options[i], -translateX, translateY);
            this.options[j] = optionNodeLeft;

            optionNodeLeft.style.marginLeft = -optionNodeLeft.offsetWidth;
            this.showOption(optionNodeLeft, delay * j);
        }
        
        currentHeight += stepLength;
    }

    this.showCancel();
}

InteractionController.prototype.hideMenu = function () {
    let delay = 25;
    for (let i = 0; i < this.options.length; i++)
        this.hideOption(this.options[i], delay * i);

    this.options = [];

    this.hideCancel();
}

InteractionController.prototype.showCrosshair = function () {
    this.crosshair.style.display = 'block';
    
    let crosshair = this.crosshair;
    anime({
        targets: crosshair, 
        opacity: [0, 1],
        scaleX: [0.75, 1],
        scaleY: [0.75, 1],
        duration: 250,
        easing: 'easeInOutCubic'
    });
}

InteractionController.prototype.hideCrosshair = function () {
    let interaction = this;
    let crosshair = this.crosshair;
    anime({
        targets: crosshair, 
        opacity: [1, 0],
        scaleX: [1, 0.75],
        scaleY: [1, 0.75],
        duration: 250,
        easing: 'easeInOutCubic',
        complete: () => {
            if (interaction.active) return;
            crosshair.style.display = 'none';
        }
    });
}

InteractionController.prototype.appendOption = function (option, x, y) {
    let optionNode = this.prototype.cloneNode(true);

    optionNode.querySelector('#icon').textContent = option.icon;
    optionNode.querySelector('#text').textContent = option.text;

    this.container.appendChild(optionNode);

    optionNode.style.display = 'block';
    optionNode.style.marginTop = -optionNode.offsetHeight / 2;
    optionNode.style.transform = `translate(${x}px, ${y}px)`;

    return optionNode;
}

InteractionController.prototype.showOption = function (optionNode, delay) {
    anime({
        targets: optionNode,
        opacity: [0, 1],
        scaleX: [0.25, 1],
        scaleY: [0.25, 1],
        duration: 250,
        delay: delay,
        easing: 'easeInOutCubic'
    });
}

InteractionController.prototype.hideOption = function (optionNode, delay) {
    let container = this.container;
    anime({
        targets: optionNode,
        opacity: [1, 0],
        scaleX: [1, 0.25],
        scaleY: [1, 0.25],
        duration: 250,
        delay: delay,
        easing: 'easeInOutCubic',
        complete: () => {
            container.removeChild(optionNode);
        }
    });
}

InteractionController.prototype.showCancel = function () {
    let cancel = this.cancel;
    cancel.style.display = 'block';
    anime({
        targets: cancel,
        opacity: [0, 1],
        scaleX: [0.25, 1],
        scaleY: [0.25, 1],
        duration: 250,
        easing: 'easeInOutCubic'
    });
}

InteractionController.prototype.hideCancel = function () {
    let cancel = this.cancel;
    anime({
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
}