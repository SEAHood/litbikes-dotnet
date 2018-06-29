"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
class Arena {
    constructor(dto) {
        this.spacing = 10;
        this.size = dto.size;
    }
    draw(p) {
        p.background(51);
        // THE OLD BLUEISH GRID
        // p.strokeWeight(1);
        // p.stroke('rgba(125,249,255,0.10)');
        // for (var i = 0; i < this.size; i += this.spacing ) {
        //     p.line(i, 0, i, this.size);
        // }
        // for (var i = 0; i < this.size; i += this.spacing ) {
        //     p.line(0, i, this.size, i);
        // }            
    }
}
exports.Arena = Arena;
//# sourceMappingURL=arena.js.map