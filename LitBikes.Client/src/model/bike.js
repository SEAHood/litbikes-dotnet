"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const util_1 = require("../util");
const trailSegment_1 = require("./trailSegment");
const _ = require("underscore");
require("p5");
class Bike {
    constructor(bikeDto) {
        this.idRingPulseCount = 0;
        this.idRingPulseMax = 2;
        this.idRingDuration = 1500; //ms
        this.setPos(new util_1.Vector(bikeDto.pos.x, bikeDto.pos.y));
        this.dir = new util_1.Vector(bikeDto.dir.x, bikeDto.dir.y);
        this.spd = bikeDto.spd;
        this.colour = bikeDto.colour;
        this.trail = [];
        _.each(bikeDto.trail, (seg) => {
            this.trail.push(trailSegment_1.TrailSegment.fromDto(seg));
        });
        this.trailOpacity = 1;
        this.idRingBlinkTime = -1;
        this.idRingBlinkOn = false;
        this.idRingSize = 0;
        this.respawning = true;
    }
    update(canMove) {
        if (canMove) {
            let xDiff = this.dir.x * this.spd; // * this.timeDilation.x;
            let yDiff = this.dir.y * this.spd; // * this.timeDilation.y;
            this.setPos(new util_1.Vector(this.pos.x + xDiff, this.pos.y + yDiff));
        }
        if (this.crashing) {
            this.trailOpacity = Math.max(this.trailOpacity - 0.02, 0);
            if (this.trailOpacity == 0) {
                this.crashing = false;
                this.trailOpacity = 1;
            }
        }
    }
    updateFromDto(dto) {
        this.pos = dto.pos;
        this.dir = dto.dir;
        this.spd = dto.spd;
        this.colour = dto.colour;
        this.trail = [];
        _.each(dto.trail, (seg) => {
            this.trail.push(trailSegment_1.TrailSegment.fromDto(seg));
        });
        if (this.respawning && Date.now() - this.idRingDuration > this.lastRespawn) {
            this.respawning = false;
        }
    }
    addTrailSegment() {
        let lastSeg = _.last(this.trail).end;
        let newSeg = new trailSegment_1.TrailSegment(new util_1.Vector(lastSeg.x, lastSeg.y), new util_1.Vector(this.pos.x, this.pos.y));
        this.trail.push(newSeg);
    }
    crash(timeOfCrash) {
        this.dir = new util_1.Vector(0, 0);
        this.crashing = true;
        this.addTrailSegment();
    }
    respawned(timeOfCrash) {
        this.respawning = true;
        this.crashing = false;
        this.trailOpacity = 1;
        this.lastRespawn = Date.now();
    }
    draw(p, showRespawnRing, isControlledPlayer) {
        // Respawning effect
        if (this.respawning && showRespawnRing) {
            let innerRingSize = Math.max(0, this.idRingSize - 10);
            p.fill('rgba(0,0,0,0)');
            p.stroke(255);
            p.strokeWeight(2);
            p.ellipse(this.pos.x, this.pos.y, this.idRingSize, this.idRingSize);
            p.stroke(this.colour.replace('%A%', '1'));
            p.strokeWeight(1);
            p.ellipse(this.pos.x, this.pos.y, innerRingSize, innerRingSize);
            this.idRingSize = this.idRingSize + 1.5;
            if (this.idRingSize > 50) {
                this.idRingSize = 0;
                this.idRingPulseCount++;
                if (this.idRingPulseCount >= this.idRingPulseMax) {
                    this.respawning = false;
                    this.idRingPulseCount = 0;
                }
            }
        }
        // Draw trail
        p.strokeWeight(2);
        p.stroke(this.colour.replace('%A%', this.trailOpacity.toString()));
        // Create trail segment between bike and last trail end
        let headEnd = _.find(this.trail, t => t.isHead).end;
        let newSeg = new trailSegment_1.TrailSegment(new util_1.Vector(headEnd.x, headEnd.y), new util_1.Vector(this.pos.x, this.pos.y));
        let trail = _.clone(this.trail);
        trail.push(newSeg);
        p.noFill();
        _.each(trail, (tp) => {
            // if (tp.isHead) {
            //     p.stroke('rgb(255,0,0)');
            // } else {
            //     p.stroke(this.colour.replace('%A%', this.trailOpacity.toString()));
            // }
            //p.ellipse(tp.start.x, tp.start.y, 3, 3);
            p.line(tp.start.x, tp.start.y, tp.end.x, tp.end.y);
        });
        // Draw bike
        let bikeColour = isControlledPlayer
            ? "rgb(255, 255, 255)"
            : this.colour.replace('%A%', '1');
        p.noStroke();
        p.fill(bikeColour);
        p.ellipse(this.pos.x, this.pos.y, 5, 5);
        // Draw crashing
        if (this.crashing) {
            // Explosion
            var randColour = util_1.NumberUtil.rand255();
            p.stroke('rgba(' + randColour + ', 0, 0, 0.80)');
            p.fill('rgba(' + randColour + ', ' + randColour + ' , 0, 0.75)');
            p.ellipse(this.pos.x, this.pos.y, 20, 20);
            var randSize = Math.floor(Math.random() * 35);
            randColour = util_1.NumberUtil.rand255();
            p.stroke('rgba(' + randColour + ', ' + randColour + ' , 0, 0.55)');
            p.fill('rgba(' + util_1.NumberUtil.rand255() + ', 0, 0, 0.65)');
            p.ellipse(this.pos.x, this.pos.y, randSize, randSize);
        }
    }
    getPos() {
        return this.pos;
    }
    setPos(pos) {
        this.pos = pos;
    }
    getDir() {
        return this.dir;
    }
    getSpd() {
        return this.spd;
    }
    setSpd(spd) {
        this.spd = spd;
    }
    getColour() {
        return this.colour;
    }
    isCrashing() {
        return this.crashing;
    }
    isRespawning() {
        return this.respawning;
    }
}
exports.Bike = Bike;
//# sourceMappingURL=bike.js.map