function createTile(letter){
    if (letter === '*')
        letter = '';
    const tile = Object.assign(document.createElement('div'), {
        className: 'tile',
        textContent: letter.toUpperCase(),
    });
    
    let state;
    if (letter === '')
        state = 'wrong';
    else if (letter === letter.toLowerCase())
        state = 'green';
    else if (letter === letter.toUpperCase())
        state = 'yellow';
    tile.classList.add(state);
    return tile;
}

function createRow(word){
    const row = Object.assign(document.createElement('div'), {
        className: 'row',
    });
    for (let letter of word){
        row.appendChild(createTile(letter));
    }
    row.style.gridTemplateColumns = `repeat(${word.length}, 1fr)`;
    return row;
}

function createEmptyRow(length){
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

function fillBoard(attempts){
    const board = document.getElementById('board');
    while (board.firstChild) {
        board.removeChild(board.firstChild);
    }
    for (const attempt of attempts.slice(0, -1)){
        board.appendChild(createRow(attempt));
    }
    
    let row = createEmptyRow(attempts[0].length);
    board.appendChild(row);
    animateRow(row, attempts.at(-1));
    for (let i = 0; i < 6 - attempts.length; i++){
        board.appendChild(createEmptyRow(attempts[0].length));
    }
}

function animateRow(row, word) {
    let evaluation = [];
    for (let letter of word){
        if (letter === '*')
            evaluation.push('wrong');
        else if (letter === letter.toLowerCase())
            evaluation.push('green');
        else if (letter === letter.toUpperCase())
            evaluation.push('yellow');
    }
    const tiles = row.querySelectorAll('.tile');
    
    tiles.forEach((tile, index) => {
        tile.textContent = word[index] === '*' ? '' : word[index].toUpperCase();
        setTimeout(() => {
            tile.classList.add('flip');
            setTimeout(() => {
                console.log('lalala');
                tile.classList.add(evaluation[index]);
            }, 250);
        }, index * 200);
    });
}

