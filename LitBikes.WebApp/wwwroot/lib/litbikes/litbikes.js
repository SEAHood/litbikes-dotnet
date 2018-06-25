
/// <reference path="typings/globals/socket.io-client/index.d.ts" />
/// <reference path="typings/globals/underscore/index.d.ts" />
/// <reference path="typings/globals/jquery/index.d.ts" />
/// <reference path="typings/p5.d.ts" />
/// <reference path="util.ts" />
/// <reference path="dto.ts" />
/// <reference path="model/arena.ts" />
/// <reference path="model/trailSegment.ts" />
/// <reference path="model/bike.ts" />
/// <reference path="model/player.ts" />
/// <reference path="model/powerUp.ts" />
/// <reference path="game/game.ts" />

var Util;
(function (Util) {
    var Vector = /** @class */ (function () {
        function Vector(x, y) {
            this.x = x;
            this.y = y;
        }
        Vector.distance = function (v1, v2) {
            var a = v1.x - v2.x;
            var b = v1.y - v2.y;
            return Math.sqrt(a * a + b * b);
        };
        return Vector;
    }());
    Util.Vector = Vector;
    var NumberUtil = /** @class */ (function () {
        function NumberUtil() {
        }
        NumberUtil.randInt = function (min, max) {
            return Math.floor(Math.random() * (max - min + 1)) + min;
        };
        NumberUtil.rand255 = function () {
            return Math.floor(Math.random() * 256);
        };
        NumberUtil.sameVector = function (v1, v2, error) {
            if (error === void 0) { error = 0; }
            var xDiff = v1.x - v2.x;
            var yDiff = v1.y - v2.y;
            return (xDiff >= error && xDiff <= error) &&
                (yDiff >= error && yDiff <= error);
        };
        NumberUtil.pad = function (n, width, z) {
            z = z || '0';
            var ns = n + '';
            return ns.length >= width ? ns : new Array(width - ns.length + 1).join(z) + ns;
        };
        return NumberUtil;
    }());
    Util.NumberUtil = NumberUtil;
    var ColourUtil = /** @class */ (function () {
        function ColourUtil() {
        }
        ColourUtil.rgba = function (r, g, b, a) {
            return 'rgba(' + r + ',' + g + ',' + b + ',' + a + ')';
        };
        /* accepts parameters
         * h  Object = {h:x, s:y, v:z}
         * OR
         * h, s, v
         */
        ColourUtil.prototype.HSVtoRGB = function (h, s, v) {
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
        };
        return ColourUtil;
    }());
    Util.ColourUtil = ColourUtil;
    var Connection = /** @class */ (function () {
        function Connection(socket, pid) {
            this.socket = socket;
            this.pid = pid;
        }
        Connection.prototype.fireWorldUpdated = function (worldUpdate) {
            this.socket.emit('world-update', worldUpdate);
        };
        return Connection;
    }());
    Util.Connection = Connection;
})(Util || (Util = {}));

var Model;
(function (Model) {
    var Arena = /** @class */ (function () {
        function Arena(dto) {
            this.spacing = 10;
            this.size = dto.size;
        }
        Arena.prototype.draw = function (p) {
            p.background(51);
            // THE OLD BLUEISH GRID
            // p.strokeWeight(1);
            // p.stroke('rgba(125,249,255,0.10)');
            // for (var i = 0; i < this.size; i += this.spacing ) {
            //     p.line(i, 0, i, this.size);
            // }
            // for (var i = 0; i < this.size; i += this.spacing ) {
            //     p.line(0, i, this.size, i);
            // }            
        };
        return Arena;
    }());
    Model.Arena = Arena;
})(Model || (Model = {}));

