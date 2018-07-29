import { Vector } from "../util"
import "p5"
import { TrailSegmentDto } from "../dto/dto";

export class TrailSegment {

    isHead: boolean;
    start: Vector;
    end: Vector;

    constructor( start: Vector, end: Vector ) {
        this.start = start;
        this.end = end;
    }

    static fromDto( dto: TrailSegmentDto ) : TrailSegment {
        const segment = new TrailSegment( dto.start, dto.end );
        segment.isHead = dto.isHead;
        return segment;
    }

    draw( p : p5 ) {

    }
}