import { Vector } from "../util";
import { IDtoShort, BikeDtoShort, TrailSegmentDtoShort, GameSettingsDtoShort, PlayerDtoShort, PowerUpDtoShort,
    ArenaDtoShort, ImpactDtoShort, DebugDtoShort, WorldUpdateDtoShort, ClientChatMessageDtoShort as SendChatMessageDtoShort,
    ScoreDtoShort, ClientUpdateDtoShort, GameJoinDtoShort, HelloDtoShort, ClientGameJoinDtoShort,
    ChatMessageDtoShort
} from "./shortDto";

export interface IDto {
    toShortDto(): IDtoShort;
}

export class BikeDto implements IDto {
    playerId: string;
    name: string;
    pos: Vector;
    dir: Vector;
    spd: number;
    trail: TrailSegmentDto[];
    colour: string; // includes %A% alpha

    toShortDto(): IDtoShort {
        const shortDto = new BikeDtoShort();
        shortDto.i = this.playerId;
        shortDto.n = this.name;
        shortDto.p = this.pos;
        shortDto.d = this.dir;
        shortDto.s = this.spd;
        shortDto.t = this.trail.map(t => t.toShortDto() as TrailSegmentDtoShort);
        shortDto.c = this.colour;
        return shortDto;
    }
}

export class PlayerDto implements IDto {
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

    toShortDto(): IDtoShort {
        const shortDto = new PlayerDtoShort();
        shortDto.i = this.playerId;
        shortDto.n = this.name;
        shortDto.b = this.bike.toShortDto() as BikeDtoShort;
        shortDto.c = this.crashed;
        shortDto.cr = this.crashing;
        shortDto.ci = this.crashedInto;
        shortDto.cin = this.crashedIntoName;
        shortDto.dt = this.deathTimestamp;
        shortDto.s = this.spectating;
        shortDto.sc = this.score;
        shortDto.cpu = this.currentPowerUp;
        shortDto.e = this.effect;
        return shortDto;
    }
}

export class PowerUpDto implements IDto {
    id: string;
    pos: Vector;
    type: string;
    collected: boolean;

    toShortDto(): IDtoShort {
        const shortDto = new PowerUpDtoShort();
        shortDto.i = this.id;
        shortDto.p = this.pos;
        shortDto.t = this.type;
        shortDto.c = this.collected;
        return shortDto;
    }
}

export class HelloDto implements IDto {
    gameSettings: GameSettingsDto;
    world: WorldUpdateDto;

    toShortDto(): IDtoShort {
        const shortDto = new HelloDtoShort();
        shortDto.gs = this.gameSettings.toShortDto() as GameSettingsDtoShort;
        shortDto.w = this.world.toShortDto() as WorldUpdateDtoShort;
        return shortDto;
    }
}

export class GameJoinDto implements IDto {
    player: PlayerDto;
    scores: ScoreDto[];

    toShortDto(): IDtoShort {
        const shortDto = new GameJoinDtoShort();
        shortDto.p = this.player.toShortDto() as PlayerDtoShort;
        shortDto.s = this.scores.map(s => s.toShortDto() as ScoreDtoShort);
        return shortDto;
    }
}

export class ArenaDto implements IDto {
    size: number;

    toShortDto(): IDtoShort {
        const shortDto = new ArenaDtoShort();
        shortDto.s = this.size;
        return shortDto;
    }
}

export class TrailSegmentDto implements IDto {
    isHead: boolean;
    start: Vector;
    end: Vector;

    toShortDto(): IDtoShort {
        const shortDto = new TrailSegmentDtoShort();
        shortDto.ih = this.isHead;
        shortDto.s = this.start;
        shortDto.e = this.end;
        return shortDto;
    }
}

export class WorldUpdateDto implements IDto {
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

    toShortDto(): IDtoShort {
        const shortDto = new WorldUpdateDtoShort();
        shortDto.t = this.timestamp;
        shortDto.gt = this.gameTick;
        shortDto.rip = this.roundInProgress;
        shortDto.rtl = this.roundTimeLeft;
        shortDto.tunr = this.timeUntilNextRound;
        shortDto.cw = this.currentWinner;
        shortDto.p = this.players.map(p => p.toShortDto() as PlayerDtoShort);
        shortDto.pu = this.powerUps.map(pu => pu.toShortDto() as PowerUpDtoShort);
        shortDto.a = this.arena.toShortDto() as ArenaDtoShort;
        shortDto.d = this.debug.toShortDto() as DebugDtoShort;
        return shortDto;
    }
}

export class ChatMessageDto implements IDto {
    message: string;
    timestamp: number;
    source: string;
    sourceColour: string; // includes %A% alpha
    isSystemMessage: boolean;

    toShortDto(): IDtoShort {
        const shortDto = new ChatMessageDtoShort();
        shortDto.m = this.message;
        shortDto.t = this.timestamp;
        shortDto.s = this.source;
        shortDto.sc = this.sourceColour;
        shortDto.ism = this.isSystemMessage;
        return shortDto;
    }
}

export class ClientChatMessageDto implements IDto {
    message: string;

    toShortDto(): IDtoShort {
        const shortDto = new SendChatMessageDtoShort();
        shortDto.m = this.message;
        return shortDto;
    }
}

export class ScoreDto implements IDto {
    playerId: string;
    name: string;
    score: number;

    toShortDto(): IDtoShort {
        const shortDto = new ScoreDtoShort();
        shortDto.i = this.playerId;
        shortDto.n = this.name;
        shortDto.s = this.score;
        return shortDto;
    }
}

export class GameSettingsDto implements IDto {
    gameTickMs: number;

    toShortDto(): IDtoShort {
        const shortDto = new GameSettingsDtoShort();
        shortDto.gt = this.gameTickMs;
        return shortDto;
    }
}

export class DebugDto implements IDto {
    impacts: ImpactDto[];

    toShortDto(): IDtoShort {
        const shortDto = new DebugDtoShort();
        shortDto.i = this.impacts.map(i => i.toShortDto() as ImpactDtoShort);
        return shortDto;
    }
}

export class ImpactDto implements IDto {
    pos: Vector;

    toShortDto(): IDtoShort {
        const shortDto = new ImpactDtoShort();
        shortDto.p = this.pos;
        return shortDto;
    }
}

export class ClientUpdateDto implements IDto {
    playerId: string;
    xDir: number;
    yDir: number;
    xPos: number;
    yPos: number;

    toShortDto(): IDtoShort {
        const shortDto = new ClientUpdateDtoShort();
        shortDto.i = this.playerId;
        shortDto.xd = this.xDir;
        shortDto.yd = this.yDir;
        shortDto.xp = this.xPos;
        shortDto.yp = this.yPos;
        return shortDto;
    }
}

export class ClientGameJoinDto implements IDto {
    name: string;

    toShortDto(): IDtoShort {
        const shortDto = new ClientGameJoinDtoShort();
        shortDto.n = this.name;
        return shortDto;
    }
}