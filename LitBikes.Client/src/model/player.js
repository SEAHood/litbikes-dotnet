"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
class Player {
    constructor(pid, name, bike, crashed, spectating, deathTimestamp, crashedInto, crashedIntoName, score, isControlledPlayer) {
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
    getPid() {
        return this.pid;
    }
    getName() {
        return this.name;
    }
    setName(name) {
        this.name = name;
    }
    getBike() {
        return this.bike;
    }
    setBike(bike) {
        this.bike = bike;
    }
    isCrashed() {
        return this.crashed;
    }
    getCrashedInto() {
        return this.crashedInto;
    }
    getCrashedIntoName() {
        return this.crashedIntoName;
    }
    isAlive() {
        return !this.spectating && !this.crashed;
    }
    isVisible() {
        return !this.spectating || this.bike.isCrashing();
    }
    isSpectating() {
        return this.spectating;
    }
    getCurrentPowerUp() {
        return this.currentPowerUp;
    }
    getEffect() {
        return this.effect || "none";
    }
    update() {
        this.bike.update(this.isAlive());
    }
    draw(p, showName) {
        if (this.isVisible()) {
            let showRespawnRing = this.isAlive() && this.isControlledPlayer;
            this.bike.draw(p, showRespawnRing, this.isControlledPlayer);
            if (showName) {
                p.textSize(15);
                p.textAlign('center', 'middle');
                p.text(this.name, this.bike.getPos().x, Math.max(0, this.bike.getPos().y - 15));
            }
            //p.text(this.effect ? this.effect : "none", this.bike.getPos().x, Math.max(0, this.bike.getPos().y - 15));
        }
    }
    updateFromDto(dto) {
        let wasAlive = this.isAlive();
        if (!this.crashed && dto.crashed) {
            this.bike.crash();
            this.deathTimestamp = dto.deathTimestamp || Math.floor(Date.now());
        }
        if (!dto.crashed && (this.crashed || (this.spectating && !dto.spectating))) {
            this.bike.respawned();
        }
        let oldPowerUp = this.currentPowerUp;
        this.crashed = dto.crashed;
        this.crashedInto = dto.crashedInto;
        this.crashedIntoName = dto.crashedIntoName;
        this.spectating = dto.spectating;
        this.score = dto.score;
        this.currentPowerUp = dto.currentPowerUp ? dto.currentPowerUp.toLowerCase() : null;
        this.effect = dto.effect;
        this.bike.updateFromDto(dto.bike);
    }
}
exports.Player = Player;
//# sourceMappingURL=player.js.map