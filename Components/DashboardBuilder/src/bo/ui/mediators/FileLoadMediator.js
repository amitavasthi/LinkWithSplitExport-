(function (bo, d3) {
    'use strict';
    var FileLoadMediator = function (target, dispatcher, datamodel) {
        dispatcher.addEventListener('dataprogress', function (event) {
            target.setprogress(event.params.id, event.params.pct);
        });
        dispatcher.addEventListener('changefileload', function (event) {
            switch (event.params.typ) {
                case "setfiles":
                   // alert("OK"+JSON.stringify(datamodel));
                    datamodel.fidx = event.params.fileidx;
                    target.setdata(datamodel.files[event.params.fileidx].parts);

                    //target.show(event.params.duration);
                    break;
                case "startload":
                    datamodel.startload();
                    //alert("OK"+JSON.stringify(filemodel));
                    //target.setdata(filemodel.files[event.params.fileidx].parts);
                    //target.show(event.params.duration);
                    break;
                case "show":
                    target.show(event.params.duration);
                    break;
                case "hide":
                    target.hide(event.params.duration);
                    break;
                default:
            }
        });
    };
    bo.FileLoadMediator = FileLoadMediator;
})(bo, d3);