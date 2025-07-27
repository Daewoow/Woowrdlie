

// Получаем gameId из URL
const gameId = new URLSearchParams(window.location.search).get("gameId");
document.getElementById("gameId").textContent = gameId;

// Получаем имя пользователя из sessionStorage
const userName = sessionStorage.getItem("userName") || "Игрок";

const chatroom = document.getElementById("chatroom");

const game = new WoowrdlieGame();
// Инициализация SignalR соединения
const hub = new Hub(15000, 600000);
hub.init();
// game.createEmptyField(14);
// game.getHistory().then(() => game.updateField());

initEvents();