var Model;
(function (Model) {
    var Vector = Util.Vector;
    var NumberUtil = Util.NumberUtil;
    var Bike = /** @class */ (function () {
        function Bike(bikeDto) {
            var _this = this;
            this.idRingPulseCount = 0;
            this.idRingPulseMax = 2;
            this.idRingDuration = 1500; //ms
            this.setPos(new Vector(bikeDto.pos.x, bikeDto.pos.y));
            this.dir = new Vector(bikeDto.dir.x, bikeDto.dir.y);
            this.spd = bikeDto.spd;
            this.colour = bikeDto.colour;
            this.trail = [];
            _.each(bikeDto.trail, function (seg) {
                _this.trail.push(Model.TrailSegment.fromDto(seg));
            });
            this.trailOpacity = 1;
            this.idRingBlinkTime = -1;
            this.idRingBlinkOn = false;
            this.idRingSize = 0;
            this.respawning = true;
        }
        Bike.prototype.update = function (canMove) {
            if (canMove) {
                var xDiff = this.dir.x * this.spd; // * this.timeDilation.x;
                var yDiff = this.dir.y * this.spd; // * this.timeDilation.y;
                this.setPos(new Vector(this.pos.x + xDiff, this.pos.y + yDiff));
            }
            if (this.crashing) {
                this.trailOpacity = Math.max(this.trailOpacity - 0.02, 0);
                if (this.trailOpacity == 0) {
                    this.crashing = false;
                    this.trailOpacity = 1;
                }
            }
        };
        Bike.prototype.updateFromDto = function (dto) {
            var _this = this;
            this.pos = dto.pos;
            this.dir = dto.dir;
            this.spd = dto.spd;
            this.colour = dto.colour;
            this.trail = [];
            _.each(dto.trail, function (seg) {
                _this.trail.push(Model.TrailSegment.fromDto(seg));
            });
            if (this.respawning && Date.now() - this.idRingDuration > this.lastRespawn) {
                this.respawning = false;
            }
        };
        Bike.prototype.addTrailSegment = function () {
            var lastSeg = _.last(this.trail).end;
            var newSeg = new Model.TrailSegment(new Vector(lastSeg.x, lastSeg.y), new Vector(this.pos.x, this.pos.y));
            this.trail.push(newSeg);
        };
        Bike.prototype.crash = function (timeOfCrash) {
            this.dir = new Vector(0, 0);
            this.crashing = true;
            this.addTrailSegment();
        };
        Bike.prototype.respawned = function (timeOfCrash) {
            this.respawning = true;
            this.crashing = false;
            this.trailOpacity = 1;
            this.lastRespawn = Date.now();
        };
        Bike.prototype.draw = function (p, showRespawnRing, isControlledPlayer) {
            // Respawning effect
            if (this.respawning && showRespawnRing) {
                var innerRingSize = Math.max(0, this.idRingSize - 10);
                p.fill('rgba(0,0,0,0)');
                p.stroke(255);
                p.strokeWeight(2);
                p.ellipse(this.pos.x, this.pos.y, this.idRingSize, this.idRingSize);
                p.stroke(this.colour.replace('%A%', '1'));
                p.strokeWeight(1);
                p.ellipse(this.pos.x, this.pos.y, innerRingSize, innerRingSize);
                this.idRingSize = this.idRingSize + 1.5;
                if (this.idRingSize > 50) {
                    this.idRingSize = 0;
                    this.idRingPulseCount++;
                    if (this.idRingPulseCount >= this.idRingPulseMax) {
                        this.respawning = false;
                        this.idRingPulseCount = 0;
                    }
                }
            }
            // Draw trail
            p.strokeWeight(2);
            p.stroke(this.colour.replace('%A%', this.trailOpacity.toString()));
            // Create trail segment between bike and last trail end
            var headEnd = _.find(this.trail, function (t) { return t.isHead; }).end;
            var newSeg = new Model.TrailSegment(new Vector(headEnd.x, headEnd.y), new Vector(this.pos.x, this.pos.y));
            var trail = _.clone(this.trail);
            trail.push(newSeg);
            p.noFill();
            _.each(trail, function (tp) {
                // if (tp.isHead) {
                //     p.stroke('rgb(255,0,0)');
                // } else {
                //     p.stroke(this.colour.replace('%A%', this.trailOpacity.toString()));
                // }
                //p.ellipse(tp.start.x, tp.start.y, 3, 3);
                p.line(tp.start.x, tp.start.y, tp.end.x, tp.end.y);
            });
            // Draw bike
            var bikeColour = isControlledPlayer
                ? "rgb(255, 255, 255)"
                : this.colour.replace('%A%', '1');
            p.noStroke();
            p.fill(bikeColour);
            p.ellipse(this.pos.x, this.pos.y, 5, 5);
            // Draw crashing
            if (this.crashing) {
                // Explosion
                var randColour = NumberUtil.rand255();
                p.stroke('rgba(' + randColour + ', 0, 0, 0.80)');
                p.fill('rgba(' + randColour + ', ' + randColour + ' , 0, 0.75)');
                p.ellipse(this.pos.x, this.pos.y, 20, 20);
                var randSize = Math.floor(Math.random() * 35);
                randColour = NumberUtil.rand255();
                p.stroke('rgba(' + randColour + ', ' + randColour + ' , 0, 0.55)');
                p.fill('rgba(' + NumberUtil.rand255() + ', 0, 0, 0.65)');
                p.ellipse(this.pos.x, this.pos.y, randSize, randSize);
            }
        };
        Bike.prototype.getPos = function () {
            return this.pos;
        };
        Bike.prototype.setPos = function (pos) {
            this.pos = pos;
        };
        Bike.prototype.getDir = function () {
            return this.dir;
        };
        Bike.prototype.getSpd = function () {
            return this.spd;
        };
        Bike.prototype.setSpd = function (spd) {
            this.spd = spd;
        };
        Bike.prototype.getColour = function () {
            return this.colour;
        };
        Bike.prototype.isCrashing = function () {
            return this.crashing;
        };
        Bike.prototype.isRespawning = function () {
            return this.respawning;
        };
        return Bike;
    }());
    Model.Bike = Bike;
})(Model || (Model = {}));

