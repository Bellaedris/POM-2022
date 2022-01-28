function setup() {
    createCanvas(600, 600);
    strokeWeight(cellSize);

    frameRate(30);

    init()
}

// data structures
var aggregate = []
var walkers = []
var lastDelta = 0;

// simulation parameters
var cellSize = 10;
var nbWalkers = 400;
var iterPerFrame = 50;
var stopThreshold = 300; // stop iterating when n numbers are agregated

function init() {
    // place vector at the center of the screen
    let initialCell = new Walker(width / 2, height / 2, true);
    aggregate.push(initialCell);

    for (let i = 0; i < nbWalkers; i++) {
        walkers.push(new Walker(random(width), random(height)));
    }
}

function draw() {
    // stop simulation when the aggregate reaches a certain size.
    // includes a safety to avoid crashing your browser
    if (aggregate.length >= stopThreshold || lastDelta >= 3000) {
        return;
    } else {
        lastDelta = deltaTime;
    }

    //displays the cells
    background(0);

    aggregate.forEach(cell => {
        cell.paint()
    });

    walkers.forEach(cell => {
        cell.paint()
    });

    // displaces all the walkers a set number of times
    // also removes the walkers that got aggregated
    for (var c = 0; c < iterPerFrame; c++) {
        for (var i = 0; i < walkers.length; i++) {
            walkers[i].randomWalk();
            if (walkers[i].checkAggregated(aggregate)) {
                aggregate.push(walkers[i]);
                walkers.splice(i, 1);
            }
        }
    }

    // re add walkers until there's enough
    for (let i = walkers.length; i < nbWalkers; i++) {
        walkers.push(new Walker(random(width), random(height)));
    }
}

class Walker {
    constructor(posX, posY, aggregated) {
        this.pos = createVector(posX, posY);
        this.aggregated = aggregated || false;
    }

    // randomly moves the walker
    randomWalk() {
        this.pos.add(p5.Vector.random2D());
        constrain(this.pos.x, 0, width);
        constrain(this.pos.y, 0, height);
    }

    // display the walker
    paint() {
        if (this.aggregated) {
            stroke(255, 0, 0);
        } else {
            stroke(255);
        }
        point(this.pos);
    }

    checkAggregated(aggregate) {
        for (let i = 0; i < aggregate.length; i++) {
            if (p5.Vector.dist(this.pos, aggregate[i].pos) <= cellSize) {
                this.aggregated = true;
                return true;
            }
        }
        return false
    }

}

function resetDrawing() {
    nbWalkers = parseInt(document.getElementById("nbwalkers").value);
    iterPerFrame = parseInt(document.getElementById("iterPerFrame").value);
    stopThreshold = parseInt(document.getElementById("stopthreshold").value);

    aggregate = []
    walkers = [];
    init();
}