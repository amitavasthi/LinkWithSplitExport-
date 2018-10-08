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
        dispatcher.addEventListener('loadcomplete', function (event) {

            // alert("READY");
            //els = $(target.filecontent.node()).select("div").velocity("transition.slideUpOut", { stagger: 250, drag:true });
            // target.filecontent.transition().duration(600).style({ "opacity": 0 });
           // target.hideprogress(0);
           // target.filecontent.transition().duration(600).style({ "height": "0px", "opacity": 0 }).each("end", function () {
               // target.setmsg("CHECK THE FIELDS");
               // target.msgcontent.transition().duration(600).style({ "opacity": 1 });
             // });
          
            target.setmsg("CHECK THE FIELDS");
           // datamodel.checkfields();
           // dispatcher.dispatch('changeoverlay', { typ: 'hide', duration: 500 });
           // target.hide(500);
           
        });
    };
    bo.FileLoadMediator = FileLoadMediator;
})(bo, d3);