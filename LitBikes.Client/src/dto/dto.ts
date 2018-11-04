import { Vector } from "../util";
import { PowerUpType } from "../model/powerUp";

export class BikeDto {
    playerId: string;
    name: string;
    pos: Vector;
    dir: Vector;
    spd: number;
    trail: TrailSegmentDto[];
    colour: string; // includes %A% alpha
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
    currentPowerUp: PowerUpType;
    effect: string;
}

export class PowerUpDto {
    id: string;
    pos: Vector;
    type: PowerUpType;
    collected: boolean;
}

export class HelloDto {
    gameSettings: GameSettingsDto;
    world: WorldUpdateDto;
}

export class GameJoinDto {
    player: PlayerDto;
    scores: ScoreDto[];
}

export class ArenaDto {
    size: number;
}

export class TrailSegmentDto {
    isHead: boolean;
    start: Vector;
    end: Vector;
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
}

export class ChatMessageDto {
    message: string;
    timestamp: number;
    source: string;
    sourceColour: string; // includes %A% alpha
    isSystemMessage: boolean;
}

export class ClientChatMessageDto {
    message: string;
}

export class ScoreDto {
    playerId: string;
    name: string;
    score: number;
}

export class GameSettingsDto {
    gameTickMs: number;
}

export class DebugDto {
    impacts: ImpactDto[];
}

export class ImpactDto {
    pos: Vector;
}

export class ClientUpdateDto {
    playerId: string;
    xDir: number;
    yDir: number;
    xPos: number;
    yPos: number;
}

export class ClientGameJoinDto {
    name: string;
}