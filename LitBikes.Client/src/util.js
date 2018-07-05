"use strict";
//import { WorldUpdateDto } from './dto';
Object.defineProperty(exports, "__esModule", { value: true });
class Vector {
    constructor(x, y) {
        this.x = x;
        this.y = y;
    }
    static distance(v1, v2) {
        var a = v1.x - v2.x;
        var b = v1.y - v2.y;
        return Math.sqrt(a * a + b * b);
    }
}
exports.Vector = Vector;
class NumberUtil {
    static randInt(min, max) {
        return Math.floor(Math.random() * (max - min + 1)) + min;
    }
    static rand255() {
        return Math.floor(Math.random() * 256);
    }
    static sameVector(v1, v2, error = 0) {
        let xDiff = v1.x - v2.x;
        let yDiff = v1.y - v2.y;
        return (xDiff >= error && xDiff <= error) &&
            (yDiff >= error && yDiff <= error);
    }
    static pad(n, width, z) {
        z = z || '0';
        let ns = n + '';
        return ns.length >= width ? ns : new Array(width - ns.length + 1).join(z) + ns;
    }
}
exports.NumberUtil = NumberUtil;
class ColourUtil {
    static rgba(r, g, b, a) {
        return 'rgba(' + r + ',' + g + ',' + b + ',' + a + ')';
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
            case 0:
                r = v, g = t, b = p;
                break;
            case 1:
                r = q, g = v, b = p;
                break;
            case 2:
                r = p, g = v, b = t;
                break;
            case 3:
                r = p, g = q, b = v;
                break;
            case 4:
                r = t, g = p, b = v;
                break;
            case 5:
                r = v, g = p, b = q;
                break;
        }
        return {
            r: Math.round(r * 255),
            g: Math.round(g * 255),
            b: Math.round(b * 255)
        };
    }
}
exports.ColourUtil = ColourUtil;
// WTF IS THIS
//export class Connection {
//    public socket: any;
//    public pid: number;
//    constructor(socket: any, pid: number) {
//        this.socket = socket;
//        this.pid = pid;
//    }
//    public fireWorldUpdated(worldUpdate: WorldUpdateDto) {
//        this.socket.emit('world-update', worldUpdate);
//    }
//}
//# sourceMappingURL=util.js.map