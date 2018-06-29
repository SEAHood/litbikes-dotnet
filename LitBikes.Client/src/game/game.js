"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const bike_1 = require("../model/bike");
const player_1 = require("../model/player");
const powerUp_1 = require("../model/powerUp");
const arena_1 = require("../model/arena");
const util_1 = require("../util");
require("p5");
const signalR = require("@aspnet/signalr");
const _ = require("underscore");
const $ = require("jquery");
class Game {
    constructor() {
        this.host = 'http://' + window.location.hostname + ':9092';
        //private socket = null;//io.connect(this.host);
        this.hubConnection = new signalR.HubConnectionBuilder().withUrl("/hub").build();
        this.players = [];
        this.powerUps = [];
        this.registered = false;
        this.version = "0.1b";
        this.gameStarted = false;
        this.gameJoined = false;
        this.showDebug = false;
        this.showRespawn = true;
        this.serverTimedOut = false;
        this.hubConnection.on('hello', (data) => {
            this.initGame(data);
        });
        this.hubConnection.on('joined-game', (data) => {
            this.joinGame(data);
        });
        setInterval(() => {
            this.timeKeepAliveSent = new Date().getTime();
            this.hubConnection.invoke('keep-alive');
        }, 1000);
        this.hubConnection.on('keep-alive-ack', data => {
            let timeNow = new Date().getTime();
            this.latency = timeNow - this.timeKeepAliveSent;
            this.refreshServerTimeout();
        });
        this.hubConnection.on('world-update', (data) => {
            if (this.gameStarted) {
                this.processWorldUpdate(data);
            }
        });
        this.hubConnection.on('score-update', (data) => {
            this.updateScores(data);
        });
        this.hubConnection.on('chat-message', (data) => {
            // TODO: Use moment or something?
            let messageTime = new Date(data.timestamp).toTimeString().split(' ')[0];
            let chatElement = "<li>";
            chatElement += "[" + messageTime + "]";
            if (data.isSystemMessage) {
                chatElement += "&nbsp;<span style='color:#AFEEEE'>" + _.escape(data.message) + "</span>";
            }
            else {
                let colour = data.sourceColour.replace("%A%", "100");
                chatElement += "&nbsp;<span style='color:" + colour + "'><strong>" + data.source + "</strong></span>:";
                chatElement += "&nbsp;" + _.escape(data.message);
            }
            chatElement += "</li>";
            $('#chat-log ul').append(chatElement);
            $('#chat-log').scrollTop($('#chat-log')[0].scrollHeight);
            if ($('#message-list li').length > 250) {
                $('#message-list li').first().remove();
            }
            this.messageCount = $("#message-list li").length;
        });
        var Keys;
        (function (Keys) {
            Keys[Keys["LEFT_ARROW"] = 37] = "LEFT_ARROW";
            Keys[Keys["UP_ARROW"] = 38] = "UP_ARROW";
            Keys[Keys["RIGHT_ARROW"] = 39] = "RIGHT_ARROW";
            Keys[Keys["DOWN_ARROW"] = 40] = "DOWN_ARROW";
            Keys[Keys["W"] = 87] = "W";
            Keys[Keys["A"] = 65] = "A";
            Keys[Keys["S"] = 83] = "S";
            Keys[Keys["D"] = 68] = "D";
            Keys[Keys["R"] = 82] = "R";
            Keys[Keys["SPACE"] = 32] = "SPACE";
            Keys[Keys["F3"] = 114] = "F3";
            Keys[Keys["H"] = 72] = "H";
            Keys[Keys["TAB"] = 9] = "TAB";
            Keys[Keys["ENTER"] = 13] = "ENTER";
        })(Keys || (Keys = {}));
        ;
        $(document).on('keyup', ev => {
            let keyCode = ev.which;
            if (this.player) {
                if (keyCode === Keys.TAB) {
                    this.tabPressed = false;
                }
            }
        });
        $(document).on('keydown', ev => {
            if ($(ev.target).is('input')) {
                // Typing in chat, don't process as game keys
                if (ev.which === Keys.ENTER) { // enter
                    if ($(ev.target).is('#player-name-input')) { // enter when inside player name box
                        let name = $('#player-name-input').val();
                        if (this.nameIsValid(name.toString())) {
                            this.requestJoinGame(name.toString());
                        }
                    }
                    else if ($(ev.target).is('#chat-input')) { // enter when inside chat box
                        let message = $('#chat-input').val();
                        if (message.toString().trim() != "") {
                            this.hubConnection.invoke('chat-message', message);
                            $('#chat-input').val('');
                        }
                    }
                    $(ev.target).blur();
                }
                return;
            }
            if (this.player) {
                let keyCode = ev.which;
                let newVector = null;
                let eventMatched = true;
                if (keyCode === Keys.UP_ARROW || keyCode === Keys.W) {
                    newVector = new util_1.Vector(0, -1);
                }
                else if (keyCode === Keys.DOWN_ARROW || keyCode === Keys.S) {
                    newVector = new util_1.Vector(0, 1);
                }
                else if (keyCode === Keys.RIGHT_ARROW || keyCode === Keys.D) {
                    newVector = new util_1.Vector(1, 0);
                }
                else if (keyCode === Keys.LEFT_ARROW || keyCode === Keys.A) {
                    newVector = new util_1.Vector(-1, 0);
                }
                else if (keyCode === Keys.F3) {
                    this.showDebug = !this.showDebug;
                }
                else if (keyCode === Keys.R) {
                    this.hubConnection.invoke('request-respawn');
                }
                else if (keyCode === Keys.H) {
                    this.showRespawn = !this.showRespawn;
                }
                else if (keyCode === Keys.TAB) {
                    this.tabPressed = true;
                }
                else if (keyCode === Keys.SPACE) {
                    this.hubConnection.invoke('use-powerup');
                }
                else if (keyCode === Keys.ENTER) {
                    $('#chat-input').focus();
                }
                else {
                    eventMatched = false;
                }
                if (eventMatched) {
                    ev.preventDefault();
                    ev.stopPropagation();
                }
                if (newVector) {
                    //this.player.setDirection(newVector);
                    //this.sendClientUpdate();
                    // TODO MOVE THIS SOMEWHERE ELSE
                    let updateDto = {
                        pid: this.player.getPid(),
                        xDir: newVector.x,
                        yDir: newVector.y,
                        xPos: this.player.getBike().getPos().x,
                        yPos: this.player.getBike().getPos().y
                    };
                    this.hubConnection.invoke('update', updateDto);
                }
            }
        });
        $(document).ready(() => {
            $('#player-name-input').on('input', () => {
                let name = $('#player-name-input').val();
                if (this.nameIsValid(name.toString())) {
                    $('#player-name-submit').show();
                }
                else {
                    $('#player-name-submit').hide();
                }
            });
            $('#player-name-submit').on('click', () => {
                let name = $('#player-name-input').val();
                this.requestJoinGame(name.toString());
            });
        });
        this.hubConnection.invoke('hello');
    }
    setup(p) {
        this.mainFont = p.loadFont('fonts/3Dventure.ttf');
        this.secondaryFont = p.loadFont('fonts/visitor.ttf');
        this.debugFont = p.loadFont('fonts/larabie.ttf');
        this.powerUpIconRocket = p.loadImage('images/game/powerups/rocket.png');
        this.powerUpIconSlow = p.loadImage('images/game/powerups/slow.png');
        p.createCanvas(this.arena.size, this.arena.size);
    }
    nameIsValid(name) {
        return name.trim().length > 1 && name.trim().length <= 15;
    }
    refreshServerTimeout() {
        if (this.serverTimeoutTimer) {
            clearInterval(this.serverTimeoutTimer);
            this.serverTimeoutTimer = null;
        }
        this.serverTimedOut = false;
        this.serverTimeoutTimer = setInterval(() => this.serverTimedOut = true, 5000);
    }
    requestJoinGame(name) {
        let joinObj = {
            name: name
        };
        this.hubConnection.invoke('request-join-game', joinObj);
    }
    joinGame(data) {
        $('#welcome-container').hide();
        $('#info-container').slideDown();
        this.gameJoined = true;
        this.player = new player_1.Player(data.player.pid, data.player.name, new bike_1.Bike(data.player.bike), data.player.crashed, data.player.spectating, data.player.deathTimestamp, data.player.crashedInto, data.player.crashedIntoName, data.player.score, true);
        this.updateScores(data.scores);
    }
    initGame(data) {
        if (!data.gameSettings.gameTickMs) {
            console.error("Cannot start game - game tick interval is not defined");
        }
        this.gameTick = data.world.gameTick;
        this.arena = new arena_1.Arena(data.world.arena);
        this.gameTickMs = data.gameSettings.gameTickMs;
        this.processWorldUpdate(data.world);
        this.p5Instance = new p5(this.sketch(), $('#game-container')[0]);
        this.gameStarted = true;
        $('#game').width(this.arena.size);
        $('#game').height(this.arena.size);
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
            _.each(this.players, (p) => {
                // Faster farther from center - disabled just now      
                //let spdModifier = this.calculateSpeedModifier(b);                         
                //b.setSpd(baseSpeed + spdModifier);
                p.update();
            });
        }, this.gameTickMs);
    }
    calculateSpeedModifier(b) {
        let gameSize = this.arena.size;
        let center = new util_1.Vector(gameSize / 2, gameSize / 2);
        let bikePos = new util_1.Vector(b.getPos().x, b.getPos().y), distance = util_1.Vector.distance(bikePos, center), oldMin = 0, oldMax = gameSize / 2, newMin = 0, newMax = 0.5, oldRange = oldMax - oldMin, newRange = newMax - newMin, spdModifier = ((distance - oldMin) * newRange / oldRange) + newMin; // Trust me
        return spdModifier;
    }
    processWorldUpdate(data) {
        this.roundInProgress = data.roundInProgress;
        this.timeUntilNextRound = data.timeUntilNextRound;
        this.currentWinner = data.currentWinner;
        if (this.roundTimeLeft != data.roundTimeLeft) {
            var t = new Date(data.roundTimeLeft * 1000);
            var minutes = util_1.NumberUtil.pad(t.getMinutes(), 2);
            var seconds = util_1.NumberUtil.pad(t.getSeconds(), 2);
            $('#round-timer').text(minutes + ":" + seconds);
        }
        this.roundTimeLeft = data.roundTimeLeft;
        let updatedPlayers = _.pluck(data.players, 'pid');
        let existingPlayers = _.pluck(this.players, 'pid');
        _.each(existingPlayers, (pid) => {
            if (!_.contains(updatedPlayers, pid)) {
                this.players = _.reject(this.players, (p) => p.getPid() === pid);
            }
        });
        _.each(data.players, (p) => {
            if (this.gameJoined && p.pid === this.player.getPid() && this.player) {
                this.player.updateFromDto(p);
            }
            else {
                let existingPlayer = _.find(this.players, (ep) => ep.getPid() === p.pid);
                if (existingPlayer) {
                    existingPlayer.updateFromDto(p);
                }
                else {
                    let player = new player_1.Player(p.pid, p.name, new bike_1.Bike(p.bike), p.crashed, p.spectating, p.deathTimestamp, p.crashedInto, p.crashedIntoName, p.score, false);
                    this.players.push(player);
                }
            }
        });
        let powerUps = [];
        _.each(data.powerUps, (p) => {
            let powerUpDto = new powerUp_1.PowerUp(p.id, p.pos, p.type);
            let existingPowerUp = _.find(this.powerUps, (ep) => ep.getId() === p.id);
            if (existingPowerUp) {
                existingPowerUp.updateFromDto(p);
            }
            else {
                let powerUp = new powerUp_1.PowerUp(p.id, p.pos, p.type);
                this.powerUps.push(powerUp);
                existingPowerUp = powerUp;
            }
            powerUps.push(existingPowerUp);
        });
        this.powerUps = powerUps;
        this.impacts = data.debug.impacts.map(x => x.pos);
    }
    updateScores(scores) {
        scores = _.sortBy(scores, x => x.score).reverse();
        let topFive = _.first(scores, 5);
        $('#score ul').empty();
        let playerInTopFive = false;
        topFive.forEach((score, i) => {
            let isPlayer = this.gameJoined && score.pid == this.player.getPid();
            let player = _.first(this.players.filter((p) => p.getPid() == score.pid));
            playerInTopFive = isPlayer || playerInTopFive;
            let li = isPlayer ? "<li style='color:yellow'>" : "<li>";
            let position = "#" + (i + 1);
            let bikeColour = !player || isPlayer
                ? "inherit"
                : player.getBike().getColour().replace('%A%', '1');
            let scoreElement = li + position + " <span style='color:" + bikeColour + "'>" + score.name + "</span>: " + score.score + "</li>";
            $('#score ul').append(scoreElement);
        });
        if (this.gameJoined && !playerInTopFive) {
            let playerScore = scores.filter(x => x.pid == this.player.getPid())[0];
            if (!playerScore) {
                return;
            }
            let li = "<li style='color:yellow'>";
            let position = "#" + (scores.indexOf(playerScore) + 1);
            let scoreElement = li + position + " " + playerScore.name + ": " + playerScore.score + "</li>";
            $('#score ul').append(scoreElement);
        }
    }
    getPowerUpIcon(powerUp) {
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
    sketch() {
        return (p) => {
            p.setup = () => this.setup(p);
            p.draw = () => this.draw(p);
        };
    }
    draw(p) {
        let halfWidth = this.arena.size / 2;
        let halfHeight = this.arena.size / 2;
        this.arena.draw(p);
        if (this.roundInProgress) {
            _.each(this.powerUps, (powerUp) => {
                powerUp.draw(p);
            });
            _.each(this.players, (player) => {
                player.draw(p, this.tabPressed);
            });
        }
        if (this.serverTimedOut) {
            p.noStroke();
            p.fill('rgba(0,0,0,0.4)');
            p.rect(0, halfHeight - 35, this.arena.size, 55);
            p.textFont(this.mainFont);
            p.textAlign('center', 'top');
            p.fill('rgba(125,249,255,0.50)');
            p.textSize(29);
            p.text("He's dead, Jim", halfWidth + util_1.NumberUtil.randInt(0, 2), halfHeight - 30 + util_1.NumberUtil.randInt(0, 2));
            p.fill('rgba(255,255,255,0.80)');
            p.textSize(28);
            p.text("He's dead, Jim", halfWidth, halfHeight - 30);
            p.fill('rgba(0,0,0,0.40)');
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
                    }
                    else {
                        p.tint(255, 200);
                    }
                    p.image(powerUpIcon, 5, 5, 30, 30);
                }
            }
            if (this.player.isCrashed() && this.player.isSpectating() && this.showRespawn && this.roundInProgress) {
                p.noStroke();
                p.fill('rgba(0,0,0,0.4)');
                p.rect(0, halfHeight - 35, this.arena.size, 100);
                p.textFont(this.mainFont);
                p.textAlign('center', 'top');
                if (this.player.isCrashed()) {
                    let suicide = this.player.getCrashedInto() == this.player.getPid();
                    let crashText = "";
                    if (suicide) {
                        crashText = "You killed yourself, idiot";
                    }
                    else {
                        crashText = "Killed by " + this.player.getCrashedIntoName();
                    }
                    p.fill('rgba(125,249,255,0.50)');
                    p.textSize(29);
                    p.text(crashText, halfWidth + util_1.NumberUtil.randInt(0, 2), halfHeight - 30 + util_1.NumberUtil.randInt(0, 2));
                    p.fill('rgba(255,255,255,0.80)');
                    p.textSize(28);
                    p.text(crashText, halfWidth, halfHeight - 30);
                }
                p.fill('rgba(125,249,255,0.50)');
                p.textSize(33);
                p.text("Press 'R' to respawn", halfWidth + util_1.NumberUtil.randInt(0, 2), halfHeight + util_1.NumberUtil.randInt(0, 2));
                p.fill('rgba(255,255,255,0.80)');
                p.textSize(32);
                p.text("Press 'R' to respawn", halfWidth, halfHeight);
                p.fill('rgba(0,0,0,0.40)');
                p.fill(255);
                p.textFont(this.secondaryFont);
                p.textSize(15);
                p.text("Press 'H' to hide", halfWidth, halfHeight + 45);
            }
        }
        if (!this.roundInProgress) {
            let winner = this.gameJoined && this.currentWinner === this.player.getPid()
                ? this.player
                : _.find(this.players, (player) => player.getPid() === this.currentWinner);
            let winnerName = "The Wall";
            if (winner)
                winnerName = winner.getName();
            p.noStroke();
            p.fill('rgba(0,0,0,0.4)');
            p.rect(0, halfHeight - 35, this.arena.size, 55);
            p.textFont(this.mainFont);
            p.textAlign('center', 'top');
            p.fill('rgba(125,249,255,0.50)');
            p.textSize(29);
            p.text(winnerName + " won!", halfWidth + util_1.NumberUtil.randInt(0, 2), halfHeight - 30 + util_1.NumberUtil.randInt(0, 2));
            p.fill('rgba(255,255,255,0.80)');
            p.textSize(28);
            p.text(winnerName + " won!", halfWidth, halfHeight - 30);
            p.fill('rgba(0,0,0,0.40)');
            p.fill(255);
            p.textFont(this.secondaryFont);
            p.textSize(15);
            p.text("Next round starting in " + this.timeUntilNextRound + " second" + (this.timeUntilNextRound === 1 ? "" : "s"), halfWidth, halfHeight);
        }
        if (this.player && this.player.isAlive() && this.player.getEffect().toLowerCase() == "slowed") {
            p.filter("INVERT", 0);
        }
        _.each(this.impacts, (i) => {
            p.noStroke();
            p.fill('rgba(255, 165, 0, 0.6)');
            p.ellipse(i.x, i.y, 20, 20);
        });
        // DEBUG ////////////////////////
        if (this.showDebug) {
            p.textFont(this.debugFont);
            p.fill(255);
            p.textSize(15);
            p.textAlign('left', 'top');
            p.text("LitBikes " + this.version, 10, 10);
            if (this.gameJoined) {
                let playerBike = this.player.getBike();
                p.text("fps: " + p.frameRate().toFixed(2) + "\n" +
                    "ms: " + this.latency + "ms\n" +
                    "pid: " + this.player.getPid() + "\n" +
                    "pos: " + playerBike.getPos().x.toFixed(0) + ", " + playerBike.getPos().y.toFixed(0) + "\n" +
                    "dir: " + playerBike.getDir().x + ", " + playerBike.getDir().y + "\n" +
                    "spd: " + playerBike.getSpd() + "\n" +
                    "crashed: " + (this.player.isCrashed() ? "yes" : "no") + "\n" +
                    "crashing: " + (playerBike.isCrashing() ? "yes" : "no") + "\n" + +"colour: " + playerBike.getColour() + "\n" +
                    "spectating: " + (this.player.isSpectating() ? "yes" : "no") + "\n" +
                    "round in progress: " + (this.roundInProgress ? "yes" : "no") + "\n" +
                    "round time left: " + this.roundTimeLeft + "\n" +
                    "time until next round: " + this.timeUntilNextRound + "\n" +
                    "other players: " + this.players.length + "\n" +
                    "chat message count: " + this.messageCount + "\n", 10, 30, 300, 500);
            }
            else {
                p.text("Game not joined", 10, 30, 300, 500);
            }
        }
        // DEBUG ////////////////////////
    }
}
exports.Game = Game;
//new Game();
var hubConnection = new signalR.HubConnectionBuilder().withUrl("http://localhost:59833/hub").build();
hubConnection.on("ReceiveMessage", (user, message) => {
    const msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    const encodedMsg = user + " says " + msg;
    const li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});
hubConnection.start().catch(err => console.error(err.toString()));
document.getElementById("sendButton").addEventListener("click", event => {
    const user = $("#userInput").val();
    const message = $("#messageInput").val();
    hubConnection.invoke("SendMessage", user, message).catch(err => console.error(err.toString()));
    event.preventDefault();
});
//# sourceMappingURL=game.js.map