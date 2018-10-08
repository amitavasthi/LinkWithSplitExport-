(function (bo, d3, _, queue) {
    'use strict';
    bo.DataModel = function (dispatcher) {
        var th;
        th = this;
        this.dispatcher = dispatcher;
        this.fidx = -1;
        this.ds = null;
        this.files = [
            {
                displayname: "intel data", multipart:true, parts: [
                    { displayname: "intel part 1", url: "../../data/inteldata2-001.csv", idx: 0, typ: "csv" },
                    { displayname: "intel part 2", url: "../../data/inteldata2-002.csv", idx: 1, typ: "csv" }
                ]
            }
        ];
        this.loaddata = function (o, cb) {
            d3[o.typ](o.url, function (d) {
                o.dat = d;
                cb(null, "");
            }).on("progress", function () {
                if (d3.event.lengthComputable) {
                    var pct = Math.round(d3.event.loaded * 100 / d3.event.total);
                    th.dispatcher.dispatch("dataprogress", { pct: pct, id: o.idx });
                }
            });
        };
    };
    bo.DataModel.prototype.startload = function () {
        var th, num, q;
        th = this;
        th.ds = null;
        th.ds = [];
        num = th.files[th.fidx].parts.length;
        if (num > 4) { num = 4; }
        q = queue(num);
        _.forEach(th.files[th.fidx].parts, function (d, i) {
            d.dat = [];
            q.defer(th.loaddata, d);
        });
        q.awaitAll(function () {
            _.forEach(th.files[th.fidx].parts, function (da) {
                if (da.typ === "csv" && th.files[th.fidx].multipart) {
                    th.ds = th.ds.concat(da.dat);
                    da.dat = null;
                }
            });
            th.files[th.fidx].loaded = true;
            th.dispatcher.dispatch("loadcomplete");
        });
    };
})(bo, d3, _, queue);