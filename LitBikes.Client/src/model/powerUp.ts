import { Vector } from "../util"
import { PowerUpDto } from "../dto/dto";

export enum PowerUpType {
    Nothing,
    Rocket,
    Slow
}

export class PowerUp {
    private id: string;
    private pos: Vector;
    private type: PowerUpType;
    private collected: boolean;
    private collecting: boolean;
    private popSize: number;
        
    constructor(_id: string, _pos: Vector, _type: PowerUpType) {
        this.id = _id;
        this.pos = _pos;
        this.type = _type;
        this.collected = false;
        this.collecting = false;
        this.popSize = 0;
    }
    
    public getId(): string {
        return this.id;
    }
            
    public getPos(): Vector {
        return this.pos;
    }
    
    public setPos(pos: Vector) {
        this.pos = pos;
    }
    
    public getType() {
        return this.type;
    }
    
    public setType(type: PowerUpType) {
        this.type = type;
    }

    public isCollected(): boolean {
        return this.collected;
    }

    public collect() {
        this.collected = true;
        this.collecting = true;
    }

    public updateFromDto(dto: PowerUpDto) {
        this.pos = dto.pos;            
        if (!this.collected && dto.collected) {
            this.collect();
        }
    }

    public draw(p: p5) {
        if (!this.collected) {
            p.push();
            p.noStroke();
            p.translate(this.pos.x, this.pos.y);
            let size = 3;
            switch (this.type) {
                case PowerUpType.Rocket:
                    console.log(`Drawing rocket at ${this.pos.x}, ${this.pos.y}`);
                    p.rotate(p.frameCount / -10.0);
                    p.fill("rgb(255,255,105)");
                    p.triangle(
                        -size, size * 0.8, 
                        size, size * 0.8, 
                        0, -size
                    );
                    break;
                case PowerUpType.Slow:
                    console.log(`Drawing slow at ${this.pos.x}, ${this.pos.y}`);
                    p.rotate(p.frameCount / 50.0);
                    p.fill("rgb(105,255,255)");
                    p.triangle(
                        -size, size, 
                        size, size, 
                        0, -size
                    );
                    p.rotate(p.PI);
                    p.triangle(
                        -size, size, 
                        size, size, 
                        0, -size
                    );
                    break;
                default:
                    break;
            }
            p.pop();
        } else if (this.collecting) {
            p.fill("rgba(0,0,0,0)");
            p.stroke(255);
            p.strokeWeight(2);
            p.ellipse(this.pos.x, this.pos.y, this.popSize, this.popSize);
            this.popSize = this.popSize + 1.5;
            if (this.popSize > 20) {
                this.collecting = false;
            }
        }
    }        
}