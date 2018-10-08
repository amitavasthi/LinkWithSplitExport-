(function (bo) {
    'use strict';
    var DlgMediator = function (target, dispatcher, mediators, dlgs) {
        var currdlg;
        dispatcher.addEventListener('changedlg', function (event) {        
            dispatcher.dispatch('changeoverlay', { typ: 'show', duration: event.params.duration, opacity: event.params.opacity, cb:event.params.cb });
            target.showdlg(event.params.duration);
            if (currdlg) {
                currdlg.shutdown();
                currdlg = null;
            }
            if (event.params.id) {
                currdlg = mediators.create(dlgs[event.params.id], target.element);
                currdlg.startup();
            }
           
        });
        dispatcher.addEventListener('loadcomplete', function (event) {
            target.hidedlg(500);
            dispatcher.dispatch('changeoverlay', { typ: 'hide', duration: 500 });
        });
    };
    bo.DlgMediator = DlgMediator;
})(bo);