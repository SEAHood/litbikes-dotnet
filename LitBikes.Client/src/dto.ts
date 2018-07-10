import { Vector } from "./util";

export interface BikeDto {
    playerId: number;
    name: string;
    pos: Vector;
    dir: Vector;
    spd: number;
    trail: TrailSegmentDto[];
    colour: string; // includes %A% alpha
}

export interface PlayerDto {
    playerId: number;
    name: string;
    bike: BikeDto;
    crashed: boolean;
    crashing: boolean;
    crashedInto: number;
    crashedIntoName: string;
    deathTimestamp?: number
    spectating: boolean;
    score: number;
    currentPowerUp: string;
    effect: string;
}

export interface PowerUpDto {
    id: string;
    pos: Vector;
    type: string;
    collected: boolean;
}

export interface HelloDto {
    gameSettings: GameSettingsDto;
    world: WorldUpdateDto;
}

export interface GameJoinDto {
    player: PlayerDto;
    scores: ScoreDto[];
}

export interface ArenaDto {
    size: number;
}

export interface TrailSegmentDto {
    isHead: boolean;
    start: Vector;
    end: Vector;
}

export interface WorldUpdateDto {
    timestamp: number;
    gameTick: number;
    roundInProgress: boolean;
    roundTimeLeft: number;
    timeUntilNextRound: number;
    currentWinner: number;
    players: PlayerDto[];
    powerUps: PowerUpDto[];
    arena: ArenaDto;
    debug: DebugDto;
}

export interface ChatMessageDto {
    message: string;
    timestamp: number;
    source: string;
    sourceColour: string; // includes %A% alpha
    isSystemMessage: boolean;
}

export interface SendChatMessageDto {
    message: string
}

export interface ScoreDto {
    playerId: number;
    name: string;
    score: number;
}

export interface GameSettingsDto {
    gameTickMs: number;
}

export interface DebugDto {
    impacts: ImpactDto[];
}

export interface ImpactDto {
    pos: Vector;
}

export interface ClientUpdateDto {
    playerId: number;
    xDir: number;
    yDir: number;
    xPos: number;
    yPos: number;
}

export interface ClientGameJoinDto {
    name: string;
}