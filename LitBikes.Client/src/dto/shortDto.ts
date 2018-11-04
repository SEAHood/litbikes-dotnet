import { Vector } from "../util";
import { BikeDto, TrailSegmentDto, PlayerDto, PowerUpDto, HelloDto, GameSettingsDto, WorldUpdateDto, ArenaDto,
    DebugDto, ImpactDto, ChatMessageDto, ClientChatMessageDto, ScoreDto, ClientUpdateDto, GameJoinDto, ClientGameJoinDto
} from "./dto";
import { PowerUpType } from "../model/powerUp";

export class BikeDtoShort {
    i: string;
    n: string;
    p: Vector;
    d: Vector;
    s: number;
    t: TrailSegmentDtoShort[];
    c: string; // includes %A% alpha

    constructor(fullDto?: BikeDto) {
        if (fullDto) {
            this.i = fullDto.playerId;
            this.n = fullDto.name;
            this.p = fullDto.pos;
            this.d = fullDto.dir;
            this.s = fullDto.spd;
            this.t = fullDto.trail.map(t => new TrailSegmentDtoShort(t));
            this.c = fullDto.colour;
        }
    }
}

export class PlayerDtoShort {
    i: string;
    n: string;
    b: BikeDtoShort;
    c: boolean;
    cr: boolean;
    ci: string;
    cin: string;
    dt?: number;
    s: boolean;
    sc: number;
    cpu: string;
    e: string;

    constructor(fullDto?: PlayerDto) {
        if (fullDto) {
            this.i = fullDto.playerId;
            this.n = fullDto.name;
            this.b = new BikeDtoShort(fullDto.bike);
            this.c = fullDto.crashed;
            this.cr = fullDto.crashing;
            this.ci = fullDto.crashedInto;
            this.cin = fullDto.crashedIntoName;
            this.dt = fullDto.deathTimestamp;
            this.s = fullDto.spectating;
            this.sc = fullDto.score;
            //this.cpu = fullDto.currentPowerUp;
            this.e = fullDto.effect;
        }
    }
}

export class PowerUpDtoShort {
    i: string;
    p: Vector;
    t: PowerUpType;
    c: boolean;

    constructor(fullDto?: PowerUpDto) {
        if (fullDto) {
            this.i = fullDto.id;
            this.p = fullDto.pos;
            this.t = fullDto.type;
            this.c = fullDto.collected;
        }
    }
}

export class HelloDtoShort {
    gs: GameSettingsDtoShort;
    w: WorldUpdateDtoShort;

    constructor(fullDto?: HelloDto) {
        if (fullDto) {
            this.gs = new GameSettingsDtoShort(fullDto.gameSettings);
            this.w = new WorldUpdateDtoShort(fullDto.world);
        }
    }
}

export class GameJoinDtoShort {
    p: PlayerDtoShort;
    s: ScoreDtoShort[];

    constructor(fullDto?: GameJoinDto) {
        if (fullDto) {
            this.p = new PlayerDtoShort(fullDto.player);
            this.s = fullDto.scores.map(s => new ScoreDtoShort(s));
        }
    }
}

export class ArenaDtoShort {
    s: number;

    constructor(fullDto?: ArenaDto) {
        if (fullDto) {
            this.s = fullDto.size;
        }
    }
}

export class TrailSegmentDtoShort {
    ih: boolean;
    s: Vector;
    e: Vector;

    constructor(fullDto?: TrailSegmentDto) {
        if (fullDto) {
            this.ih = fullDto.isHead;
            this.s = fullDto.start;
            this.e = fullDto.end;
        }
    }
}

export class WorldUpdateDtoShort {
    t: number;
    gt: number;
    rip: boolean;
    rtl: number;
    tunr: number;
    cw: string;
    p: PlayerDtoShort[];
    pu: PowerUpDtoShort[];
    a: ArenaDtoShort;
    d: DebugDtoShort;

    constructor(fullDto?: WorldUpdateDto) {
        if (fullDto) {
            this.t = fullDto.timestamp;
            this.gt = fullDto.gameTick;
            this.rip = fullDto.roundInProgress;
            this.rtl = fullDto.roundTimeLeft;
            this.tunr = fullDto.timeUntilNextRound;
            this.cw = fullDto.currentWinner;
            this.p = fullDto.players.map(p => new PlayerDtoShort(p));
            this.pu = fullDto.powerUps.map(pu => new PowerUpDtoShort(pu));
            this.a = new ArenaDtoShort(fullDto.arena);
            this.d = new DebugDtoShort(fullDto.debug);
        }
    }
}

export class ChatMessageDtoShort {
    m: string;
    t: number;
    s: string;
    sc: string; // includes %A% alpha
    ism: boolean;

    constructor(fullDto?: ChatMessageDto) {
        if (fullDto) {
            this.m = fullDto.message;
            this.t = fullDto.timestamp;
            this.s = fullDto.source;
            this.sc = fullDto.sourceColour;
            this.ism = fullDto.isSystemMessage;
        }
    }
}

export class ClientChatMessageDtoShort {
    m: string;

    constructor(fullDto?: ClientChatMessageDto) {
        if (fullDto) {
            this.m = fullDto.message;
        }
    }
}

export class ScoreDtoShort {
    i: string;
    n: string;
    s: number;

    constructor(fullDto?: ScoreDto) {
        if (fullDto) {
            this.i = fullDto.playerId;
            this.n = fullDto.name;
            this.s = fullDto.score;
        }
    }
}

export class GameSettingsDtoShort {
    gt: number;

    constructor(fullDto?: GameSettingsDto) {
        if (fullDto) {
            this.gt = fullDto.gameTickMs;
        }
    }
}

export class DebugDtoShort {
    i: ImpactDtoShort[];
    
    constructor(fullDto?: DebugDto) {
        if (fullDto) {
            this.i = fullDto.impacts.map(i => new ImpactDtoShort(i));
        }
    }
}

export class ImpactDtoShort {
    p: Vector;

    constructor(fullDto?: ImpactDto) {
        if (fullDto) {
            this.p = fullDto.pos;
        }
    }
}

export class ClientUpdateDtoShort {
    i: string;
    xd: number;
    yd: number;
    xp: number;
    yp: number;

    constructor(fullDto?: ClientUpdateDto) {
        if (fullDto) {
            this.i = fullDto.playerId;
            this.xd = fullDto.xDir;
            this.yd = fullDto.yDir;
            this.xp = fullDto.xPos;
            this.yp = fullDto.yPos;
        }
    }
}

export class ClientGameJoinDtoShort {
    n: string;

    constructor(fullDto?: ClientGameJoinDto) {
        if (fullDto) {
            this.n = fullDto.name;
        }
    }
}