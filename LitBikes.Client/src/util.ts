﻿//import { WorldUpdateDto } from './dto';

export class Vector {
    public x: number;
    public y: number;
    constructor(x: number, y: number) {
        this.x = x;
        this.y = y;
    }

    public static distance(v1: Vector, v2: Vector): number {
        var a = v1.x - v2.x;
        var b = v1.y - v2.y;
        return Math.sqrt(a * a + b * b);
    }

    /*constructor(obj: any) {
        if ( !!obj.x && !!obj.y ) {
            this.x = obj.x;
            this.y = obj.y;
        }
    }

    public add(x: number, y: number) {
        this.x += x;
        this.y += y;
    }*/

}

export class NumberUtil {
    public static randInt(min, max): number {
        return Math.floor(Math.random() * (max - min + 1)) + min;
    }

    public static rand255(): number {
        return Math.floor(Math.random() * 256);
    }

    public static sameVector(v1: Vector, v2: Vector, error: number = 0) {
        let xDiff = v1.x - v2.x;
        let yDiff = v1.y - v2.y;
        return (xDiff >= error && xDiff <= error) &&
            (yDiff >= error && yDiff <= error);
    }

    public static pad(n: number, width: number, z?: string) {
        z = z || "0";
        let ns = n + "";
        return ns.length >= width ? ns : new Array(width - ns.length + 1).join(z) + ns;
    }
}

export class ColourUtil {
    public static rgba(r, g, b, a): string {
        return "rgba(" + r + "," + g + "," + b + "," + a + ")";
    }

    /* accepts parameters
        * h  Object = {h:x, s:y, v:z}
        * OR
        * h, s, v
        */
    HSVtoRGB(h, s, v) {
        var r, g, b, i, f, p, q, t;
        if (arguments.length === 1) {
            s = h.s, v = h.v, h = h.h;
        }
        i = Math.floor(h * 6);
        f = h * 6 - i;
        p = v * (1 - s);
        q = v * (1 - f * s);
        t = v * (1 - (1 - f) * s);
        switch (i % 6) {
            case 0: r = v, g = t, b = p; break;
            case 1: r = q, g = v, b = p; break;
            case 2: r = p, g = v, b = t; break;
            case 3: r = p, g = q, b = v; break;
            case 4: r = t, g = p, b = v; break;
            case 5: r = v, g = p, b = q; break;
        }
        return {
            r: Math.round(r * 255),
            g: Math.round(g * 255),
            b: Math.round(b * 255)
        };
    }
}

export class RenderUtil {
    static defaultColour = "125,248,255";
    static red = "255,32,32";
    static orange = "255,153,50";
    static yellow = "255,255,84";

    static renderText(p: p5, text: string, fontSize: number, x: number, y: number, effect: TextEffect) {
        let yOffset = fontSize / 10;
        effect = effect || TextEffect.None;
        p.textAlign("center", "center");

        if (effect != TextEffect.None) {
            let accentColour = this.defaultColour;
            if (effect == TextEffect.Fire) {
                let rand = NumberUtil.randInt(0, 2);
                switch (rand) {
                    case 0:
                        accentColour = this.red;
                        break;
                    case 1:
                        accentColour = this.orange;
                        break;
                    case 2:
                    default:
                        accentColour = this.yellow;
                        break;
                }
            }
            p.fill(`rgba(${accentColour},0.50)`);
            p.textSize(fontSize + 1);
            p.text(text, x + NumberUtil.randInt(0, fontSize / 10), y + NumberUtil.randInt(0, fontSize / 10) - yOffset);
        }        
        p.fill("rgba(255,255,255,0.80)");
        p.textSize(fontSize);
        p.text(text, x, y - yOffset);
    }
}

export enum TextEffect {
    None,
    Default,
    Fire
}


export class NetworkUtil {
    static sizeOfObject(obj: any) {

        var objectList = [];
        var stack = [obj];
        var bytes = 0;

        while (stack.length) {
            var value = stack.pop();

            if (typeof value === 'boolean') {
                bytes += 4;
            }
            else if (typeof value === 'string') {
                bytes += value.length * 2;
            }
            else if (typeof value === 'number') {
                bytes += 8;
            }
            else if
            (
                typeof value === 'object'
                    && objectList.indexOf(value) === -1
            ) {
                objectList.push(value);

                for (var i in value) {
                    stack.push(value[i]);
                }
            }
        }
        return bytes;
    }
}

// WTF IS THIS
//export class Connection {
//    public socket: any;
//    public playerId: number;

//    constructor(socket: any, playerId: number) {
//        this.socket = socket;
//        this.playerId = playerId;
//    }

//    public fireWorldUpdated(worldUpdate: WorldUpdateDto) {
//        this.socket.emit('world-update', worldUpdate);
//    }
//}