import { Bike } from "../model/bike"
import { Player } from "../model/player"
import { PowerUp } from "../model/powerUp"
import { Arena } from "../model/arena"
import { Vector, NumberUtil } from "../util"
import {
    WorldUpdateDto, BikeDto, PlayerDto, PowerUpDto,
    ClientUpdateDto, GameJoinDto, ClientGameJoinDto,
    HelloDto, ChatMessageDto, ScoreDto,
    SendChatMessageDto
} from "../dto"

import "p5"
import * as signalR from "@aspnet/signalr"
import * as _ from "underscore"
import * as $ from "jquery"

export class Game {
    private player : Player;
    private host = `http://${window.location.hostname}:9092`;
    //private socket = null;//io.connect(this.host);
    private hubConnection = new signalR.HubConnectionBuilder().withUrl(`http://${window.location.hostname}:59833/hub`).build();
    //private hubConnection = new signalR.HubConnectionBuilder().withUrl("/hub").build();
    private arena : Arena;
    private players : Player[] = [];
    private powerUps : PowerUp[] = [];
    private registered = false;
    private version ="0.1b";
    private gameStarted = false;
    private gameJoined = false;
    private gameTickMs : number;
    private p5Instance : p5;
    private showDebug = false;
    private showRespawn = true;
    private timeKeepAliveSent : number;
    private latency : number;
    private gameTick : number;
    private roundInProgress: boolean;
    private roundTimeLeft: number;
    private timeUntilNextRound: number;
    private currentWinner: string; // ID
    private messageCount : number;
    private tabPressed : boolean;
    private serverTimeoutTimer : any;
    private serverTimedOut : boolean = false;

    private nameInputField : any;
    private nameInputButton : any;

    private mainFont;
    private secondaryFont;
    private debugFont;
    private powerUpIconRocket: p5.Image;
    private powerUpIconSlow: p5.Image;

    private impacts: Vector[];

