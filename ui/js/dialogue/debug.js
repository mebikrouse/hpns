let dialogueDebug = document.querySelector('#dialogue-debug');

let startButton = dialogueDebug.querySelector("#start-button");
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

let stopButton = dialogueDebug.querySelector('#stop-button');
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

let printButton = dialogueDebug.querySelector('#print-button');
printButton.onclick = function () {
    rootResponder.handle({
        name: 'propagate',
        data: {
            target: 'dialogue',
            command: {
                name: 'print',
                data: {
                    title: 'Участник диалога',
                    content: 'Всеобъемлющий ответ участника текущего диалога, который дает актуальную информацию по текущей ситуации.'
                }
            }
        }
    });
}

let skipButton = dialogueDebug.querySelector('#skip-button');
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