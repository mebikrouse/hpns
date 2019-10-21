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
                    title: 'Участник диалога',
                    content: 'Всеобъемлющий ответ участника текущего диалога, который дает актуальную информацию по текущей ситуации.'
                }
            }
        }
    });
}