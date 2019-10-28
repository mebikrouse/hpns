function Menu(options, prototype, container) {
    this.options = options;
    this.prototype = prototype;
    this.container = container;
    this.optionNodes = [];

    this.delay = 50;
}

Menu.prototype.show = function () {
    let stepHeight = 60;
    let stepsCount = Math.ceil(this.options.length / 2);
    let radius = stepsCount * stepHeight / 2;

    let currentHeight = stepHeight / 2;
    for (let i = 0; i < stepsCount; i++) {
        let positionY = currentHeight - radius;
        let positionX = Math.cos(Math.asin((radius - currentHeight) / radius)) * radius;
        currentHeight += stepHeight;

        let rightOptionNode = this.createOptionNode(this.options[i], positionX, positionY);
        this.showOptionNode(rightOptionNode, this.delay * i);
        this.optionNodes[i] = rightOptionNode;

        if (i + stepsCount >= this.options.length) return;
        let j = this.options.length - i - 1;

        let leftOptionNode = this.createOptionNode(this.options[j], -positionX, positionY, true);
        this.showOptionNode(leftOptionNode, this.delay * j);
        this.optionNodes[j] = leftOptionNode;
    }
}

Menu.prototype.hide = function () {
    for (let i = 0; i < this.optionNodes.length; i++) {
        let optionNode = this.optionNodes[i];
        this.hideOptionNode(optionNode, this.delay * i);
    }

    this.optionNodes = [];
}

Menu.prototype.createOptionNode = function (option, x, y, left) {
    let optionNode = this.prototype.cloneNode(true);
    optionNode.querySelector('#icon').textContent = option.icon;
    optionNode.querySelector('#text').textContent = option.text;

    this.container.appendChild(optionNode);

    optionNode.style.display = 'block';
    optionNode.style.marginTop = -optionNode.offsetHeight / 2;
    if (left) optionNode.style.marginLeft = -optionNode.offsetWidth;

    optionNode.style.transform = `translate(${x}px, ${y}px)`;

    return optionNode;
}

Menu.prototype.showOptionNode = function (optionNode, delay) {
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

Menu.prototype.hideOptionNode = function (optionNode, delay) {
    anime({
        targets: optionNode,
        opacity: [1, 0],
        scaleX: [1, 0.25],
        scaleY: [1, 0.25],
        duration: 250,
        delay: delay,
        easing: 'easeInOutCubic',
        complete: () => {
            optionNode.parentNode.removeChild(optionNode);
        }
    });
}