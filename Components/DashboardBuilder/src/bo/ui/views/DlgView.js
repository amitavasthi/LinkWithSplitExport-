(function (bo, d3) {
    'use strict';
    var DlgView = function (target, dispatcher) {
        this.element = target;
        this.el = d3.select(target);
    };
    DlgView.prototype.showdlg = function (dur, cb) {
        this.el.style({ "display": "", "opacity": 0 });
        this.el.transition().duration(dur || 900).style("opacity", 1).each("end", function () {
            if (cb) {
                cb();
            }
        });
    };
    DlgView.prototype.hidedlg = function (dur, cb) {
        var th;
        th = this;
        this.el.transition().duration(dur || 900).style("opacity", 0).each("end", function () {
            th.el.style("display", "none");
            if (cb) {
                cb();
            }
        });
    };
    bo.DlgView = DlgView;
})(bo, d3);