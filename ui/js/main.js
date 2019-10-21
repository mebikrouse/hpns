let dialogueView = document.querySelector('#dialogue');
let dialogueController = new DialogueController(dialogueView);

let rootResponder = new RootResponder();
rootResponder.addChild(dialogueController);

window.addEventListener('message', (e) => {
    rootResponder.handle(e.data);
});