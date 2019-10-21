let startButton = document.querySelector("#start-button");
startButton.onclick = function () {
    rootResponder.handle({
        name: 'propagate',
        data: {
            target: 'dialogue',
            command: {
                name: 'start'
            }
        }
    });
}

let stopButton = document.querySelector('#stop-button');
stopButton.onclick = function () {
    rootResponder.handle({
        name: 'propagate',
        data: {
            target: 'dialogue',
            command: {
                name: 'stop'
            }
        }
    });
}

let printButton = document.querySelector('#print-button');
printButton.onclick = function () {
    rootResponder.handle({
        name: 'propagate',
        data: {
            target: 'dialogue',
            command: {
                name: 'print',
                data: {
                    title: 'kek',
                    content: 'lol'
                }
            }
        }
    });
}

let skipButton = document.querySelector('#skip-button');
skipButton.onclick = function () {
    rootResponder.handle({
        name: 'propagate',
        data: {
            target: 'dialogue',
            command: {
                name: 'skip'
            }
        }
    });
}

let replyButton = document.querySelector('#reply-button');
replyButton.onclick = function () {
    rootResponder.handle({
        name: 'propagate',
        data: {
            target: 'dialogue',
            command: {
                name: 'replytest'
            }
        }
    })
}