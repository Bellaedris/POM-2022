var gridHeight = 10;
var gridWidth = 10;

var nbCellsHeight;
var nbCellsWidth;

var coords, sites, cluster;

var noiseReduction = 3;

function setup() {
    createCanvas(900, 900);
    //noLoop();
    let c = color(0, 0, 0);
    fill(c);
    frameRate(30 * noiseReduction)

    init();
}

function init() {
    nbCellsHeight = height / gridHeight;
    nbCellsWidth = width / gridWidth;

    //draw a grid to ease debug
    for (let l = 0; l < nbCellsHeight; l++) {
        line(0, l * gridHeight, height, l * gridHeight)
        for (let c = 0; c < nbCellsWidth; c++) {
            line(c * gridWidth, 0, c * gridHeight, width)
        }
    }

    //arrays to store coordinates, potention growth sites and the current cluster
    coords = Array.apply(null, Array(nbCellsHeight * nbCellsWidth)).map((x, i) => {
        return {
            height: Math.floor(i / nbCellsHeight) * gridHeight + (gridHeight / 2),
            width: i % nbCellsHeight * gridWidth + (gridWidth / 2),
            line: Math.floor(i / nbCellsHeight),
            column: i % nbCellsHeight,
        }
    })

    //builds an array of neighbors
    sites = new Map();
    cluster = new Map();

    // the cluster fills the entire first line
    // and update cluster
    for (let i = 0; i < nbCellsWidth; i++) {
        circle(coords[i].width, coords[i].height, 8);
        cluster.set(coords[i].line + "-" + coords[i].column, { line: coords[i].line, column: coords[i].column });
        sites.delete(coords[i].line + "-" + coords[i].column);
        //add neighbors to the array
        sites = addNeighbors(sites, cluster, coords[i]);
    }
}

function draw() {
    //loops to spread the cluster until no candidate is found anymore
    if (sites.size !== 0) {
        // take a random neighbor
        let available = Array.from(sites.values());
        let randIndex = Math.floor(Math.random() * available.length);
        let selected = available[randIndex];

        let selectedCell = cluster.get(selected.line + "-" + selected.column);
        if (selectedCell === undefined) {
            selectedCell = {
                line: selected.line,
                column: selected.column,
                number: 1
            };
            cluster.set(selected.line + "-" + selected.column, selectedCell);
        } else {
            selectedCell.number += 1;
            cluster.set(selected.line + "-" + selected.column, selectedCell)
        }

        //draw circle,  update cluster and neighbors
        // if the growth site has enough points, draw it
        if (selectedCell.number >= noiseReduction) {
            circle(coords[selected.line * nbCellsHeight + selected.column].width, coords[selected.line * nbCellsHeight + selected.column].height, 8);
            sites.delete(selected.line + "-" + selected.column);
            sites = addNeighbors(sites, cluster, selected);
        }

        if (debug) {
            //show growth sites on debug mode
            c = color(255, 0, 0);
            fill(c);
            Array.from(sites.values()).forEach(site => {
                circle(coords[site.line * nbCellsHeight + site.column].width, coords[site.line * nbCellsHeight + site.column].height, 8);
            });
            c = color(0, 0, 0);
            fill(c);
        }
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
                sites.set(n.line + "-" + n.column, { line: n.line, column: n.column })
            }
        }
    });

    return sites;
}

// returns null if not a valid coordinate (out of our scene bounds)
// otherwise returns the coordinates in an object
function getNeighbor(x, y) {
    if (x < 0 || y < 0) return null;
    else if (x > nbCellsWidth - 1 || y > nbCellsHeight - 1) return null;
    else return { line: y, column: x };
}

//UI elements
var debug = false;

// UI functions
function resetDrawing() {
    //parses the different gen parameter and generates from scratch again
    noiseReduction = parseInt(document.getElementById("noiseReduc").value);
    debug = document.getElementById('debug').checked;
    clear();
    init();
}