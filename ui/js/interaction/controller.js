function InteractionController(view) {
    Controller.call(this, 'interaction');

    this.container = view;
    this.crosshair = view.querySelector('#crosshair');
    this.prototype = view.querySelector('#prototype');

    this.options = [];

    this.registerHandler('showMenu', (data) => this.showMenu(data.options));
    this.registerHandler('hideMenu', (data) => this.hideMenu());
}

InteractionController.prototype = Object.create(Controller.prototype);
InteractionController.prototype.constructor = Controller;

InteractionController.prototype.didStart = function () {
    this.showCrosshair();
}

InteractionController.prototype.didStop = function () {
    this.hideMenu();
    this.hideCrosshair();
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

InteractionController.prototype.showMenu = function (options) {
    let delay = 25;
    let stepLength = 60;
    let stepsCount = Math.ceil(options.length / 2);
    let radius = stepLength * stepsCount / 2;
    let currentHeight = stepLength / 2;
    
    for (let i = 0; i < stepsCount; i++) {
        let optionNodeRight = this.prototype.cloneNode(true);
        this.container.appendChild(optionNodeRight);

        optionNodeRight.style.display = 'block';
        optionNodeRight.style.marginTop = -optionNodeRight.offsetHeight / 2;

        let translateY = currentHeight - radius;
        let translateX = Math.cos(Math.asin((radius - currentHeight) / radius)) * radius;
        optionNodeRight.style.transform = `translate(${translateX}px, ${translateY}px)`;

        anime({
            targets: optionNodeRight,
            opacity: [0, 1],
            scaleX: [0.25, 1],
            scaleY: [0.25, 1],
            duration: 250,
            delay: delay * i,
            easing: 'easeInOutCubic'
        });

        this.options[i] = optionNodeRight;

        currentHeight += stepLength;

        if (i + stepsCount >= options.length) return;
        let j = options.length - 1 - i;
        
        let optionNodeLeft = this.prototype.cloneNode(true);
        this.container.appendChild(optionNodeLeft);

        optionNodeLeft.style.display = 'block';
        optionNodeLeft.style.marginTop = -optionNodeLeft.offsetHeight / 2;
        optionNodeLeft.style.marginLeft = -optionNodeLeft.offsetWidth;

        optionNodeLeft.style.transform = `translate(${-translateX}px, ${translateY}px)`;

        anime({
            targets: optionNodeLeft,
            opacity: [0, 1],
            scaleX: [0.25, 1],
            scaleY: [0.25, 1],
            duration: 250,
            delay: delay * j,
            easing: 'easeInOutCubic'
        });

        this.options[j] = optionNodeLeft;
    }
}

InteractionController.prototype.hideMenu = function () {
    let delay = 25;
    let container = this.container;
    let currentDelay = 0;

    for (let optionNode of this.options) {
        anime({
            targets: optionNode,
            opacity: [1, 0],
            scaleX: [1, 0.25],
            scaleY: [1, 0.25],
            duration: 250,
            delay: currentDelay,
            easing: 'easeInOutCubic',
            complete: () => {
                container.removeChild(optionNode);
            }
        });

        currentDelay += delay;
    }

    this.options = [];
}