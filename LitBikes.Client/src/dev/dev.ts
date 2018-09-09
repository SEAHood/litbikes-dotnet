import * as signalR from "@aspnet/signalr"
import * as _ from "underscore"
import * as $ from "jquery"
import * as chartJs from "chart.js"
import {
    WorldUpdateDtoShort, HelloDtoShort, GameJoinDtoShort, ScoreDtoShort, ChatMessageDtoShort
} from "../dto/shortDto";

export class Dev {
    private hubConnection = new signalR.HubConnectionBuilder().withUrl(`http://${window.location.hostname}:59833/hub`).build();
    private chart: Chart;

    private worldUpdates = 0;
    private scoreUpdates = 0;

    private trailSegmentCount = 0;

    constructor() {
        this.hubConnection.on("Hello", (data: HelloDtoShort) => {
            console.log("Got Hello");
        });

        this.hubConnection.on("JoinedGame", (data: GameJoinDtoShort) => {
            console.log("Got JoinedGame");
        });

        this.hubConnection.on("KeepAliveAck", data => {
            console.log("Got KeepAliveAck");
        });

        this.hubConnection.on("WorldUpdate", (data: WorldUpdateDtoShort) => {
            console.log("Got WorldUpdate");
            console.log(data);
            this.worldUpdates++;
            var trailSegments = 0;
            data.p.map(p => trailSegments += p.b.t.length);
            this.trailSegmentCount = trailSegments;
        });

        this.hubConnection.on("ScoreUpdate", (data: ScoreDtoShort[]) => {
            console.log("Got ScoreUpdate");
            this.scoreUpdates++;
        });

        this.hubConnection.on("ChatMessage", (data: ChatMessageDtoShort) => {
            console.log("Got ChatMessage");
        });
        
        $("document").ready(() => {
            var ctx = document.getElementById("worldUpdates") as HTMLCanvasElement;
            chartJs.Chart.defaults.global.defaultFontColor = "#fff";
            this.chart = new chartJs.Chart(ctx, {
                type: 'line',
                data: {
                    labels: [],
                    datasets: [
                        {
                            label: "World updates per second",
                            data: [] as number[],
                            backgroundColor: 'rgba(0, 0, 0, 0)',
                            borderColor: 'rgba(211, 84, 0, 255)',
                            borderWidth: 2,
                            pointRadius: 0,
                            pointHitRadius: 3
                        },
                        {
                            label: "Score updates per second",
                            data: [] as number[],
                            backgroundColor: 'rgba(0, 0, 0, 0)',
                            borderColor: 'rgba(26, 188, 156, 255)',
                            borderWidth: 2,
                            pointRadius: 0,
                            pointHitRadius: 3
                        },
                        {
                            label: "Trail segments",
                            data: [] as number[],
                            backgroundColor: 'rgba(0, 0, 0, 0)',
                            borderColor: 'rgba(241, 196, 15, 255)',
                            borderWidth: 2,
                            pointRadius: 0,
                            pointHitRadius: 3
                        }
                    ]
                },
                options: {
                    scales: {
                        yAxes: [{
                            ticks: {
                                suggestedMin: 0,
                                suggestedMax: 100
                            }
                        }],
                        xAxes: [{
                            display: false
                        }]
                    }
                } as Object
            });

            setInterval(() => {
                this.chart.data.labels.push(Date.now().toString());
                (this.chart.data.datasets[0].data as number[]).push(this.worldUpdates);
                (this.chart.data.datasets[1].data as number[]).push(this.scoreUpdates);
                (this.chart.data.datasets[2].data as number[]).push(this.trailSegmentCount);
                this.chart.update();

                this.worldUpdates = 0;
                this.scoreUpdates = 0;
            }, 1000);
        });
    }

    go() {
        this.hubConnection.start().then(() => {
            console.log("Connected to hub");
            //this.hubConnection.invoke("AttachDev");
        }).catch(err => console.error(err.toString()));
    }
}

var devStats = new Dev();
devStats.go();