var Model;
(function (Model) {
    var Player = /** @class */ (function () {
        function Player(pid, name, bike, crashed, spectating, deathTimestamp, crashedInto, crashedIntoName, score, isControlledPlayer) {
            this.pid = pid;
            this.name = name;
            this.bike = bike;
            this.crashed = crashed;
            this.spectating = spectating;
            this.deathTimestamp = deathTimestamp;
            this.crashedInto = crashedInto;
            this.crashedIntoName = crashedIntoName;
            this.score = score;
            this.isControlledPlayer = isControlledPlayer;
            if (this.isAlive) {
                this.bike.respawned();
            }
        }
        Player.prototype.getPid = function () {
            return this.pid;
        };
        Player.prototype.getName = function () {
            return this.name;
        };
        Player.prototype.setName = function (name) {
            this.name = name;
        };
        Player.prototype.getBike = function () {
            return this.bike;
        };
        Player.prototype.setBike = function (bike) {
            this.bike = bike;
        };
        Player.prototype.isCrashed = function () {
            return this.crashed;
        };
        Player.prototype.getCrashedInto = function () {
            return this.crashedInto;
        };
        Player.prototype.getCrashedIntoName = function () {
            return this.crashedIntoName;
        };
        Player.prototype.isAlive = function () {
            return !this.spectating && !this.crashed;
        };
        Player.prototype.isVisible = function () {
            return !this.spectating || this.bike.isCrashing();
        };
        Player.prototype.isSpectating = function () {
            return this.spectating;
        };
        Player.prototype.getCurrentPowerUp = function () {
            return this.currentPowerUp;
        };
        Player.prototype.getEffect = function () {
            return this.effect || "none";
        };
        Player.prototype.update = function () {
            this.bike.update(this.isAlive());
        };
        Player.prototype.draw = function (p, showName) {
            if (this.isVisible()) {
                var showRespawnRing = this.isAlive() && this.isControlledPlayer;
                this.bike.draw(p, showRespawnRing, this.isControlledPlayer);
                if (showName) {
                    p.textSize(15);
                    p.textAlign('center', 'middle');
                    p.text(this.name, this.bike.getPos().x, Math.max(0, this.bike.getPos().y - 15));
                }
                //p.text(this.effect ? this.effect : "none", this.bike.getPos().x, Math.max(0, this.bike.getPos().y - 15));
            }
        };
        Player.prototype.updateFromDto = function (dto) {
            var wasAlive = this.isAlive();
            if (!this.crashed && dto.crashed) {
                this.bike.crash();
                this.deathTimestamp = dto.deathTimestamp || Math.floor(Date.now());
            }
            if (!dto.crashed && (this.crashed || (this.spectating && !dto.spectating))) {
                this.bike.respawned();
            }
            var oldPowerUp = this.currentPowerUp;
            this.crashed = dto.crashed;
            this.crashedInto = dto.crashedInto;
            this.crashedIntoName = dto.crashedIntoName;
            this.spectating = dto.spectating;
            this.score = dto.score;
            this.currentPowerUp = dto.currentPowerUp ? dto.currentPowerUp.toLowerCase() : null;
            this.effect = dto.effect;
            this.bike.updateFromDto(dto.bike);
        };
        return Player;
    }());
    Model.Player = Player;
})(Model || (Model = {}));

