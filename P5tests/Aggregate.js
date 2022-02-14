class Aggregate {

    aggregate = [];

    constructor() {
        this.boundA = createVector(Number.POSITIVE_INFINITY, Number.NEGATIVE_INFINITY); //up right corner
        this.boundB = createVector(Number.NEGATIVE_INFINITY, Number.POSITIVE_INFINITY); //bottom left corner
    }
    
    addCell(walker) {
        this.aggregate.push(walker);
        this.updateBounds(walker);
    }

    // update the bounding box
    updateBounds(walker) {
        if (walker.pos.x + cellSize < this.boundA.x)
            this.boundA.x = walker.pos.x + cellSize;
        if (walker.pos.x + cellSize > this.boundB.x)
            this.boundB.x = walker.pos.x + cellSize;
        if (walker.pos.y + cellSize > this.boundA.y)
            this.boundA.y = walker.pos.y + cellSize;
        if (walker.pos.y + cellSize < this.boundB.y)
            this.boundB.y = walker.pos.y + cellSize;
    }

    // return true if a point is inside the aggregate bounding box, false otherwise
    inside(p) {
        return (p.x > this.boundA.x) && (p.x < this.boundB.x) && (p.y < this.boundA.y) && (p.y > this.boundB.y);
    }

    get(i) {
        return this.aggregate[i];
    }

    paint() {
        this.aggregate.forEach(cell => {
            cell.paint()
        });
    }

    length() {
        return this.aggregate.length;
    }

}