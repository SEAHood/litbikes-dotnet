"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const express = require("express");
const path = require("path");
var app = express();
app.use(express.static(path.join(__dirname, 'public')));
app.get('/', function (req, res) {
    res.sendFile(path.join(__dirname + '/index.html'));
});
app.listen(process.env.PORT);
//# sourceMappingURL=app.js.map