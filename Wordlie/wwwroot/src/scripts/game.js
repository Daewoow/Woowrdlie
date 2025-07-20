// Получаем gameId из URL
const gameId = new URLSearchParams(window.location.search).get("gameId");
document.getElementById("gameId").textContent = gameId;

// Получаем имя пользователя из sessionStorage
const userName = sessionStorage.getItem("userName") || "Игрок";

const chatroom = document.getElementById("chatroom");

// Функция для добавления сообщения в чат
function addMessageToChat(message, sender) {
    const userNameElem = document.createElement("b");
    userNameElem.textContent = `${sender}: `;

    const elem = document.createElement("p");
    elem.classList.add("message");
    elem.appendChild(userNameElem);
    elem.appendChild(document.createTextNode(message));

    chatroom.insertBefore(elem, chatroom.lastChild);
}

// Инициализация SignalR соединения
const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/game", {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
        keepAliveIntervalInMilliseconds: 300000,
        serverTimeoutInMilliseconds: 600000
    })
    .withAutomaticReconnect([0, 2000, 5000, 10000])
    .build();

// Обработчик обычных сообщений
document.getElementById("sendBtn").addEventListener("click", function() {
    const message = document.getElementById("message").value;
    if (!message) return;

    hubConnection.invoke("Send", message, gameId, userName)
        .catch(err => console.error("Ошибка отправки:", err));

    document.getElementById("message").value = "";
});

// Обработчик попытки угадать слово
document.getElementById("guessBtn").addEventListener("click", async function() {
    const word = document.getElementById("wordGuess").value.trim();
    if (!word) return;

    // const response = fetch(`words/all`)
    // console.log(response)

    // Добавляем сообщение о попытке
    addMessageToChat(`Было написано слово "${word}"`, userName);

    try {
        // Отправляем слово на сервер
        const response = await fetch(`/game/${gameId}/try/${encodeURIComponent(word)}`, {
            method: "POST"
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(errorText || "Ошибка сервера");
        }

        // Получаем результат проверки
        const result = await response.text();

        // Добавляем результат в чат
        result.split('\n').forEach(line => {
            if (line.trim()) addMessageToChat(line, "Система");
            console.log(line);
        });
        console.log(result.split('\n'));

        // Очищаем поле ввода
        document.getElementById("wordGuess").value = "";

    } catch (error) {
        console.error("Ошибка:", error);
        addMessageToChat(error.message, "Система");
    }
});

// Обработчики SignalR сообщений
hubConnection.on("Receive", function(message, sender) {
    addMessageToChat(message, sender);
});

hubConnection.on("BadWord", function(message) {
    addMessageToChat(message, "Система");
});

hubConnection.on("ContinueGame", function(message) {
    message.split('\n').forEach(line => {
        if (line.trim()) addMessageToChat(line, "Система");
    });
    fillBoard(message.split("\n").reverse().slice(1));
});

hubConnection.on("PlayerJoined", function(name) {
    const playerElement = document.createElement("div");
    playerElement.textContent = name;
    document.getElementById("playersList").appendChild(playerElement);
    addMessageToChat(`${name}`, "Система");
});

hubConnection.on("Notify", function(message) {
    addMessageToChat(message, "Система");
});

// Запуск соединения
hubConnection.start()
    .then(() => {
        addMessageToChat("Подключено к игре", "Система");
        return hubConnection.invoke("JoinGame", gameId, userName);
    })
    .then(() => {
        return fetch(`/game/${gameId}/attempts`)
            .then(response => {
                if (!response.ok) throw new Error("Ошибка получения попыток");
                return response.text();
            })
            .then(attemptsText => {
                attemptsText.split('\n').forEach(attempt => {
                    if (attempt.trim()) {
                        addMessageToChat(attempt, "История попыток");
                    }
                });
            });
    })
    .catch(err => {
        console.error("Ошибка подключения:", err);
        addMessageToChat("Ошибка подключения к игре", "Система");
    });

hubConnection.on("ReceiveAttempts", (attemptsText) => {
    const chatroom = document.getElementById("chatroom");
    const oldAttempts = chatroom.querySelectorAll(".attempt-message");
    oldAttempts.forEach(el => el.remove());
    try {
        fillBoard(attemptsText.split("\n").slice(1));
    }
    catch(err) {
        console.error(err);
    }
    
    attemptsText.split('\n').forEach(attempt => {
        if (attempt.trim()) {
            const elem = document.createElement("p");
            elem.className = "attempt-message";
            elem.innerHTML = `<i>История:</i> ${attempt}`;
            chatroom.appendChild(elem);
        }
    });
});

// Обработка закрытия соединения
hubConnection.onclose(async () => {
    addMessageToChat("Соединение прервано. Пытаемся переподключиться...", "Система");
    setTimeout(() => hubConnection.start(), 5000);
});

// Отправка по нажатию Enter
document.getElementById("message").addEventListener("keypress", function(e) {
    if (e.key === "Enter") document.getElementById("sendBtn").click();
});

document.getElementById("wordGuess").addEventListener("keypress", function(e) {
    if (e.key === "Enter") document.getElementById("guessBtn").click();
});