let interactionDebug = document.querySelector('#interaction-debug');

let startButton = interactionDebug.querySelector("#start-button");
startButton.onclick = function () {
    rootResponder.handle({
        name: 'startChild',
        data: {
            target: 'interaction'
        }
    });
}

let stopButton = interactionDebug.querySelector('#stop-button');
stopButton.onclick = function () {
    rootResponder.handle({
        name: 'stopChild',
        data: {
            target: 'interaction'
        }
    });
}

let showMenuButton = interactionDebug.querySelector('#show-menu-button');
showMenuButton.onclick = function () {
    rootResponder.handle({
        name: 'propagate',
        data: {
            target: 'interaction',
            command: {
                name: 'showMenu',
                data: {
                    options: [
                        {icon: '👏', text: 'Похлопать'},
                        {icon: '🤏', text: 'Поговорить'},
                        {icon: '🛒', text: 'Торговаться'},
                        {icon: '🐔', text: 'Ты Петух!'},
                        {icon: '❗', text: 'Отправка груза'}
                    ]
                }
            }
        }
    });
}

let hideMenuButton = interactionDebug.querySelector('#hide-menu-button');
hideMenuButton.onclick = function () {
    rootResponder.handle({
        name: 'propagate',
        data: {
            target: 'interaction',
            command: {
                name: 'hideMenu'
            }
        }
    });
}

let showCrossButton = interactionDebug.querySelector('#show-cross-button');
showCrossButton.onclick = function () {
    rootResponder.handle({
        name: 'propagate',
        data: {
            target: 'interaction',
            command: {
                name: 'showCrosshair'
            }
        }
    });
}

let hideCrossButton = interactionDebug.querySelector('#hide-cross-button');
hideCrossButton.onclick = function () {
    rootResponder.handle({
        name: 'propagate',
        data: {
            target: 'interaction',
            command: {
                name: 'hideCrosshair'
            }
        }
    });
}