var Model;
(function (Model) {
    var PowerUp = /** @class */ (function () {
        function PowerUp(_id, _pos, _type) {
            this.id = _id;
            this.pos = _pos;
            this.type = _type;
            this.collected = false;
            this.collecting = false;
            this.popSize = 0;
        }
        PowerUp.prototype.getId = function () {
            return this.id;
        };
        PowerUp.prototype.getPos = function () {
            return this.pos;
        };
        PowerUp.prototype.setPos = function (pos) {
            this.pos = pos;
        };
        PowerUp.prototype.getType = function () {
            return this.type;
        };
        PowerUp.prototype.setType = function (type) {
            this.type = type;
        };
        PowerUp.prototype.isCollected = function () {
            return this.collected;
        };
        PowerUp.prototype.collect = function () {
            this.collected = true;
            this.collecting = true;
        };
        PowerUp.prototype.updateFromDto = function (dto) {
            this.pos = dto.pos;
            if (!this.collected && dto.collected) {
                this.collect();
            }
        };
        PowerUp.prototype.draw = function (p) {
            if (!this.collected) {
                p.push();
                p.noStroke();
                p.translate(this.pos.x, this.pos.y);
                var size = 3;
                switch (this.type.toLowerCase()) {
                    case "rocket":
                        p.rotate(p.frameCount / -10.0);
                        p.fill('rgb(255,255,105)');
                        p.triangle(-size, size * 0.8, size, size * 0.8, 0, -size);
                        break;
                    case "slow":
                        p.rotate(p.frameCount / 50.0);
                        p.fill('rgb(105,255,255)');
                        p.triangle(-size, size, size, size, 0, -size);
                        p.rotate(p.PI);
                        p.triangle(-size, size, size, size, 0, -size);
                        break;
                    default:
                        break;
                }
                p.pop();
            }
            else if (this.collecting) {
                p.fill('rgba(0,0,0,0)');
                p.stroke(255);
                p.strokeWeight(2);
                p.ellipse(this.pos.x, this.pos.y, this.popSize, this.popSize);
                this.popSize = this.popSize + 1.5;
                if (this.popSize > 20) {
                    this.collecting = false;
                }
            }
        };
        return PowerUp;
    }());
    Model.PowerUp = PowerUp;
})(Model || (Model = {}));

var Model;
(function (Model) {
    var TrailSegment = /** @class */ (function () {
        function TrailSegment(start, end) {
            this.start = start;
            this.end = end;
        }
        TrailSegment.fromDto = function (dto) {
            var segment = new TrailSegment(dto.start, dto.end);
            segment.isHead = dto.isHead;
            return segment;
        };
        TrailSegment.prototype.draw = function (p) {
        };
        return TrailSegment;
    }());
    Model.TrailSegment = TrailSegment;
})(Model || (Model = {}));

