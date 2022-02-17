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
        for (let i = 0; i < aggregate.length(); i++) {
            if (this.sqrDist(aggregate.get(i)) <= cellSize * cellSize) {
                if (Math.random() >= heightmap[Math.round(this.pos.x) * height + Math.round(this.pos.y)]) {
                    this.aggregated = true;
                    return true;
                }
            }
        }
        return false
    }

    // calculates the distance without the sqrt to accelerate the process
    sqrDist(b) {
        let dx = this.pos.x - b.pos.x;
        let dy = this.pos.y - b.pos.y;
        return dx * dx + dy * dy;
    }

}