    constructor() {
        this.hubConnection.on("Hello", ( data : HelloDto ) => {
            this.initGame(data);
        });

        this.hubConnection.on("JoinedGame", (data : GameJoinDto ) => {
            this.joinGame(data);
        });
        
        this.hubConnection.on("KeepAliveAck", data => {
            let timeNow = new Date().getTime();
            this.latency = timeNow - this.timeKeepAliveSent;
            this.refreshServerTimeout();
        });

        this.hubConnection.on("WorldUpdate", ( data : WorldUpdateDto ) => {
            if ( this.gameStarted ) {
                this.processWorldUpdate(data);
            }
        });

        this.hubConnection.on("ScoreUpdate", (data: ScoreDto[]) => {
            this.updateScores(data);
        });

        this.hubConnection.on("ChatMessage", ( data : ChatMessageDto ) => {
            // TODO: Use moment or something?
            let messageTime = new Date(data.timestamp).toTimeString().split(" ")[0];
            let chatElement = "<li>";

            chatElement += `[${messageTime}]`;
            if ( data.isSystemMessage ) {
                chatElement += `&nbsp;<span style='color:#AFEEEE'>${_.escape(data.message)}</span>`;
            } else {
                let colour = data.sourceColour.replace("%A%", "100");
                chatElement += `&nbsp;<span style='color:${colour}'><strong>${data.source}</strong></span>:`;
                chatElement += `&nbsp;${_.escape(data.message)}`;
            }
            chatElement += "</li>";

            $("#chat-log ul").append(chatElement);
            $("#chat-log").scrollTop($("#chat-log")[0].scrollHeight);

            if ( $("#message-list li").length > 250 ) {
                $("#message-list li").first().remove();
            }

            this.messageCount = $("#message-list li").length;
        });

        enum Keys {
            LEFT_ARROW = 37,
            UP_ARROW = 38,
            RIGHT_ARROW = 39,
            DOWN_ARROW = 40,
            W = 87,
            A = 65,
            S = 83,
            D = 68,
            R = 82,
            SPACE = 32,
            F3 = 114,
            H = 72,
            TAB = 9,
            ENTER = 13
        };

        $(document).on("keyup", ev => {
            console.log("OKAJWDNKAJWND");
            let keyCode = ev.which;
            if ( this.player ) {
                if (keyCode === Keys.TAB) {
                    this.tabPressed = false;
                }
            }
        });

        $(document).on("keydown", ev => {
            console.log("OKAJWDNKAJWND");
            if ( $(ev.target).is("input") ) {
                // Typing in chat, don't process as game keys
                if ( ev.which === Keys.ENTER ) { // enter
                    if ($(ev.target).is("#player-name-input")) { // enter when inside player name box
                        let name = $("#player-name-input").val();
                        if (this.nameIsValid(name.toString())) {
                            this.requestJoinGame(name.toString());
                        }                            
                    } else if ($(ev.target).is("#chat-input")) { // enter when inside chat box
                        let message = $("#chat-input").val();
                        if (message.toString().trim() != "") {
                            this.hubConnection.invoke("ChatMessage", message);
                            $("#chat-input").val("");
                        }
                    }
                    $(ev.target).blur();
                }
                return;
            }
                
            if ( this.player ) {

                let keyCode = ev.which;
                let newVector = null;
                let eventMatched = true;

                if (keyCode === Keys.UP_ARROW || keyCode === Keys.W) {
                    newVector = new Vector(0, -1);
                } else if (keyCode === Keys.DOWN_ARROW || keyCode === Keys.S) {
                    newVector = new Vector(0, 1);
                } else if (keyCode === Keys.RIGHT_ARROW || keyCode === Keys.D) {
                    newVector = new Vector(1, 0);
                } else if (keyCode === Keys.LEFT_ARROW || keyCode === Keys.A) {
                    newVector = new Vector(-1, 0);
                } else if (keyCode === Keys.F3) {
                    this.showDebug = !this.showDebug;
                } else if (keyCode === Keys.R) {
                    this.hubConnection.invoke("RequestRespawn");
                } else if (keyCode === Keys.H) {
                    this.showRespawn = !this.showRespawn;
                } else if (keyCode === Keys.TAB) {
                    this.tabPressed = true;
                } else if (keyCode === Keys.SPACE) {
                    this.hubConnection.invoke("UsePowerup");
                } else if (keyCode === Keys.ENTER) {
                    $("#chat-input").focus();
                } else {
                    eventMatched = false;
                }

                if ( eventMatched ) {
                    ev.preventDefault();
                    ev.stopPropagation();
                }

                if ( newVector ) {
                    //this.player.setDirection(newVector);
                    //this.sendClientUpdate();
                    // TODO MOVE THIS SOMEWHERE ELSE
                    let updateDto : ClientUpdateDto = {
                        playerId : this.player.getPlayerId(),
                        xDir : newVector.x,
                        yDir : newVector.y,
                        xPos : this.player.getBike().getPos().x,
                        yPos : this.player.getBike().getPos().y
                    };
                    this.hubConnection.invoke("Update", updateDto);
                }
            }
        });
            
        $(document).ready(() => {        
            $("#player-name-input").on("input", () => {
                console.log("OKAJWDNKAJWND");
                let name = $("#player-name-input").val();
                if (this.nameIsValid(name.toString())) {
                    $("#player-name-submit").show();
                } else {
                    $("#player-name-submit").hide();
                }
            });
                
            $("#player-name-submit").on("click", () => {
                let name = $("#player-name-input").val();
                this.requestJoinGame(name.toString());
            });
        });
    }

    go() {
        $(document).on("keyup", ev => {
            console.log("OKAJWDNKAJWND");
        });

        this.hubConnection.start().then(() => {
            this.hubConnection.invoke("Hello");
            setInterval(() => {
                this.timeKeepAliveSent = new Date().getTime();
                this.hubConnection.invoke("KeepAlive");
            }, 1000);
        }).catch(err => console.error(err.toString()));
    }
        
    private setup( p : p5 ) {
        this.mainFont = p.loadFont("fonts/3Dventure.ttf");
        this.secondaryFont = p.loadFont("fonts/visitor.ttf");
        this.debugFont = p.loadFont("fonts/larabie.ttf");
            
        this.powerUpIconRocket = p.loadImage("img/game/powerups/rocket.png");
        this.powerUpIconSlow = p.loadImage("img/game/powerups/slow.png");

        p.createCanvas(this.arena.size, this.arena.size);
    }

    private nameIsValid(name: string): boolean {
        return name.trim().length > 1 && name.trim().length <= 15;
    }

    private refreshServerTimeout() {
        if (this.serverTimeoutTimer) {
            clearInterval(this.serverTimeoutTimer);
            this.serverTimeoutTimer = null;
        }
        this.serverTimedOut = false;
        this.serverTimeoutTimer = setInterval(() => this.serverTimedOut = true, 5000);
    }

    private requestJoinGame(name: string) {
        const joinObj: ClientGameJoinDto = {
            name: name
        };
        this.hubConnection.invoke("RequestJoinGame", joinObj);
    }

