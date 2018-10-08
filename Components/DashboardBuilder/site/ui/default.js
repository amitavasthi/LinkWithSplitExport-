$(document).ready(function () {
    'use-strict';
    var AppStyleCommand = function (appthemes) {
        this.execute = function (event) {
            var mdl = appthemes[event.params];
            //  appmodel.theme = event.params;
            vein.inject('body', mdl.bg);
            vein.inject(['.olay'], mdl.olay);
            vein.inject(['.dlgolay'], mdl.dlg);
            vein.inject('.fileloadview', mdl.fileloadview);
        };
    };;
    var Dashboard = soma.Application.extend({
        init: function () {
            var th, vwolay, vwdlg, vwprg, appstyles;
            th = this;
             appstyles = {
                "light": new bo.AppLight(),
                "dark": new bo.AppDark()
             };

            this.injector.mapValue("appthemes", appstyles);
            this.commands.add("changetheme", AppStyleCommand);

            this.injector.mapClass("datamodel", bo.DataModel, true);

            //vein.inject(['.olay'], { "background-color": "#333333" });

            vwolay = this.mediators.create(bo.OverlayView, document.querySelector('.olay'));
            mediators = this.mediators.create(bo.OverlayMediator, vwolay);

            vwdlg = this.mediators.create(bo.DlgView, document.querySelector('.dlg'));
            mediators = this.mediators.create(bo.DlgMediator, vwdlg);

            vwprg = this.mediators.create(bo.FileLoadView, document.querySelector('.dlg'));
            mediators = this.mediators.create(bo.FileLoadMediator, vwprg);

        },
        start: function () {
            var th;
            th = this;
            this.dispatcher.dispatch("changetheme", "light");
            th.dispatcher.dispatch("changefileload", { typ: "setfiles", fileidx: 0 });
            // this.dispatcher.dispatch("changeoverlay", { typ: "show", duration: 1000, opacity: "1" });

            this.dispatcher.dispatch("changedlg", {
                duration: 800, opacity: 0.4, cb: function () {
                    th.dispatcher.dispatch("changefileload", { typ: "startload" });
                }
            });
        }
    });
    app = new Dashboard();
});