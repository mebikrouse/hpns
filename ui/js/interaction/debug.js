let interactionDebug = document.querySelector('#interaction-debug');

let startButton = interactionDebug.querySelector("#start-button");
startButton.onclick = function () {
    rootResponder.handle({
        name: 'propagate',
        data: {
            target: 'interaction',
            command: {
                name: 'start'
            }
        }
    });
}

let stopButton = interactionDebug.querySelector('#stop-button');
stopButton.onclick = function () {
    rootResponder.handle({
        name: 'propagate',
        data: {
            target: 'interaction',
            command: {
                name: 'stop'
            }
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
                    options: ['1', '2', '3', '4', '5', '6', '7', '8', '9']
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