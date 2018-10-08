(function (bo, d3, $) {
    'use strict';
    var FileLoadView = function (target, dispatcher) {
        this.element = d3.select(target);
        this.dispatcher = dispatcher;
        this.outer = this.element.append("div").attr("class", "ds-1 fileloadview").style({ width: "280px", margin: "40px auto", "padding": "5px" });
        this.filecontent = this.outer.append("div").attr("class", "dlgfilecontent");
        this.msgcontent = this.outer.append("div").attr("class", "dlgmsgcontent txtnorm txtbold");
        this.dat = [];
        this.prgcol = "";
        this.size = "small";
        this.datatype = "json";
        this.fontcss = "txtlight txtnorm";
        this.cbcomplete = null;
    };
    FileLoadView.prototype.setdata = function (da) {
        var th, div, pdiv, tdiv, bar, lbl;
        th = this;
        th.dat = da;
        th.filecontent.selectAll("div").remove();
        _.forEach(th.dat, function (d, i) {
            tdiv = th.filecontent.append("div").attr("class", th.fontcss);//.style("text-align","center");
            tdiv.text(d.displayname);

            pdiv = th.filecontent.append("div");
            if (th.size === "small") {
                pdiv.attr("id", "_lprg" + d.idx).attr("class", "ui tiny" + th.prgcol + " progress").style({ "height": "4px", "margin-top": "4px", "margin-bottom": "10px", "border-radius": 0 });
                bar = pdiv.append("div").attr("class", "bar").style("height", "4px");
                $(pdiv.node()).progress({
                    percent: 0
                });
            } else {
                pdiv.attr("id", "_lprg" + d.idx).attr("class", "ui " + th.prgcol + " progress").style({ "margin-top": "4px", "margin-bottom": "10px", "border-radius": 0 });
                bar = pdiv.append("div").attr("class", "bar").append("div").attr("class", "progress txtlight").style("background-color", "transparent");
                lbl = pdiv.append("div").attr("class", "label");
                $(pdiv.node()).progress({
                    percent: 0
                });
            }
        });
    };
    FileLoadView.prototype.setmsg = function (msg) {
        this.msgcontent.text(msg);
    };
    FileLoadView.prototype.setprogress = function (id, pct) {
        $("#_lprg" + id).progress({
            percent: pct
        });
    };
    FileLoadView.prototype.show = function (dur) {
        this.outer.style({ "display": "", "opacity": 0 });
        this.outer.transition().duration(dur).style("opacity", 1);
    };
    FileLoadView.prototype.hide = function (dur) {
        var th;
        th = this;
        this.outer.transition().duration(dur).style("opacity", 0).each("end", function () {
            th.outer.style("display", "none");
        });
    };
    FileLoadView.prototype.hideprogress = function (dur) {
        var th;
        th = this;
        th.filecontent.transition().duration(dur).style("opacity", 0);
    };
    bo.FileLoadView = FileLoadView;
})(bo, d3, $)