var Game;
(function (Game_1) {
    var Bike = Model.Bike;
    var Player = Model.Player;
    var PowerUp = Model.PowerUp;
    var Arena = Model.Arena;
    var Vector = Util.Vector;
    var NumberUtil = Util.NumberUtil;
    var Game = /** @class */ (function () {
        function Game() {
            var _this = this;
            this.host = 'http://' + window.location.hostname + ':9092';
            this.socket = io.connect(this.host);
            this.players = [];
            this.powerUps = [];
            this.registered = false;
            this.version = "0.1b";
            this.gameStarted = false;
            this.gameJoined = false;
            this.showDebug = false;
            this.showRespawn = true;
            this.serverTimedOut = false;
            this.socket.on('hello', function (data) {
                _this.initGame(data);
            });
            this.socket.on('joined-game', function (data) {
                _this.joinGame(data);
            });
            setInterval(function () {
                _this.timeKeepAliveSent = new Date().getTime();
                _this.socket.emit('keep-alive');
            }, 1000);
            this.socket.on('keep-alive-ack', function (data) {
                var timeNow = new Date().getTime();
                _this.latency = timeNow - _this.timeKeepAliveSent;
                _this.refreshServerTimeout();
            });
            this.socket.on('world-update', function (data) {
                if (_this.gameStarted) {
                    _this.processWorldUpdate(data);
                }
            });
            this.socket.on('score-update', function (data) {
                _this.updateScores(data);
            });
            this.socket.on('chat-message', function (data) {
                // TODO: Use moment or something?
                var messageTime = new Date(data.timestamp).toTimeString().split(' ')[0];
                var chatElement = "<li>";
                chatElement += "[" + messageTime + "]";
                if (data.isSystemMessage) {
                    chatElement += "&nbsp;<span style='color:#AFEEEE'>" + _.escape(data.message) + "</span>";
                }
                else {
                    var colour = data.sourceColour.replace("%A%", "100");
                    chatElement += "&nbsp;<span style='color:" + colour + "'><strong>" + data.source + "</strong></span>:";
                    chatElement += "&nbsp;" + _.escape(data.message);
                }
                chatElement += "</li>";
                $('#chat-log ul').append(chatElement);
                $('#chat-log').scrollTop($('#chat-log')[0].scrollHeight);
                if ($('#message-list li').length > 250) {
                    $('#message-list li').first().remove();
                }
                _this.messageCount = $("#message-list li").length;
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
            $(document).on('keyup', function (ev) {
                var keyCode = ev.which;
                if (_this.player) {
                    if (keyCode === Keys.TAB) {
                        _this.tabPressed = false;
                    }
                }
            });
            $(document).on('keydown', function (ev) {
                if ($(ev.target).is('input')) {
                    // Typing in chat, don't process as game keys
                    if (ev.which === Keys.ENTER) { // enter
                        if ($(ev.target).is('#player-name-input')) { // enter when inside player name box
                            var name_1 = $('#player-name-input').val();
                            if (_this.nameIsValid(name_1)) {
                                _this.requestJoinGame(name_1);
                            }
                        }
                        else if ($(ev.target).is('#chat-input')) { // enter when inside chat box
                            var message = $('#chat-input').val();
                            if (message.trim() != "") {
                                _this.socket.emit('chat-message', message);
                                $('#chat-input').val('');
                            }
                        }
                        $(ev.target).blur();
                    }
                    return;
                }
                if (_this.player) {
                    var keyCode = ev.which;
                    var newVector = null;
                    var eventMatched = true;
                    if (keyCode === Keys.UP_ARROW || keyCode === Keys.W) {
                        newVector = new Vector(0, -1);
                    }
                    else if (keyCode === Keys.DOWN_ARROW || keyCode === Keys.S) {
                        newVector = new Vector(0, 1);
                    }
                    else if (keyCode === Keys.RIGHT_ARROW || keyCode === Keys.D) {
                        newVector = new Vector(1, 0);
                    }
                    else if (keyCode === Keys.LEFT_ARROW || keyCode === Keys.A) {
                        newVector = new Vector(-1, 0);
                    }
                    else if (keyCode === Keys.F3) {
                        _this.showDebug = !_this.showDebug;
                    }
                    else if (keyCode === Keys.R) {
                        _this.socket.emit('request-respawn');
                    }
                    else if (keyCode === Keys.H) {
                        _this.showRespawn = !_this.showRespawn;
                    }
                    else if (keyCode === Keys.TAB) {
                        _this.tabPressed = true;
                    }
                    else if (keyCode === Keys.SPACE) {
                        _this.socket.emit('use-powerup');
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
                        var updateDto = {
                            pid: _this.player.getPid(),
                            xDir: newVector.x,
                            yDir: newVector.y,
                            xPos: _this.player.getBike().getPos().x,
                            yPos: _this.player.getBike().getPos().y
                        };
                        _this.socket.emit('update', updateDto);
                    }
                }
            });
            $(document).ready(function () {
                $('#player-name-input').on('input', function () {
                    var name = $('#player-name-input').val();
                    if (_this.nameIsValid(name)) {
                        $('#player-name-submit').show();
                    }
                    else {
                        $('#player-name-submit').hide();
                    }
                });
                $('#player-name-submit').on('click', function () {
                    var name = $('#player-name-input').val();
                    _this.requestJoinGame(name);
                });
            });
            this.socket.emit('hello');
        }
        Game.prototype.setup = function (p) {
            this.mainFont = p.loadFont('fonts/3Dventure.ttf');
            this.secondaryFont = p.loadFont('fonts/visitor.ttf');
            this.debugFont = p.loadFont('fonts/larabie.ttf');
            this.powerUpIconRocket = p.loadImage('img/game/powerups/rocket.png');
            this.powerUpIconSlow = p.loadImage('img/game/powerups/slow.png');
            p.createCanvas(this.arena.size, this.arena.size);
        };
        Game.prototype.nameIsValid = function (name) {
            return name.trim().length > 1 && name.trim().length <= 15;
        };
        Game.prototype.refreshServerTimeout = function () {
            var _this = this;
            if (this.serverTimeoutTimer) {
                clearInterval(this.serverTimeoutTimer);
                this.serverTimeoutTimer = null;
            }
            this.serverTimedOut = false;
            this.serverTimeoutTimer = setInterval(function () { return _this.serverTimedOut = true; }, 5000);
        };
        Game.prototype.requestJoinGame = function (name) {
            var joinObj = {
                name: name
            };
            this.socket.emit('request-join-game', joinObj);
        };
        Game.prototype.joinGame = function (data) {
            $('#welcome-container').hide();
            $('#info-container').slideDown();
            this.gameJoined = true;
            this.player = new Player(data.player.pid, data.player.name, new Bike(data.player.bike), data.player.crashed, data.player.spectating, data.player.deathTimestamp, data.player.crashedInto, data.player.crashedIntoName, data.player.score, true);
            this.updateScores(data.scores);
        };
        Game.prototype.initGame = function (data) {
            var _this = this;
            if (!data.gameSettings.gameTickMs) {
                console.error("Cannot start game - game tick interval is not defined");
            }
            this.gameTick = data.world.gameTick;
            this.arena = new Arena(data.world.arena);
            this.gameTickMs = data.gameSettings.gameTickMs;
            this.processWorldUpdate(data.world);
            this.p5Instance = new p5(this.sketch(), 'game-container');
            this.gameStarted = true;
            $('#game').width(this.arena.size);
            $('#game').height(this.arena.size);
            // MAIN UPDATE LOOP
            setInterval(function () {
                _this.gameTick++;
                var baseSpeed = 1.5; // TODO: Get this from the server!
                if (_this.gameJoined) {
                    // Faster farther from center - disabled just now
                    //let spdModifier = this.calculateSpeedModifier(this.player);
                    //this.player.setSpd(baseSpeed + spdModifier)
                    _this.player.update();
                }
                _.each(_this.players, function (p) {
                    // Faster farther from center - disabled just now      
                    //let spdModifier = this.calculateSpeedModifier(b);                         
                    //b.setSpd(baseSpeed + spdModifier);
                    p.update();
                });
            }, this.gameTickMs);
        };
        Game.prototype.calculateSpeedModifier = function (b) {
            var gameSize = this.arena.size;
            var center = new Vector(gameSize / 2, gameSize / 2);
            var bikePos = new Vector(b.getPos().x, b.getPos().y), distance = Vector.distance(bikePos, center), oldMin = 0, oldMax = gameSize / 2, newMin = 0, newMax = 0.5, oldRange = oldMax - oldMin, newRange = newMax - newMin, spdModifier = ((distance - oldMin) * newRange / oldRange) + newMin; // Trust me
            return spdModifier;
        };
        Game.prototype.processWorldUpdate = function (data) {
            var _this = this;
            this.roundInProgress = data.roundInProgress;
            this.timeUntilNextRound = data.timeUntilNextRound;
            this.currentWinner = data.currentWinner;
            if (this.roundTimeLeft != data.roundTimeLeft) {
                var t = new Date(data.roundTimeLeft * 1000);
                var minutes = NumberUtil.pad(t.getMinutes(), 2);
                var seconds = NumberUtil.pad(t.getSeconds(), 2);
                $('#round-timer').text(minutes + ":" + seconds);
            }
            this.roundTimeLeft = data.roundTimeLeft;
            var updatedPlayers = _.pluck(data.players, 'pid');
            var existingPlayers = _.pluck(this.players, 'pid');
            _.each(existingPlayers, function (pid) {
                if (!_.contains(updatedPlayers, pid)) {
                    _this.players = _.reject(_this.players, function (p) { return p.getPid() === pid; });
                }
            });
            _.each(data.players, function (p) {
                if (_this.gameJoined && p.pid === _this.player.getPid() && _this.player) {
                    _this.player.updateFromDto(p);
                }
                else {
                    var existingPlayer = _.find(_this.players, function (ep) { return ep.getPid() === p.pid; });
                    if (existingPlayer) {
                        existingPlayer.updateFromDto(p);
                    }
                    else {
                        var player = new Player(p.pid, p.name, new Bike(p.bike), p.crashed, p.spectating, p.deathTimestamp, p.crashedInto, p.crashedIntoName, p.score, false);
                        _this.players.push(player);
                    }
                }
            });
            var powerUps = [];
            _.each(data.powerUps, function (p) {
                var powerUpDto = new PowerUp(p.id, p.pos, p.type);
                var existingPowerUp = _.find(_this.powerUps, function (ep) { return ep.getId() === p.id; });
                if (existingPowerUp) {
                    existingPowerUp.updateFromDto(p);
                }
                else {
                    var powerUp = new PowerUp(p.id, p.pos, p.type);
                    _this.powerUps.push(powerUp);
                    existingPowerUp = powerUp;
                }
                powerUps.push(existingPowerUp);
            });
            this.powerUps = powerUps;
            this.impacts = data.debug.impacts.map(function (x) { return x.pos; });
        };
        Game.prototype.updateScores = function (scores) {
            var _this = this;
            scores = _.sortBy(scores, function (x) { return x.score; }).reverse();
            var topFive = _.first(scores, 5);
            $('#score ul').empty();
            var playerInTopFive = false;
            topFive.forEach(function (score, i) {
                var isPlayer = _this.gameJoined && score.pid == _this.player.getPid();
                var player = _.first(_this.players.filter(function (p) { return p.getPid() == score.pid; }));
                playerInTopFive = isPlayer || playerInTopFive;
                var li = isPlayer ? "<li style='color:yellow'>" : "<li>";
                var position = "#" + (i + 1);
                var bikeColour = !player || isPlayer
                    ? "inherit"
                    : player.getBike().getColour().replace('%A%', '1');
                var scoreElement = li + position + " <span style='color:" + bikeColour + "'>" + score.name + "</span>: " + score.score + "</li>";
                $('#score ul').append(scoreElement);
            });
            if (this.gameJoined && !playerInTopFive) {
                var playerScore = scores.filter(function (x) { return x.pid == _this.player.getPid(); })[0];
                if (!playerScore) {
                    return;
                }
                var li = "<li style='color:yellow'>";
                var position = "#" + (scores.indexOf(playerScore) + 1);
                var scoreElement = li + position + " " + playerScore.name + ": " + playerScore.score + "</li>";
                $('#score ul').append(scoreElement);
            }
        };
        Game.prototype.getPowerUpIcon = function (powerUp) {
            var powerUpIcon = null;
            switch (powerUp) {
                case "rocket":
                    return this.powerUpIconRocket;
                case "slow":
                    return this.powerUpIconSlow;
                default:
                    return null;
            }
        };
        Game.prototype.sketch = function () {
            var _this = this;
            return function (p) {
                p.setup = function () { return _this.setup(p); };
                p.draw = function () { return _this.draw(p); };
            };
        };
        Game.prototype.draw = function (p) {
            var _this = this;
            var halfWidth = this.arena.size / 2;
            var halfHeight = this.arena.size / 2;
            this.arena.draw(p);
            if (this.roundInProgress) {
                _.each(this.powerUps, function (powerUp) {
                    powerUp.draw(p);
                });
                _.each(this.players, function (player) {
                    player.draw(p, _this.tabPressed);
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
                p.text("He's dead, Jim", halfWidth + NumberUtil.randInt(0, 2), halfHeight - 30 + NumberUtil.randInt(0, 2));
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
                    var powerUpIcon = this.getPowerUpIcon(this.player.getCurrentPowerUp());
                    if (powerUpIcon) {
                        var pos = this.player.getBike().getPos();
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
                        var suicide = this.player.getCrashedInto() == this.player.getPid();
                        var crashText = "";
                        if (suicide) {
                            crashText = "You killed yourself, idiot";
                        }
                        else {
                            crashText = "Killed by " + this.player.getCrashedIntoName();
                        }
                        p.fill('rgba(125,249,255,0.50)');
                        p.textSize(29);
                        p.text(crashText, halfWidth + NumberUtil.randInt(0, 2), halfHeight - 30 + NumberUtil.randInt(0, 2));
                        p.fill('rgba(255,255,255,0.80)');
                        p.textSize(28);
                        p.text(crashText, halfWidth, halfHeight - 30);
                    }
                    p.fill('rgba(125,249,255,0.50)');
                    p.textSize(33);
                    p.text("Press 'R' to respawn", halfWidth + NumberUtil.randInt(0, 2), halfHeight + NumberUtil.randInt(0, 2));
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
                var winner = this.gameJoined && this.currentWinner === this.player.getPid()
                    ? this.player
                    : _.find(this.players, function (player) { return player.getPid() === _this.currentWinner; });
                var winnerName = "The Wall";
                if (winner)
                    winnerName = winner.getName();
                p.noStroke();
                p.fill('rgba(0,0,0,0.4)');
                p.rect(0, halfHeight - 35, this.arena.size, 55);
                p.textFont(this.mainFont);
                p.textAlign('center', 'top');
                p.fill('rgba(125,249,255,0.50)');
                p.textSize(29);
                p.text(winnerName + " won!", halfWidth + NumberUtil.randInt(0, 2), halfHeight - 30 + NumberUtil.randInt(0, 2));
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
            _.each(this.impacts, function (i) {
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
                    var playerBike = this.player.getBike();
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
        };
        return Game;
    }());
    Game_1.Game = Game;
    new Game();
})(Game || (Game = {}));
