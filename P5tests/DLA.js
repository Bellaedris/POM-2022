//TODO perlin map to simulate corals not developing as much at high depth/pressures
//TODO corals shouldn't develop as much as the depth increases (due to to lack of sunlight). They also require fishes, clean salty water 

function setup() {
    createCanvas(600, 600);
    strokeWeight(cellSize);

    frameRate(30);

    init();
}

// data structures
var aggregate;
var walkers = []
var heightmap; // 
var lastDelta = 0;

// simulation parameters
var cellSize = 10;
var nbWalkers = 400;
var iterPerFrame = 50;
var stopThreshold = 10000; // stop iterating when n numbers are agregated
var stickyness = 1;
var noiseScale = 0.1;

// UI
var mode = "default"

function init() {
    aggregate = new Aggregate();
    initHeightmap();
    switch(mode) {
        case "default": 
            initDefault();
            break;
        case "line": 
            initLine();
            break;
    }
}

function initDefault() {
    // place vector at the center of the screen
    aggregate.addCell(new Walker(width / 2, height / 2, true));
    spawnWalkers();
}

function initLine() {
    for(let i = 0; i < width / cellSize; i++) {
        aggregate.addCell(new Walker(i * cellSize, height, true))
    }
    spawnWalkers();
}

function spawnWalkers() {
    // re add walkers until there's enough
    /*for (let i = walkers.length; i < nbWalkers; i++) {
        walkers.push(new Walker(random(width), random(height)));
    }*/
    while(walkers.length < nbWalkers) {
        let coord = createVector(random(width), random(height));
        if (!aggregate.inside(coord))
            walkers.push(new Walker(coord.x, coord.y));
    }
}

function initHeightmap() {
    noiseDetail(4, 0.2)
    heightmap = Array(width * height);

    for(let i = 0; i < width; i++) {
        for(let j = 0; j < height; j++) {
            heightmap[j * height + i] = noise(j * noiseScale, i * noiseScale); //simplex.noise2D(j, i);
        }
    }
}

/*function showHeightmap() {
    for (let i = 0; i < height; i++) {
        for(let j = 0; j < width; j++) {
            stroke(heightmap[j * height + i] * 255);
            point(j * cellSize, i * cellSize);
        }
    }
}*/

function draw() {
    // stop simulation when the aggregate reaches a certain size.
    // includes a safety to avoid crashing your browser
    if (aggregate.length() >= stopThreshold || lastDelta >= 3000) {
        return;
    } else {
        lastDelta = deltaTime;
    }

    //displays the cells
    background(0);

    aggregate.paint();

    walkers.forEach(cell => {
        cell.paint()
    });

    // displaces all the walkers a set number of times
    // also removes the walkers that got aggregated
    for (var c = 0; c < iterPerFrame; c++) {
        for (var i = 0; i < walkers.length; i++) {
            walkers[i].randomWalk();
            if (walkers[i].checkAggregated(aggregate)) {
                aggregate.addCell(walkers[i]);
                walkers.splice(i, 1);
            }
        }
    }

    spawnWalkers()
}

function resetDrawing() {
    nbWalkers = parseInt(document.getElementById("nbwalkers").value);
    iterPerFrame = parseInt(document.getElementById("iterPerFrame").value);
    stopThreshold = parseInt(document.getElementById("stopthreshold").value);
    stickyness = parseFloat(document.getElementById("stickyness").value);
    mode = document.getElementById("mode").value;

    aggregate = new Aggregate();
    walkers = [];
    init();
}