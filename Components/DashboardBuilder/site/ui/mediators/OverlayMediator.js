(function (bo, d3) {
    'use strict';
    var OverlayMediator = function (target, dispatcher) {
        dispatcher.addEventListener('changeoverlay', function (event) {
            switch (event.params.typ) {
                case "show":
                    target.showolay(event.params.opacity, event.params.duration, function () {
                        if (event.params.cb) {
                            event.params.cb();
                        }
                    });
                    break;
                case "hide":
                    target.hideolay(event.params.duration, function () {
                        if (event.params.cb) {
                            event.params.cb();
                        }
                    });
                    break;         
                default:
            }
        });
    };
    bo.OverlayMediator = OverlayMediator;
})(bo, d3);