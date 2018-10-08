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
                    { displayname: "intel part 1", url: "http://linkmanager.tokyo.local/Handlers/LinkBIExternal.ashx?Method=ProcessSavedReport&ResponseType=JSON&IdReport=7c384b95-8d92-4147-bd3c-4949bcceb4c75062ca6e-d220-4dd4-b76c-c57044de6398&Username=kmorjan&Password=CC03E747A6AFBBCBF8BE7668ACFEBEE5", idx: 0, typ: "json" }
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
                };
            });
        };
    };
    bo.DataModel.prototype.startload = function () {
        var th, num, q;
        th = this;
        th.ds = null;
        th.ds = [];
        num = th.files[th.fidx].parts.length;
        if (num > 4) { num = 4; };
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
    bo.DataModel.prototype.checkfields = function () {
        var th, checkok;
        th = this;
        checkok = function (dat, k) {      
            _.delay(function () {
                th.checkfield(dat, k);
            }, 500);
        };

        if (th.files[th.fidx].multipart) {
            _.forEach(_.keys(th.ds[0]), function (k) {
                checkok(th.ds[0], k);
            });
        }
        alert("HERE");
        
    };
    bo.DataModel.prototype.checkfield = function (dat, k) {
        var isdate, isnum, isstring, dt, udat;
        isdate = true;
        isnum = true;
        isstring = true;
        udat = _.map(_.uniq(dat, function (d) { return d[k]; }), function (dd) { return dd[k]; });
        _.forEach(udat, function (v) {
            if (isstring) {
                isstring = _.isString(v);
            }
            if (isnum) {
                isnum = !isNaN(v);
            }
            if (isdate) {
               // dt = moment("" + v, "YYYY-MM-DD");
               // if (!(dt === null || dt === undefined)) {
               //     isdate = dt.isValid();
               // } else {
                    isdate = false;
               // }
            }
        });
        return [isstring, isnum, isdate, udat.length];
    };
})(bo, d3, _, queue);