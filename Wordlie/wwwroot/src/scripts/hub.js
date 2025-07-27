class Hub{
    constructor(keepAlive, serverTimeout){
        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl("/game", {
                skipNegotiation: true,
                transport: signalR.HttpTransportType.WebSockets,
                keepAliveIntervalInMilliseconds: keepAlive,
                serverTimeoutInMilliseconds: serverTimeout
            })
            .withAutomaticReconnect([0, 2000, 5000, 10000])
            .build();
    }
    
    init(){
        this.hubConnection.on("Receive", function(message, sender) {
            addMessageToChat(message, sender);
        });

        this.hubConnection.on("BadWord", function(message) {
            addMessageToChat(message, "Система");
        });

        this.hubConnection.on("ContinueGame", async function(message) {
            try {
                const lines = message.split('\n');
                addMultipleMessagesToChat(lines, "Система");

                // Получаем последнее слово и добавляем его в игру
                const lastWord = await game.getLastWord();
                await game.addNewWord(lastWord);
            } catch (error) {
                console.error('Ошибка в обработке ContinueGame:', error);
                // Можно добавить обработку ошибки (например, показать сообщение пользователю)
            }
        });

        this.hubConnection.on("PlayerJoined", function(name) {
            const playerElement = document.createElement("div");
            playerElement.textContent = name;
            document.getElementById("playersList").appendChild(playerElement);
            addMessageToChat(`${name}`, "Система");
        });

        this.hubConnection.on("Notify", function(message) {
            addMessageToChat(message, "Система");
        });

        this.hubConnection.on("ReceiveAttempts", (attemptsText) => {
            const chatroom = document.getElementById("chatroom");
            const oldAttempts = chatroom.querySelectorAll(".attempt-message");
            oldAttempts.forEach(el => el.remove());

            const attempts = attemptsText.split("\n");

            attempts.forEach(attempt => {
                if (attempt.trim()) {
                    const elem = document.createElement("p");
                    elem.className = "attempt-message";
                    elem.innerHTML = `<i>История:</i> ${attempt}`;
                    chatroom.appendChild(elem);
                }
            });
            
            // try {
            //     fillBoard(attempts.slice(1), attempts[0].length);
            // }
            // catch(err) {
            //     console.error(err);
            // }
        });

        this.hubConnection.start()
            .then(() => {
                addMessageToChat("Подключено к игре", "Система");
                return this.hubConnection.invoke("JoinGame", gameId, userName);
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
            .then(() => {
                return game.getHistory();
            })
            .then(() => {
                game.updateField();
            })
            .catch(err => {
                console.error("Ошибка подключения:", err);
                addMessageToChat("Ошибка подключения к игре", "Система");
            });
        
        this.hubConnection.onclose(async () => {
            addMessageToChat("Соединение прервано. Пытаемся переподключиться...", "Система");
            setTimeout(() => this.hubConnection.start(), 5000);
        }); 
    }
}