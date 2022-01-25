function setup() {
    createCanvas(300, 300);
    noLoop();
    let c = color(0, 0, 0);
    fill(c);
}

var gridHeight = 10;
var gridWidth = 10;

var numIter = 100; //iterations of the model

function draw() {
    let nbCellsHeight = height / gridHeight;
    let nbCellsWidth = width / gridWidth;

    //draw a grid to ease debug
    for(let l = 0; l < nbCellsHeight; l++) {
        line(0, l * gridHeight, height, l * gridHeight)
        for(let c = 0; c < nbCellsWidth; c++) {
            line(c * gridWidth, 0, c * gridHeight, width)
        }
    }

    //arrays to store coordinates, potention growth sites and the current cluster
    let coords = Array.apply(null , Array(nbCellsHeight * nbCellsWidth)).map((x, i) =>  {
        return {
            height: Math.floor(i / nbCellsHeight) * gridHeight + (gridHeight / 2),
            width: i % nbCellsHeight * gridWidth + (gridWidth / 2),
            line: Math.floor(i / nbCellsHeight),
            column: i % nbCellsHeight,
        }
    })

    //builds an array of neighbors
    let sites = new Map();
    let cluster = new Map();

    // place a circle on an end of the grid
    // and update cluster
    circle(coords[0].width, coords[0].height, 8);
    cluster.set(coords[0].line + "-" + coords[0].column, {line: coords[0].line, column: coords[0].column});
    
    //add neighbors to the array
    sites = addNeighbors(sites, cluster, coords[0])

    //loops for a set number of times to spread the cluster
    for(var n = 0; n < numIter; n++) {
        // take a random neighbor
        let available = Array.from(sites.values());
        let randIndex = Math.floor(Math.random() * available.length);
        let selected = available[randIndex];
        //draw circle,  update cluster and neighbors
        circle(coords[selected.column * nbCellsHeight + selected.line].width, coords[selected.column * nbCellsHeight + selected.line].height, 8);
        cluster.set(selected.line + "-" + selected.column, {line: selected.line, column: selected.column});
        sites.delete(selected.line + "-" + selected.column);
        sites = addNeighbors(sites, cluster, selected)
    }

}   

// searches for empty neighbors of the selected cell. Adds them 
// to the map of possible growth sites 
function addNeighbors(sites, cluster, selected) {
    let x = selected.column;
    let y = selected.line;

    // build surroundings array
    let surroundings = [
        getNeighbor(x, y - 1), //up
        getNeighbor(x + 1, y), //left
        getNeighbor(x, y + 1), //bottom
        getNeighbor(x - 1, y), //right
        /*getNeighbor(x, y - 1), //upleft
        getNeighbor(x + 1, y + 1), //bottomleft
        getNeighbor(x - 1, y + 1), //bottomright
        getNeighbor(x - 1, y - 1) //upright*/
    ]

    surroundings.forEach(n => {
        if (n !== null) {
            if (cluster.get(n.line + "-" + n.column) === undefined) {
                //adds to the list of possible neighbors only if not already present
                sites.set(n.line + "-" + n.column, {line: n.line, column: n.column})
            }
        }
    });

    return sites;
}

// returns null if not a valid coordinate (out of our scene bounds)
// otherwise returns the coordinates in an object
function getNeighbor(x, y) {
    if (x < 0 || y < 0) return null;
    else if (x > gridWidth || y > gridHeight) return null;
    else return {line: y, column: x};
}