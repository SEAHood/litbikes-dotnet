import { Vector } from "../util";
import { IDto, BikeDto, TrailSegmentDto, PlayerDto, PowerUpDto, HelloDto, GameSettingsDto, WorldUpdateDto, ArenaDto,
    DebugDto, ImpactDto, ChatMessageDto, ClientChatMessageDto, ScoreDto, ClientUpdateDto, GameJoinDto, ClientGameJoinDto
} from "./dto";

export interface IDtoShort {
    toFullDto(): IDto;
}

export class BikeDtoShort implements IDtoShort {
    i: string;
    n: string;
    p: Vector;
    d: Vector;
    s: number;
    t: TrailSegmentDtoShort[];
    c: string; // includes %A% alpha

    toFullDto(): IDto {
        const dto = new BikeDto();
        dto.playerId = this.i;
        dto.name = this.n;
        dto.pos = this.p;
        dto.dir = this.d;
        dto.spd = this.s;
        dto.trail = this.t.map(t => t.toFullDto() as TrailSegmentDto);
        dto.colour = this.c;
        return dto;
    }
}

export class PlayerDtoShort implements IDtoShort {
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

    toFullDto(): IDto {
        const dto = new PlayerDto();
        dto.playerId = this.i;
        dto.name = this.n;
        dto.bike = this.b.toFullDto() as BikeDto;
        dto.crashed = this.c;
        dto.crashing = this.cr;
        dto.crashedInto = this.ci;
        dto.crashedIntoName = this.cin;
        dto.deathTimestamp = this.dt;
        dto.spectating = this.s;
        dto.score = this.sc;
        dto.currentPowerUp = this.cpu;
        dto.effect = this.e;
        return dto;
    }
}

export class PowerUpDtoShort implements IDtoShort {
    i: string;
    p: Vector;
    t: string;
    c: boolean;

    toFullDto(): IDto {
        const dto = new PowerUpDto();
        dto.id = this.i;
        dto.pos = this.p;
        dto.type = this.t;
        dto.collected = this.c;
        return dto;
    }
}

export class HelloDtoShort implements IDtoShort {
    gs: GameSettingsDtoShort;
    w: WorldUpdateDtoShort;

    toFullDto(): IDto {
        const dto = new HelloDto();
        dto.gameSettings = this.gs.toFullDto() as GameSettingsDto;
        dto.world = this.w.toFullDto() as WorldUpdateDto;
        return dto;
    }
}

export class GameJoinDtoShort implements IDtoShort {
    p: PlayerDtoShort;
    s: ScoreDtoShort[];

    toFullDto(): IDto {
        const dto = new GameJoinDto();
        dto.player = this.p.toFullDto() as PlayerDto;
        dto.scores = this.s.map(s => s.toFullDto() as ScoreDto);
        return dto;
    }
}

export class ArenaDtoShort implements IDtoShort {
    s: number;

    toFullDto(): IDto {
        const dto = new ArenaDto();
        dto.size = this.s;
        return dto;
    }
}

export class TrailSegmentDtoShort implements IDtoShort {
    ih: boolean;
    s: Vector;
    e: Vector;

    toFullDto(): IDto {
        const dto = new TrailSegmentDto();
        dto.isHead = this.ih;
        dto.start = this.s;
        dto.end = this.e;
        return dto;
    }
}

export class WorldUpdateDtoShort implements IDtoShort {
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

    toFullDto(): IDto {
        const dto = new WorldUpdateDto();
        dto.timestamp = this.t;
        dto.gameTick = this.gt;
        dto.roundInProgress = this.rip;
        dto.roundTimeLeft = this.rtl;
        dto.timeUntilNextRound = this.tunr;
        dto.currentWinner = this.cw;
        dto.players = this.p.map(p => p.toFullDto() as PlayerDto);
        dto.powerUps = this.pu.map(pu => pu.toFullDto() as PowerUpDto);
        dto.arena = this.a.toFullDto() as ArenaDto;
        dto.debug = this.d.toFullDto() as DebugDto;
        return dto;
    }
}

export class ChatMessageDtoShort implements IDtoShort {
    m: string;
    t: number;
    s: string;
    sc: string; // includes %A% alpha
    ism: boolean;

    toFullDto(): IDto {
        const dto = new ChatMessageDto();
        dto.message = this.m;
        dto.timestamp = this.t;
        dto.source = this.s;
        dto.sourceColour = this.sc;
        dto.isSystemMessage = this.ism;
        return dto;
    }
}

export class ClientChatMessageDtoShort implements IDtoShort {
    m: string;

    toFullDto(): IDto {
        const dto = new ClientChatMessageDto();
        dto.message = this.m;
        return dto;
    }
}

export class ScoreDtoShort implements IDtoShort {
    i: string;
    n: string;
    s: number;

    toFullDto(): IDto {
        const dto = new ScoreDto();
        dto.playerId = this.i;
        dto.name = this.n;
        dto.score = this.s;
        return dto;
    }
}

export class GameSettingsDtoShort implements IDtoShort {
    gt: number;

    toFullDto(): IDto {
        const dto = new GameSettingsDto();
        dto.gameTickMs = this.gt;
        return dto;
    }
}

export class DebugDtoShort implements IDtoShort {
    i: ImpactDtoShort[];

    toFullDto(): IDto {
        const dto = new DebugDto();
        dto.impacts = this.i.map(i => i.toFullDto() as ImpactDto);
        return dto;
    }
}

export class ImpactDtoShort implements IDtoShort {
    p: Vector;

    toFullDto(): IDto {
        const dto = new ImpactDto();
        dto.pos = this.p;
        return dto;
    }
}

export class ClientUpdateDtoShort implements IDtoShort {
    i: string;
    xd: number;
    yd: number;
    xp: number;
    yp: number;

    toFullDto(): IDto {
        const dto = new ClientUpdateDto();
        dto.playerId = this.i;
        dto.xDir = this.xd;
        dto.yDir = this.yd;
        dto.xPos = this.xp;
        dto.yPos = this.yp;
        return dto;
    }
}

export class ClientGameJoinDtoShort implements IDtoShort {
    n: string;

    toFullDto(): IDto {
        const dto = new ClientGameJoinDto();
        dto.name = this.n;
        return dto;
    }
}