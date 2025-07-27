function addMessageToChat(message, sender) {
    const userNameElem = document.createElement("b");
    userNameElem.textContent = `${sender}: `;

    const elem = document.createElement("p");
    elem.classList.add("message");
    elem.appendChild(userNameElem);
    elem.appendChild(document.createTextNode(message));

    chatroom.insertBefore(elem, chatroom.lastChild);
}

function addMultipleMessagesToChat(messages, sender){
    for (const message of messages) {
        addMessageToChat(message, sender);
    }
}