    private joinGame( data : GameJoinDto ) {
        $("#welcome-container").hide();
        $("#info-container").slideDown();
        this.gameJoined = true;
        this.player = new Player(
            data.player.playerId, 
            data.player.name, 
            new Bike(data.player.bike), 
            data.player.crashed,
            data.player.spectating,
            data.player.deathTimestamp,
            data.player.crashedInto,
            data.player.crashedIntoName,
            data.player.score,
            true);
        this.updateScores(data.scores);
    }

    private initGame( data : HelloDto ) {
        if ( !data.gameSettings.gameTickMs ) {
            console.error("Cannot start game - game tick interval is not defined");
        }
        this.gameTick = data.world.gameTick;
        this.arena = new Arena(data.world.arena);
        this.gameTickMs = data.gameSettings.gameTickMs;
            
        this.processWorldUpdate(data.world);

        this.p5Instance = new p5(this.sketch(), $("#game-container")[0]);
        this.gameStarted = true;
            
        $("#game").width(this.arena.size);
        $("#game").height(this.arena.size);

        // MAIN UPDATE LOOP
        setInterval(() => {
            this.gameTick++;
            var baseSpeed = 1.5; // TODO: Get this from the server!
            if (this.gameJoined) {
                // Faster farther from center - disabled just now
                //let spdModifier = this.calculateSpeedModifier(this.player);
                //this.player.setSpd(baseSpeed + spdModifier)
                this.player.update();
            }
            _.each(this.players, (p : Player) => {
                // Faster farther from center - disabled just now      
                //let spdModifier = this.calculateSpeedModifier(b);                         
                //b.setSpd(baseSpeed + spdModifier);
                p.update();
            });
        }, this.gameTickMs);
    }

    private calculateSpeedModifier( b : Bike ): number {            
        let gameSize = this.arena.size;
        let center = new Vector(gameSize/2, gameSize/2);
        let bikePos = new Vector(b.getPos().x, b.getPos().y),
            distance = Vector.distance(bikePos, center),
            oldMin = 0,
            oldMax = gameSize/2,
            newMin = 0,
            newMax = 0.5,
            oldRange = oldMax - oldMin,
            newRange = newMax - newMin,
            spdModifier = ((distance - oldMin) * newRange / oldRange) + newMin; // Trust me
        return spdModifier;
    }
        
    private processWorldUpdate( data : WorldUpdateDto ) {
        this.roundInProgress = data.roundInProgress;
        this.timeUntilNextRound = data.timeUntilNextRound;
        this.currentWinner = data.currentWinner;
        console.log(data.roundTimeLeft);
        if (this.roundTimeLeft != data.roundTimeLeft) {
            var t = new Date(data.roundTimeLeft * 1000);
            var minutes = NumberUtil.pad(t.getMinutes(), 2);
            var seconds = NumberUtil.pad(t.getSeconds(), 2);
            $("#round-timer").text(minutes + ":" + seconds);
        }
        this.roundTimeLeft = data.roundTimeLeft;

        let updatedPlayers = _.pluck(data.players, "playerId");
        let existingPlayers = _.pluck(this.players, "playerId");
        _.each(existingPlayers, (playerId: string ) => {
            if ( !_.contains(updatedPlayers, playerId ) ) {
                this.players = _.reject(this.players, (p : Player) => p.getPlayerId() === playerId );
            }
        });

        _.each(data.players, (p: PlayerDto) => {
            if ( this.gameJoined && p.playerId === this.player.getPlayerId() && this.player ) {
                this.player.updateFromDto(p);
            } else {
                let existingPlayer = _.find(this.players, (ep: Player) => ep.getPlayerId() === p.playerId);
                if ( existingPlayer ) {
                    existingPlayer.updateFromDto(p);
                } else {
                    console.log("Adding new player: " + p.playerId);
                    let player = new Player(
                        p.playerId, 
                        p.name, 
                        new Bike(p.bike), 
                        p.crashed,
                        p.spectating,
                        p.deathTimestamp,
                        p.crashedInto,
                        p.crashedIntoName,
                        p.score,
                        false);             
                    this.players.push(player);
                }
            }
        });

        let powerUps = [];
        _.each(data.powerUps, (p: PowerUpDto) => {
            let powerUpDto = new PowerUp(p.id, p.pos, p.type);
            let existingPowerUp = _.find(this.powerUps, (ep: PowerUp) => ep.getId() === p.id);
            if ( existingPowerUp ) {
                existingPowerUp.updateFromDto(p);
            } else {
                let powerUp = new PowerUp(p.id, p.pos, p.type);
                this.powerUps.push(powerUp);
                existingPowerUp = powerUp;
            }
            powerUps.push(existingPowerUp);
        });
        this.powerUps = powerUps;
        this.impacts = data.debug.impacts.map(x => x.pos);
    }

