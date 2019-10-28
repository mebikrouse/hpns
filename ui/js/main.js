let dialogueView = document.querySelector('#dialogue');
let dialogueController = new DialogueController(dialogueView);

let interactionView = document.querySelector('#interaction');
let interactionController = new InteractionController(interactionView);

let rootResponder = new RootResponder();
rootResponder.addChild(dialogueController);
rootResponder.addChild(interactionController);

rootResponder.start();

window.addEventListener('message', (e) => {
    console.log(JSON.stringify(e));
    rootResponder.handle(e.data);
});