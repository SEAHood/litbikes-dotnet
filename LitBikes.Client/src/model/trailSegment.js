"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
require("p5");
class TrailSegment {
    constructor(start, end) {
        this.start = start;
        this.end = end;
    }
    static fromDto(dto) {
        let segment = new TrailSegment(dto.start, dto.end);
        segment.isHead = dto.isHead;
        return segment;
    }
    draw(p) {
    }
}
exports.TrailSegment = TrailSegment;
//# sourceMappingURL=trailSegment.js.map