    private updateScores(scores: ScoreDto[]) {            
        scores = _.sortBy(scores, x => x.score).reverse();
        let topFive = _.first(scores, 5);
        $("#score ul").empty();
        let playerInTopFive = false;
        topFive.forEach((score: ScoreDto, i: number) => {                
            let isPlayer = this.gameJoined && score.playerId == this.player.getPlayerId();
            let player: Player = _.first(this.players.filter((p: Player) => p.getPlayerId() == score.playerId));
            playerInTopFive = isPlayer || playerInTopFive;
            let li = isPlayer ? "<li style='color:yellow'>" : "<li>";
            let position = `#${i + 1}`;
            let bikeColour = !player || isPlayer 
                ? "inherit"
                :  player.getBike().getColour().replace("%A%", "1");
            let scoreElement = li + position + " <span style='color:" + bikeColour + "'>" + score.name + "</span>: " + score.score + "</li>";
            $("#score ul").append(scoreElement);
        });

        if (this.gameJoined && !playerInTopFive) {
            let playerScore = scores.filter(x => x.playerId == this.player.getPlayerId())[0];
            if (!playerScore) {
                return;
            }
            let li = "<li style='color:yellow'>";
            let position = `#${scores.indexOf(playerScore) + 1}`;
            let scoreElement = li + position + " " + playerScore.name + ": " + playerScore.score + "</li>";
            $("#score ul").append(scoreElement);
        }
    }

    private getPowerUpIcon(powerUp: string) {            
        let powerUpIcon = null;
        switch (powerUp) {
            case "rocket":
                return this.powerUpIconRocket;
            case "slow":
                return this.powerUpIconSlow;
            default:
                return null;
        }
    }

    private sketch() {
        return ( p : p5 ) => {
            p.setup = () => this.setup(p);
            p.draw = () => this.draw(p);
        }
    }

