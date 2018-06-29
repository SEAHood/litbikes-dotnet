import { Vector } from '../util'
import { ArenaDto, TrailSegmentDto } from '../dto'
import 'p5'

export class TrailSegment {

    public isHead: boolean;
    public start: Vector;
    public end: Vector;

    constructor( start: Vector, end: Vector ) {
        this.start = start;
        this.end = end;
    }

    public static fromDto( dto: TrailSegmentDto ) : TrailSegment {
        let segment = new TrailSegment( dto.start, dto.end );
        segment.isHead = dto.isHead;
        return segment;
    }

    public draw( p : p5 ) {

    }

}