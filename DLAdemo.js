function setup() {
    createCanvas(500, 500);
    background(0, 0, 0);
    //noLoop();
    stroke(255)
    strokeWeight(cellSize);

    frameRate(30);

    init()
}

var cellSize = 10;
var aggregate = []
var walkers = []

var nbWalkers = 300;

function init() {
    // place vector at the center of the screen
    let initialCell = new Walker(width / 2, height / 2, true);
    aggregate.push(initialCell);
    initialCell.paint();

    for (let i = 0; i < nbWalkers; i++) {
        walkers.push(new Walker(random(width), random(height)));
    }
}

function draw() {

    // spawn random walker and make it loop a lot until it touches
    // an aggregated cell
    //check if the cell is in contact with another
    walkers.forEach(walker => {
        walker.checkAggregated(aggregate);
        if (!walker.aggregated) {
            walker.randomWalk();
        }
        walker.paint();
    });
}

class Walker {
    constructor(posX, posY, aggregated) {
        this.pos = createVector(posX, posY);
        this.aggregated = aggregated || false;
    }

    // randomly moves the walker
    randomWalk() {
        stroke(0);
        point(this.pos);
        this.pos.add(p5.Vector.random2D());
        constrain(this.pos.x, 0, width);
        constrain(this.pos.y, 0, height);
    }

    // display the walker
    paint() {
        if (this.aggregated) {
            stroke(255, 0, 0);
            point(this.pos);
        } else {
            stroke(255);
            point(this.pos);
        }
    }

    checkAggregated(aggregate) {
        for (let i = 0; i < aggregate.length; i++) {
            if (p5.Vector.dist(this.pos, aggregate[i].pos) <= cellSize) {
                console.log("intersected");
                this.aggregated = true;
                aggregate.push(this);
                let index = walkers.indexOf(this);
                walkers.splice(index, 1);
                break;
            }
        };
    }

}