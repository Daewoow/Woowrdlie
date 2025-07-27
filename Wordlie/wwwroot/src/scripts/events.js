function initEvents(){
    document.getElementById("sendBtn").addEventListener("click", function() {
        const message = document.getElementById("message").value;
        if (!message) return;

        hub.hubConnection.invoke("Send", message, gameId, userName)
            .catch(err => console.error("Ошибка отправки:", err));

        document.getElementById("message").value = "";
    });
    
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
    
    document.getElementById("wordGuess").addEventListener("input", function(event) {
        game.updateCurrentRowContent(event.target.value);
    })

    document.getElementById("message").addEventListener("keypress", function(e) {
        if (e.key === "Enter") document.getElementById("sendBtn").click();
    });

    document.getElementById("wordGuess").addEventListener("keypress", function(e) {
        if (e.key === "Enter") document.getElementById("guessBtn").click();
    });
}