(function (bo, d3) {
    'use strict';  
    var OverlayView = function (target, dispatcher) {
        this.element = d3.select(target);
    };
    OverlayView.prototype.showolay = function (op, dur, cb) {
        this.element.style({ "display": "", "opacity": 0 });
        this.element.transition().duration(dur || 900).style("opacity", op || 1).each("end", function () {
            if (cb) {
                cb();
            }      
        });
    };
    OverlayView.prototype.hideolay = function (dur, cb) {
        var th;
        th = this;
        this.element.transition().duration(dur || 900).style("opacity", 0).each("end", function () {
            th.element.style("display", "none");
            if (cb) {
                cb();
            }
        });
    };
    bo.OverlayView = OverlayView;
})(bo, d3);