    private draw( p : p5 ) {
        let halfWidth = this.arena.size / 2;
        let halfHeight = this.arena.size / 2;

        this.arena.draw(p);
        
        if (this.roundInProgress) {
            _.each( this.powerUps, (powerUp: PowerUp) => {
                powerUp.draw(p);
            });

            _.each( this.players, ( player: Player ) => {
                player.draw(p, this.tabPressed);
            });
        }

        if ( this.serverTimedOut ) {
            p.noStroke();
            p.fill("rgba(0,0,0,0.4)");
            p.rect(0, halfHeight - 35, this.arena.size, 55);

            p.textFont(this.mainFont);
            p.textAlign("center", "top");

            p.fill("rgba(125,249,255,0.50)");
            p.textSize(29);
            p.text("He's dead, Jim",
                halfWidth + NumberUtil.randInt(0, 2), halfHeight - 30 + NumberUtil.randInt(0, 2));
            p.fill("rgba(255,255,255,0.80)");
            p.textSize(28);
            p.text("He's dead, Jim",
                halfWidth, halfHeight - 30);

            p.fill("rgba(0,0,0,0.40)");
            p.fill(255);
            p.textFont(this.secondaryFont);
            p.textSize(15);
            p.text("Lost connection to the server", halfWidth, halfHeight);

            return;
        }

        if (this.gameJoined) {
            if (this.roundInProgress) {
                this.player.draw(p, this.tabPressed);
                let powerUpIcon = this.getPowerUpIcon(this.player.getCurrentPowerUp()); 
                if (powerUpIcon) {
                    let pos = this.player.getBike().getPos();
                    if (pos.x < 75 && pos.y < 75) {
                        p.tint(255, 100);
                    } else {
                        p.tint(255, 200);
                    }
                    p.image(powerUpIcon, 5, 5, 30, 30);
                }
            }

            if ( this.player.isCrashed() && this.player.isSpectating() && this.showRespawn && this.roundInProgress ) {
                p.noStroke();
                p.fill("rgba(0,0,0,0.4)");
                p.rect(0, halfHeight - 35, this.arena.size, 100);

                p.textFont(this.mainFont);
                p.textAlign("center", "top");

                if ( this.player.isCrashed() ) {
                    let suicide = this.player.getCrashedInto() == this.player.getPlayerId();
                    let crashText = "";
                    if (suicide) {
                        crashText = "You killed yourself, idiot";
                    } else {
                        crashText = `Killed by ${this.player.getCrashedIntoName()}`;
                    }
                    p.fill("rgba(125,249,255,0.50)");
                    p.textSize(29);
                    p.text(crashText,
                        halfWidth + NumberUtil.randInt(0, 2), halfHeight - 30 + NumberUtil.randInt(0, 2));
                    p.fill("rgba(255,255,255,0.80)");
                    p.textSize(28);
                    p.text(crashText,
                        halfWidth, halfHeight - 30);
                }

                p.fill("rgba(125,249,255,0.50)");
                p.textSize(33);
                p.text("Press 'R' to respawn",
                    halfWidth + NumberUtil.randInt(0, 2), halfHeight + NumberUtil.randInt(0, 2));

                p.fill("rgba(255,255,255,0.80)");
                p.textSize(32);
                p.text("Press 'R' to respawn", halfWidth, halfHeight);

                p.fill("rgba(0,0,0,0.40)");
                p.fill(255);
                p.textFont(this.secondaryFont);
                p.textSize(15);
                p.text("Press 'H' to hide", halfWidth, halfHeight + 45);
            }
        } 

        if (!this.roundInProgress) {
            let winner = this.gameJoined && this.currentWinner === this.player.getPlayerId()
                ? this.player
                : _.find(this.players, (player: Player) => player.getPlayerId() === this.currentWinner);
                
            let winnerName = "The Wall";
            if (winner)
                winnerName = winner.getName();

            p.noStroke();
            p.fill("rgba(0,0,0,0.4)");
            p.rect(0, halfHeight - 35, this.arena.size, 55);

            p.textFont(this.mainFont);
            p.textAlign("center", "top");

            p.fill("rgba(125,249,255,0.50)");
            p.textSize(29);
            p.text(winnerName + " won!",
                halfWidth + NumberUtil.randInt(0, 2), halfHeight - 30 + NumberUtil.randInt(0, 2));
            p.fill("rgba(255,255,255,0.80)");
            p.textSize(28);
            p.text(winnerName + " won!",
                halfWidth, halfHeight - 30);

            p.fill("rgba(0,0,0,0.40)");
            p.fill(255);
            p.textFont(this.secondaryFont);
            p.textSize(15);
            p.text(`Next round starting in ${this.timeUntilNextRound} second${this.timeUntilNextRound === 1 ? "" : "s"}`, halfWidth, halfHeight);
        }
            
        if (this.player && this.player.isAlive() && this.player.getEffect().toLowerCase() == "slowed") {
            p.filter("INVERT", 0);
        }

        _.each(this.impacts, (i: Vector) => {                
            p.noStroke();
            p.fill("rgba(255, 165, 0, 0.6)");
            p.ellipse(i.x, i.y, 20, 20);
        })
            
        // DEBUG ////////////////////////
        if ( this.showDebug ) {
            p.textFont(this.debugFont);
            p.fill(255);
            p.textSize(15);
            p.textAlign("left", "top");
            p.text(`LitBikes ${this.version}`, 10, 10);
            if ( this.gameJoined ) {
                let playerBike = this.player.getBike();
                p.text(
                    `fps: ${p.frameRate().toFixed(2)}\nms: ${this.latency}ms\nplayerId: ${this.player.getPlayerId()}\npos: ${playerBike.getPos().x.toFixed(0)}, ${playerBike.getPos().y.toFixed(0)}\ndir: ${playerBike.getDir().x}, ${playerBike.getDir().y}\nspd: ${playerBike.getSpd()}\ncrashed: ${this.player.isCrashed() ? "yes" : "no"}\ncrashing: ${playerBike.isCrashing() ? "yes" : "no"}\n${+
                    "colour: "}${playerBike.getColour()
                    }\nspectating: ${this.player.isSpectating() ? "yes" : "no"}\nround in progress: ${this.roundInProgress ? "yes" : "no"}\nround time left: ${this.roundTimeLeft}\ntime until next round: ${this.timeUntilNextRound}\nother players: ${this.players.length
                    }\nchat message count: ${this.messageCount}\n`
                , 10, 30, 300, 500);
            } else {
                p.text("Game not joined", 10, 30, 300, 500);
            }
        }
        // DEBUG ////////////////////////
    }
}

let game = new Game();
game.go();