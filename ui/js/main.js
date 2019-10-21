let dialogueView = document.querySelector('#dialogue');
let dialogueController = new DialogueController(dialogueView);

let rootResponder = new RootResponder();
rootResponder.addChild(dialogueController);

let inputHandler = new InputHandler();
inputHandler.keyPressed.attach((sender, key) => rootResponder.handleKeyPress(key));
inputHandler.keyReleased.attach((sender, key) => rootResponder.handleKeyRelease(key));

window.addEventListener('message', (e) => {
    rootResponder.handle(e.data);
});

window.addEventListener('keydown', (e) => {
    inputHandler.keyDown(e.keyCode);
});

window.addEventListener('keyup', (e) => {
    inputHandler.keyUp(e.keyCode);
})