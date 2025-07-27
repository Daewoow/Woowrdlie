class WoowrdlieGame{
    constructor() {
        this.wordLength = 6;
        this.history = []; 
        this.wordElements = [];
        this.boardElement = document.getElementById('board');
    }

    async getHistory() {
        try {
            const response = await fetch(`/game/${gameId}/attemptsJson`);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const result = await response.json();
            
            this.history = result["words"];
            this.wordLength = result["wordLength"];
        } catch (error) {
            console.error('Ошибка при получении истории:', error);
            throw error;
        }
    }
    
    async getLastWord(){
        try {
            const response = await fetch(`/game/${gameId}/attemptsJson`);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const result = await response.json();

            return result["words"].at(-1);
        } catch (error) {
            console.error('Ошибка при получении истории:', error);
            throw error;
        }
    }
    
    updateField(){
        this.clearField();
        this.createEmptyField(this.wordLength, Math.max(6, this.history.length + 1));
        
        for (let i = 0; i < this.history.length; i++) {
            this.fillRow(this.wordElements[i], this.history[i]);
        }
    }
    
    addNewWord(word){
        this.history.push(word);
        if (this.isFinalWord(word)){
            setTimeout(() => {
                window.location.href = 'https://www.youtube.com/watch?v=dQw4w9WgXcQ';
            }, 2000);
        }
        if (this.history.length === this.wordElements.length) {
            this.addEmptyRow(this.wordLength);
        }
        
        const lastRow = this.wordElements[this.history.length - 1];
        this.fillAndAnimateRow(lastRow, word);
    }
    
    fillRow(row, word){
        const tiles = [...row.getElementsByClassName("tile")];
        tiles.forEach((tile, index) => this.fillTile(tile, word[index]));
    }
    
    fillTile(tile, letter){
        tile.className = "tile";
        if (letter["isKnown"]){
            if (letter["isMoved"]){
                tile.classList.add("yellow");
            }
            else{
                tile.classList.add("green");
            }
        }
        else {
            tile.classList.add("wrong");
        }
        tile.textContent = letter["value"].toUpperCase();
    }
    
    createEmptyField(wordLength, numberOfRows = 6){
        for (let i = 0; i < numberOfRows; i++){
            this.addEmptyRow(wordLength)
        }
    }
    
    addEmptyRow(wordLength){
        const row = this.createEmptyRow(wordLength);
        this.wordElements.push(row);
        this.boardElement.appendChild(row);
    }
    
    clearField(){
        while (this.boardElement.firstChild) {
            this.boardElement.removeChild(this.boardElement.firstChild);
        }
        
        this.wordElements = [];
    }

    createEmptyRow(length){
        const tile = Object.assign(document.createElement('div'), {
            className: 'tile',
        });
        const row = Object.assign(document.createElement('div'), {
            className: 'row',
        });
        row.style.gridTemplateColumns = `repeat(${length}, 1fr)`;
        for (let i = 0; i < length; i++)
            row.appendChild(tile.cloneNode(true));
        return row;
    }
    
    fillAndAnimateRow(row, word) {
        let evaluation = [];
        for (let letter of word){
            if (letter["isKnown"]) {
                if (letter["isMoved"])
                    evaluation.push('yellow');
                else
                    evaluation.push('green');
            }
            else
                evaluation.push('wrong');
        }
        const tiles = row.querySelectorAll('.tile');

        tiles.forEach((tile, index) => {
            tile.textContent = word[index]['value'].toUpperCase();
            tile.className = 'tile';
            setTimeout(() => {
                tile.classList.add('flip');
                setTimeout(() => {
                    console.log('lalala');
                    tile.classList.add(evaluation[index]);
                }, 250);
            }, index * 200);
        });
    }
    
    updateCurrentRowContent(wordString){
        wordString = wordString.trim().padEnd(this.wordLength);
        if (/[^a-zA-Zа-яА-ЯёЁ\s]/.test(wordString)){
            return;
        }
        
        const currentRow = this.wordElements[this.history.length];
        currentRow.querySelectorAll('.tile')
            .forEach((tile, index) => {
                tile.textContent = wordString[index].toUpperCase();
        })
    }
    
    isFinalWord(word){
        return word.every(letter => letter["isKnown"] && !letter["isMoved"]);
    }
}