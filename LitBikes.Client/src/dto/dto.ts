import { Vector } from "../util";
import { BikeDtoShort, TrailSegmentDtoShort, GameSettingsDtoShort, PlayerDtoShort, PowerUpDtoShort,
    ArenaDtoShort, ImpactDtoShort, DebugDtoShort, WorldUpdateDtoShort, ClientChatMessageDtoShort,
    ScoreDtoShort, ClientUpdateDtoShort, GameJoinDtoShort, HelloDtoShort, ClientGameJoinDtoShort,
    ChatMessageDtoShort
} from "./shortDto";
import { PowerUpType } from "../model/powerUp";

export class BikeDto {
    playerId: string;
    name: string;
    pos: Vector;
    dir: Vector;
    spd: number;
    trail: TrailSegmentDto[];
    colour: string; // includes %A% alpha

    constructor(shortDto?: BikeDtoShort) {
        if (shortDto) {
            this.playerId = shortDto.i;
            this.name = shortDto.n;
            this.pos = shortDto.p;
            this.dir = shortDto.d;
            this.spd = shortDto.s;
            this.trail = shortDto.t.map(t => new TrailSegmentDto(t));
            this.colour = shortDto.c;
        }
    }
}

export class PlayerDto {
    playerId: string;
    name: string;
    bike: BikeDto;
    crashed: boolean;
    crashing: boolean;
    crashedInto: string;
    crashedIntoName: string;
    deathTimestamp?: number;
    spectating: boolean;
    score: number;
    currentPowerUp: string;
    effect: string;

    constructor(shortDto?: PlayerDtoShort) {
        if (shortDto) {
            this.playerId = shortDto.i;
            this.name = shortDto.n;
            this.bike = new BikeDto(shortDto.b);
            this.crashed = shortDto.c;
            this.crashing = shortDto.cr;
            this.crashedInto = shortDto.ci;
            this.crashedIntoName = shortDto.cin;
            this.deathTimestamp = shortDto.dt;
            this.spectating = shortDto.s;
            this.score = shortDto.sc;
            this.currentPowerUp = shortDto.cpu;
            this.effect = shortDto.e;
        }
    }
}

export class PowerUpDto {
    id: string;
    pos: Vector;
    type: PowerUpType;
    collected: boolean;

    constructor(shortDto?: PowerUpDtoShort) {
        if (shortDto) {
            this.id = shortDto.i;
            this.pos = shortDto.p;
            this.type = shortDto.t;
            this.collected = shortDto.c;
        }
    }
}

export class HelloDto {
    gameSettings: GameSettingsDto;
    world: WorldUpdateDto;

    constructor(shortDto?: HelloDtoShort) {
        if (shortDto) {
            this.gameSettings = new GameSettingsDto(shortDto.gs);
            this.world = new WorldUpdateDto(shortDto.w);
        }
    }
}

export class GameJoinDto {
    player: PlayerDto;
    scores: ScoreDto[];

    constructor(shortDto?: GameJoinDtoShort) {
        if (shortDto) {
            this.player = new PlayerDto(shortDto.p);
            this.scores = shortDto.s.map(s => new ScoreDto(s));
        }
    }
}

export class ArenaDto {
    size: number;

    constructor(shortDto?: ArenaDtoShort) {
        if (shortDto) {
            this.size = shortDto.s;
        }
    }
}

export class TrailSegmentDto {
    isHead: boolean;
    start: Vector;
    end: Vector;

    constructor(shortDto?: TrailSegmentDtoShort) {
        if (shortDto) {
            this.isHead = shortDto.ih;
            this.start = shortDto.s;
            this.end = shortDto.e;
        }
    }
}

export class WorldUpdateDto {
    timestamp: number;
    gameTick: number;
    roundInProgress: boolean;
    roundTimeLeft: number;
    timeUntilNextRound: number;
    currentWinner: string;
    players: PlayerDto[];
    powerUps: PowerUpDto[];
    arena: ArenaDto;
    debug: DebugDto;

    constructor(shortDto?: WorldUpdateDtoShort) {
        if (shortDto) {
            this.timestamp = shortDto.t;
            this.gameTick = shortDto.gt;
            this.roundInProgress = shortDto.rip;
            this.roundTimeLeft = shortDto.rtl;
            this.timeUntilNextRound = shortDto.tunr;
            this.currentWinner = shortDto.cw;
            this.players = shortDto.p.map(p => new PlayerDto(p));
            this.powerUps = shortDto.pu.map(pu => new PowerUpDto(pu));
            this.arena = new ArenaDto(shortDto.a);
            this.debug = new DebugDto(shortDto.d);
        }
    }
}

export class ChatMessageDto {
    message: string;
    timestamp: number;
    source: string;
    sourceColour: string; // includes %A% alpha
    isSystemMessage: boolean;

    constructor(shortDto?: ChatMessageDtoShort) {
        if (shortDto) {
            this.message = shortDto.m;
            this.timestamp = shortDto.t;
            this.source = shortDto.s;
            this.sourceColour = shortDto.sc;
            this.isSystemMessage = shortDto.ism;
        }
    }
}

export class ClientChatMessageDto {
    message: string;

    constructor(shortDto?: ClientChatMessageDtoShort) {
        if (shortDto) {
            this.message = shortDto.m;
        }
    }
}

export class ScoreDto {
    playerId: string;
    name: string;
    score: number;

    constructor(shortDto?: ScoreDtoShort) {
        if (shortDto) {
            this.playerId = shortDto.i;
            this.name = shortDto.n;
            this.score = shortDto.s;
        }
    }
}

export class GameSettingsDto {
    gameTickMs: number;

    constructor(shortDto?: GameSettingsDtoShort) {
        if (shortDto) {
            this.gameTickMs = shortDto.gt;
        }
    }
}

export class DebugDto {
    impacts: ImpactDto[];

    constructor(shortDto?: DebugDtoShort) {
        if (shortDto) {
            this.impacts = shortDto.i.map(i => new ImpactDto(i));
        }
    }
}

export class ImpactDto {
    pos: Vector;

    constructor(shortDto?: ImpactDtoShort) {
        if (shortDto) {
            this.pos = shortDto.p;
        }
    }
}

export class ClientUpdateDto {
    playerId: string;
    xDir: number;
    yDir: number;
    xPos: number;
    yPos: number;

    constructor(shortDto?: ClientUpdateDtoShort) {
        if (shortDto) {
            this.playerId = shortDto.i;
            this.xDir = shortDto.xd;
            this.yDir = shortDto.yd;
            this.xPos = shortDto.xp;
            this.yPos = shortDto.yp;
        }
    }
}

export class ClientGameJoinDto {
    name: string;

    constructor(shortDto?: ClientGameJoinDtoShort) {
        if (shortDto) {
            this.name = shortDto.n;
        }
    }
}