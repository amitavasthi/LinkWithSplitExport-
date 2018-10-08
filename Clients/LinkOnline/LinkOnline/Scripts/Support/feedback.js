var username;
var userdetails;
function user(username) {userdetails = username;}
function users() {  return userdetails;}
(function ($) {
    var userdetails = [];
    var usernamep;
    var fname;
    var LastName;
    var email;
    var UserId;
    var url = window.location.href;
    var module = [];
    var pageinfo;
    var browsertype = "";
    var imagerequired = 'NO';
    var imgsrc;
    var count = 0;
    var count1 = 0;
    var m;
    // for finding browser details
    var nVer = navigator.appVersion;
    var nAgt = navigator.userAgent;
    var browserName = navigator.appName;
    var fullVersion = '' + parseFloat(navigator.appVersion);
    var majorVersion = parseInt(navigator.appVersion, 10);
    var nameOffset, verOffset, ix;
    var cookie = navigator.cookieEnabled;

    // In Opera, the true version is after "Opera" or after "Version"
    if ((verOffset = nAgt.indexOf("Opera")) != -1) {
        browserName = "Opera";
        fullVersion = nAgt.substring(verOffset + 6);
        if ((verOffset = nAgt.indexOf("Version")) != -1)
            fullVersion = nAgt.substring(verOffset + 8);
    }
        // In MSIE, the true version is after "MSIE" in userAgent
    else if ((verOffset = nAgt.indexOf("MSIE")) != -1) {
        browserName = "Microsoft Internet Explorer";
        fullVersion = nAgt.substring(verOffset + 5);
    }
        // In Chrome, the true version is after "Chrome" 
    else if ((verOffset = nAgt.indexOf("Chrome")) != -1) {
        browserName = "Chrome";
        fullVersion = nAgt.substring(verOffset + 7);
    }
        // In Safari, the true version is after "Safari" or after "Version" 
    else if ((verOffset = nAgt.indexOf("Safari")) != -1) {
        browserName = "Safari";
        fullVersion = nAgt.substring(verOffset + 7);
        if ((verOffset = nAgt.indexOf("Version")) != -1)
            fullVersion = nAgt.substring(verOffset + 8);
    }
        // In Firefox, the true version is after "Firefox" 
    else if ((verOffset = nAgt.indexOf("Firefox")) != -1) {
        browserName = "Firefox";
        fullVersion = nAgt.substring(verOffset + 8);
    }
        // In most other browsers, "name/version" is at the end of userAgent 
    else if ((nameOffset = nAgt.lastIndexOf(' ') + 1) < (verOffset = nAgt.lastIndexOf('/'))) {
        browserName = nAgt.substring(nameOffset, verOffset);
        fullVersion = nAgt.substring(verOffset + 1);
        if (browserName.toLowerCase() == browserName.toUpperCase()) {
            browserName = navigator.appName;
        }
    }
    // trim the fullVersion string at semicolon/space if present
    if ((ix = fullVersion.indexOf(";")) != -1)
        fullVersion = fullVersion.substring(0, ix);
    if ((ix = fullVersion.indexOf(" ")) != -1)
        fullVersion = fullVersion.substring(0, ix);

    majorVersion = parseInt('' + fullVersion, 10);
    if (isNaN(majorVersion)) {
        fullVersion = '' + parseFloat(navigator.appVersion);
        majorVersion = parseInt(navigator.appVersion, 10);
    }
    $.feedback = function (options) {

        var settings = $.extend({
            ajaxURL: '',
            postBrowserInfo: true,
            postHTML: true,
            postURL: true,
            proxy: undefined,
            letterRendering: false,
            initButtonText: '',
            strokeStyle: 'black',
            shadowColor: 'black',
            shadowOffsetX: 1,
            shadowOffsetY: 1,
            shadowBlur: 10,
            lineJoin: 'bevel',
            lineWidth: 3,
            html2canvasURL: '/Scripts/Support/html2canvas.js',
            feedbackButton: '.feedback-btn',
            showDescriptionModal: true,
            isDraggable: true,
            onScreenshotTaken: function () { },
            tpl: {
                highlighter: '<div class="dragclass" id="feedback-highlighter" style="height:520px;width:800px;"><div  id="hide" style="font-family: SegoeUILight;color: #666666"><p><b>Click and drag on the page to help us better understand your feedback. You can move this dialog if it\'s in the way.</b></p><button class="feedback-setblackout"><div  class="ico"></div><span>Highlight</span></button><label class="lower"><b>Highlight areas relevant to your feedback.</b></label><div  class="feedback-buttons"><button id="feedback-highlighter-next" class="feedback-next-btn feedback-btn-gray">Next</button><button id="feedback-highlighter-back" class="feedback-back-btn feedback-btn-gray">Back</button></div></div><div  id="show" class="modal-body"><div  class="cantain"><h4><b>information</b></h4><div  class="main-nav"><ul><li><b>name</b><ul><li><label id="main-name"></label></li></ul></li><li><b>username</b><ul><li><label id="main-uname"></label></li></ul></li> <li><b>email</b><ul><li><label id="main-email"></label></li></ul></li><li><b>module</b><ul><li><label id="module-name"></label></li></ul></li></ul></div><br/><br/><div  class="line1"></div><div class="description"><h4><b>description</b></h4><div  id="feedback-overvie"><textarea id="txtdiscriptiom" class="textarea" rows="8" disable="false" cols="30"name="s1" style="width:98.4%; overflow:hidden;"></textarea></div><div  id="imgblk" class="imgblkupld"><button id="del" class="dele"><img id="btnimage" src="/Images/Support/remove.PNG"/></button><p id="imgupldtxt">your screenshot is attached <img id="imgpin" src="/Images/Support/pin.PNG"/></p></div></div><div id="hide"><p id="error">* please enter description</p></div><div  class="line2"></div><br /><button id="summary" class="sum"><b>technical information</b> + </button><p id="browser-info">#</p></div<br /><div  class="btns-info"><input id="btnscreen"  type="button" value="take screnshot" /><br/><br/><input id="btnsentrequest" class="sendreq" type="button" value="send request" /></div></div></div><div  class="feedback-wizard-close"></div></div>',
                overview: '<div class="dragclass" id="feedback-overview"><div class="feedback-logo">Feedback</div><div id="feedback-overview-description"><div id="feedback-overview-description-text"><h3>Description</h3><h3 class="feedback-clientinfo">Client Details</h3><div id="feedback-additional-none"><span>None</span></div><div id="feedback-browser-info"><span id="spnbrser">Browser Type: </span></div><div id="feedback-page-info"><span id="pginfo">Page URL</span></div><div id="feedback-page-structure"><span></span></div></div></div><div id="feedback-overview-screenshot"><h3>Screenshot</h3></div><div class="feedback-buttons"><button id="feedback-submit" class="feedback-submit-btn feedback-btn-blue">Submit</button><button id="feedback-overview-back" class="feedback-back-btn feedback-btn-gray">Back</button></div><div id="feedback-overview-error">Please enter a description.</div><div class="feedback-wizard-close"></div></div>',
                description: '<div id="feedback-welcome"><div class="feedback-logo">Feedback</div><p>Feedback lets you send us suggestions about our products. We welcome problem reports, feature ideas and general comments.</p><p>Start by writing a brief description:</p><textarea id="feedback-note-tmp"></textarea><p>Next we\'ll let you identify areas of the page related to your description.</p><button id="feedback-welcome-next" class="feedback-next-btn feedback-btn-gray">Next</button><div id="feedback-welcome-error">Please enter a description.</div><div class="feedback-wizard-close"></div></div>',
                submitSucces: '<div id="feedback-submit-success"><div class="feedback-logo">Feedback</div><p>Sadly an error occured while sending your feedback. Please try againi</p><p>We cannot respond individually to every one, but we will use your comments as we strive to improve your experience.</p><button class="feedback-close-btn feedback-btn-blue">OK</button><div class="feedback-wizard-close"></div></div>',
               // submitError: '<div id="feedback-submit-error"><div class="feedback-logo">Feedback</div><p>Thank you for your feedback.We value every piece of feedback we receive.</p><button class="feedback-close-btn feedback-btn-blue">OK</button><div class="feedback-wizard-close"></div></div>'
            },
            onClose: function () { },
            screenshotStroke: true,
            highlightElement: true,
            initialBox: false
        }, options);

        var supportedBrowser = !!window.HTMLCanvasElement;
        var isFeedbackButtonNative = settings.feedbackButton == '.feedback-btn';
        var _html2canvas = false;
        if (supportedBrowser) {
            if (isFeedbackButtonNative) {               
            }
             
            $(document).ready(function () {

                $("#feedback-overview").hide();
                imagerequired = 'NO';
                count = 1;
                userdetails = users().split(',');
                usernamep = userdetails[0];
                fname = userdetails[1];
                LastName = userdetails[2];
                email = userdetails[3];
                module = url.split('/');
                var modules = module[4];

                if (isFeedbackButtonNative) {
                    $(this).hide();
                }
                if (!_html2canvas) {
                    $.getScript(settings.html2canvasURL, function () {
                        _html2canvas = true;
                    });
                }
                var canDraw = false,
                    img = '',
                    h = $(document).height(),
                    w = $(document).width(),
                    tpl = '<div id="feedback-module">';

                if (settings.initialBox) {
                    tpl += settings.tpl.description;
                }

                tpl += settings.tpl.highlighter + '<canvas id="feedback-canvas"></canvas><div id="feedback-helpers"></div><input id="feedback-note" name="feedback-note" type="hidden"></div>';

                $('body').append(tpl);

                moduleStyle = {
                    'position': 'absolute',
                    'left': '0px',
                    'top': '0px'
                };
                canvasAttr = {
                    'width': w,
                    'height': h
                };

                $('#feedback-module').css(moduleStyle);
                $('#feedback-canvas').attr(canvasAttr).css('z-index', '30000');

                if (!settings.initialBox) {
                    $('#feedback-highlighter-back').remove();
                    canDraw = true;
                    $('#feedback-canvas').css('cursor', 'crosshair');
                    $('#feedback-helpers').show();
                    $('#feedback-welcome').hide();

                    $('#feedback-highlighter').attr('id', "feedback-overview");


                    $('#feedback-overview').show();


                    $('#txtdiscriptiom').html("");

                    $('#imgblk').hide();
                    browsertype = 'Browser Name : ' + browserName + "<br/>" + 'Full version' + ' : ' + fullVersion + '<br/>' + 'Cookies Enabled :' + cookie + '<br/>' + 'UserAgent :' + nAgt;
                    $('#browser-info').html(browsertype);
                    $('#main-name').html(fname + " " + LastName);
                    $('#main-uname').html(usernamep);
                    $('#main-email').html(email);
                    $('#module-name').html(modules);
                    $('#hide').hide();
                    $('#feedback-canvas').hide();
                    $('#feedback-overview').addClass('feedback-draggable')
                }

                $('#btnscreen').click(function () {

                    //if ($('#txtdiscriptiom').val() == '') {
                    //    $("#txtdiscriptiom").css({
                    //        "border-color": "red"
                    //    });
                    //    $('#error').show();
                    //}
                    // else {

                    //var dragdiv = document.getElementsByClassName("dragclass");
                    //for (var i = 0; i < dragdiv.length; i++) {
                    //    new Draggable(dragdiv[i]);
                    //}

                         imagerequired = 'YES';
                        $('#feedback-overview').attr('id', "feedback-highlighter");
                        $('#feedback-canvas').show();
                        $('#show').hide(); $('#hide').show();
                        $("#feedback-highlighter").css("height", "200");
                        $("#feedback-highlighter").css("width", "500");
                        $('#feedback-highlighter').css({ top: '80%', left: '70%', margin: '-' + ($('#feedback-highlighter').height() / 2) + 'px 0 0 -' + ($('#feedback-highlighter').width() / 2) + 'px' });

                        imagerequired = 'YES';

                        count1 = 1;
                  // }
                });
                $('#del').click(function () {  
                  
                     imagerequired = 'NO'; $('#btnscreen').show();
                    $('#imgblk').hide();
                    canDraw = true;
                });
                $('#btnimage').click(function () {

                    imagerequired = 'NO';
                    $('#btnscreen').show();
                    $('#imgblk').hide();
                });

                $(document).on('click', '#txtdiscriptiom', function () {

                    $('#error').hide();
                    $("#txtdiscriptiom").css({
                        "width": "98.5%",
                        "resize": "none",
                        "border": "2px solid #6CAEE0"
                    });

                });

                var ctx = $('#feedback-canvas')[0].getContext('2d');
                ctx.fillStyle = 'rgba(102,102,102,0.25)';
                ctx.fillRect(0, 0, $('#feedback-canvas').width(), $('#feedback-canvas').height());
                rect = {};
                drag = false;
                highlight = 0,
                post = {};
                if (settings.postBrowserInfo) {
                    post.browser = {};
                    post.browser.appCodeName = navigator.appCodeName;
                    post.browser.appName = navigator.appName;
                    post.browser.appVersion = navigator.appVersion;
                    post.browser.cookieEnabled = navigator.cookieEnabled;
                    cookie = navigator.cookieEnabled;
                    post.browser.onLine = navigator.onLine;
                    post.browser.platform = navigator.platform;
                    post.browser.userAgent = navigator.userAgent;
                    post.browser.plugins = [];

                    $.each(navigator.plugins, function (i) {
                        post.browser.plugins.push(navigator.plugins[i].name);
                    });
                    $('#feedback-browser-info').show();
                }
                if (settings.postURL) {
                    post.url = document.URL;
                    $('#feedback-page-info').show();
                }

                if (settings.postHTML) {
                    post.html = $('html').html();
                    $('#feedback-page-structure').show();
                }

                if (!settings.postBrowserInfo && !settings.postURL && !settings.postHTML)
                    $('#feedback-additional-none').show();

                $(document).on('mousedown', '#feedback-canvas', function (e) {
                    if (canDraw) {

                        rect.startX = e.pageX - $(this).offset().left;
                        rect.startY = e.pageY - $(this).offset().top;
                        rect.w = 0;
                        rect.h = 0;
                        drag = true;
                    }
                });

                $(document).on('mouseup', function () {
                    if (canDraw) {
                        drag = false;

                        var dtop = rect.startY,
                            dleft = rect.startX,
                            dwidth = rect.w,
                            dheight = rect.h;
                        dtype = 'highlight';

                        if (dwidth == 0 || dheight == 0) return;

                        if (dwidth < 0) {
                            dleft += dwidth;
                            dwidth *= -1;
                        }
                        if (dheight < 0) {
                            dtop += dheight;
                            dheight *= -1;
                        }

                        if (dtop + dheight > $(document).height())
                            dheight = $(document).height() - dtop;
                        if (dleft + dwidth > $(document).width())
                            dwidth = $(document).width() - dleft;

                        if (highlight == 0)
                            dtype = 'blackout';

                        $('#feedback-helpers').append('<div class="feedback-helper" data-type="' + dtype + '" data-time="' + Date.now() + '" style="position:absolute;top:' + dtop + 'px;left:' + dleft + 'px;width:' + dwidth + 'px;height:' + dheight + 'px;z-index:30000;"></div>');

                        redraw(ctx);
                        rect.w = 0;
                    }
                });
                $(document).on('mousemove', function (e) {
                    if (canDraw && drag) {
                        $('#feedback-overview').css('cursor', 'default');
                        $('#feedback-highlighter').css('cursor', 'default');

                        rect.w = (e.pageX - $('#feedback-canvas').offset().left) - rect.startX;
                        rect.h = (e.pageY - $('#feedback-canvas').offset().top) - rect.startY;

                        ctx.clearRect(0, 0, $('#feedback-canvas').width(), $('#feedback-canvas').height());
                        ctx.fillStyle = 'rgba(102,102,102,0.25)';
                        ctx.fillRect(0, 0, $('#feedback-canvas').width(), $('#feedback-canvas').height());
                        $('.feedback-helper').each(function () {
                            if ($(this).attr('data-type') == 'highlight')
                                drawlines(ctx, parseInt($(this).css('left'), 10), parseInt($(this).css('top'), 10), $(this).width(), $(this).height());

                        });
                        if (highlight == 1) {                            
                        }

                        $('.feedback-helper').each(function () {
                            if ($(this).attr('data-type') == 'highlight')
                                ctx.clearRect(parseInt($(this).css('left'), 10), parseInt($(this).css('top'), 10), $(this).width(), $(this).height());
                        });
                        $('.feedback-helper').each(function () {
                            if ($(this).attr('data-type') == 'blackout') {
                                //drawlines(ctx, rect.startX, rect.startY, rect.w, rect.h);
                               // ctx.clearRect(parseInt($(this).css('left'), 10), parseInt($(this).css('top'), 10), $(this).width(), $(this).height());

                            }
                        });
                        if (highlight == 0) {
                            drawlines(ctx, rect.startX, rect.startY, rect.w, rect.h);
                        }
                    }
                });

                if (settings.highlightElement) {
                    var highlighted = [],
                        tmpHighlighted = [],
                        hidx = 0;

                    //$(document).on('mousemove hover', '#feedback-canvas,#feedback-highlighter', function (e) {

                    //    $('#feedback-canvas').css('background-color', 'rgba(102, 102, 102, 0.3)');

                    //});




                    $(document).on('mousemove click', '#feedback-canvas',  function (e) {
                        if (canDraw) {
                            redraw(ctx);
                            tmpHighlighted = [];

                              $('#feedback-canvas').css('cursor', 'crosshair');
                           // $('#feedback-canvas').css('cursor', 'default');
                              $('* :not(script,iframe,div,section,.feedback-btn,#feedback-module *)').each(function () {
                                if ($(this).attr('data-highlighted') === 'true')
                                    return;

                                if (e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width() && e.pageY > $(this).offset().top + parseInt($(this).css('padding-top'), 10) && e.pageY < $(this).offset().top + $(this).height() + parseInt($(this).css('padding-top'), 10)) {
                                    tmpHighlighted.push($(this));
                                }
                            });

                            var $toHighlight = tmpHighlighted[tmpHighlighted.length - 1];

                            if ($toHighlight && !drag) {

                                var _x = $toHighlight.offset().left - 2,
                                    _y = $toHighlight.offset().top - 2,
                                    _w = $toHighlight.width() + parseInt($toHighlight.css('padding-left'), 10) + parseInt($toHighlight.css('padding-right'), 10) + 6,
                                    _h = $toHighlight.height() + parseInt($toHighlight.css('padding-top'), 10) + parseInt($toHighlight.css('padding-bottom'), 10) + 6;

                                if (highlight == 1) {
                                    drawlines(ctx, _x, _y, _w, _h);
                                    ctx.clearRect(_x, _y, _w, _h);
                                    dtype = 'highlight';
                                }

                                $('.feedback-helper').each(function () {
                                    if ($(this).attr('data-type') == 'highlight')
                                        ctx.clearRect(parseInt($(this).css('left'), 10), parseInt($(this).css('top'), 10), $(this).width(), $(this).height());
                                });

                                if (highlight == 0) {
                                    drawlines(ctx, _x, _y, _w, _h);
                                    ctx.clearRect(_x, _y, _w, _h);
                                    dtype = 'blackout';
                                }

                               $('.feedback-helper').each(function () {
                                   if ($(this).attr('data-type') == 'blackout') {

                                       drawlines(ctx, parseInt($(this).css('left'), 10), parseInt($(this).css('top'), 10), $(this).width(), $(this).height());
                                       ctx.clearRect(parseInt($(this).css('left'), 10), parseInt($(this).css('top'), 10), $(this).width(), $(this).height());
                                   }
                                });

                                if (e.type == 'click' && e.pageX == rect.startX && e.pageY == rect.startY) {
                                    $('#feedback-helpers').append('<div class="feedback-helper" data-highlight-id="' + hidx + '" data-type="' + dtype + '" data-time="' + Date.now() + '" style="position:absolute;top:' + _y + 'px;left:' + _x + 'px;width:' + _w + 'px;height:' + _h + 'px;z-index:30000;"></div>');
                                    highlighted.push(hidx);
                                    ++hidx;
                                    redraw(ctx);
                                }
                            }
                        }
                    });
                }

                $(document).on('mouseleave', 'body,#feedback-canvas', function () {
                   // redraw(ctx);
                });

                $(document).on('mouseenter', '.feedback-helper', function () {
                    redraw(ctx);
                });



                $(document).on('mouseenter mouseleave', '.feedback-helper', function (e) {
                    if (drag)
                        return;

                    rect.w = 0;
                    rect.h = 0;

                    if (e.type === 'mouseenter') {
                        $(this).css('z-index', '30001');
                        $(this).append('<div class="feedback-helper-inner" style="width:' + ($(this).width() - 2) + 'px;height:' + ($(this).height() - 2) + 'px;position:absolute;margin:1px;"></div>');
                        $(this).append('<div id="feedback-close"></div>');
                        $(this).find('#feedback-close').css({
                            'top': -1 * ($(this).find('#feedback-close').height() / 2) + 'px',
                            'left': $(this).width() - ($(this).find('#feedback-close').width() / 2) + 'px'
                        });
                        if ($(this).attr('data-type') == 'blackout') {
                            /* redraw white */
                            ctx.clearRect(0, 0, $('#feedback-canvas').width(), $('#feedback-canvas').height());
                            ctx.fillStyle = 'rgba(102,102,102,0.25)';
                            ctx.fillRect(0, 0, $('#feedback-canvas').width(), $('#feedback-canvas').height());
                            $('.feedback-helper').each(function () {
                               if ($(this).attr('data-type') == 'highlight')
                                   drawlines(ctx, parseInt($(this).css('left'), 10), parseInt($(this).css('top'), 10), $(this).width(), $(this).height());

                            });
                            $('.feedback-helper').each(function () {
                                if ($(this).attr('data-type') == 'highlight')
                                    ctx.clearRect(parseInt($(this).css('left'), 10), parseInt($(this).css('top'), 10), $(this).width(), $(this).height());
                            });

                            ctx.clearRect(parseInt($(this).css('left'), 10), parseInt($(this).css('top'), 10), $(this).width(), $(this).height())
                            ctx.fillStyle = 'rgba(0,0,0,0.75)';
                            ctx.fillRect(parseInt($(this).css('left'), 10), parseInt($(this).css('top'), 10), $(this).width(), $(this).height());

                            ignore = $(this).attr('data-time');

                            /* redraw black */
                            $('.feedback-helper').each(function () {
                                if ($(this).attr('data-time') == ignore)
                                    return true;
                                if ($(this).attr('data-type') == 'blackout') {
                                    // ctx.fillStyle = 'rgba(0,0,0,1)';
                                    ctx.fillRect(parseInt($(this).css('left'), 10), parseInt($(this).css('top'), 10), $(this).width(), $(this).height())
                                }
                            });
                        }
                    }
                    else {
                        $(this).css('z-index', '30000');
                        $(this).children().remove();
                        if ($(this).attr('data-type') == 'blackout') {
                            redraw(ctx);
                        }
                    }
                });
                $("#browser-info").hide();
                var technicalinfo = 0;
                $("#browser-info").hide();
                $(document).on('click', '#summary', function () {

                    if (technicalinfo == 0) { $("#browser-info").show(); technicalinfo = 1; } else { $("#browser-info").hide(); technicalinfo = 0; }


                });
                $(document).on('click', '#feedback-close', function () {
                    if (settings.highlightElement && $(this).parent().attr('data-highlight-id'))
                        var _hidx = $(this).parent().attr('data-highlight-id');

                    $(this).parent().remove();

                    if (settings.highlightElement && _hidx)
                        $('[data-highlight-id="' + _hidx + '"]').removeAttr('data-highlighted').removeAttr('data-highlight-id');

                    redraw(ctx);
                });

                $('#feedback-module').on('click', '.feedback-wizard-close,.feedback-close-btn', function () {
                    close();
                });

                $(document).on('keyup', function (e) {
                    if (e.keyCode == 27)
                        close();
                });

                $(document).on('selectstart dragstart', document, function (e) {
                    e.preventDefault();
                });


                $(document).on('mousedown', '.feedback-sethighlight', function () {
                    highlight = 1;
                    $(this).addClass('feedback-active');
                    $('.feedback-setblackout').removeClass('feedback-active');
                });

                $(document).on('mousedown', '.feedback-setblackout', function () {
                    highlight = 0;
                    $(this).addClass('feedback-active');
                    $('.feedback-sethighlight').removeClass('feedback-active');
                });
                $(document).on('click', '#feedback-highlighter-next', function () {

                    if (count1 == 1) {
                        $('#feedback-overview').attr('id', "feedback-highlighter");
                        // for client data
                        $('#imgblk').show();
                        $('#btnscreen').hide();
                        imagerequired = 'YES';
                        $("#overviewtakescreenshot").hide(); //to hide screen shot button                       
                        canDraw = false;
                        $('#feedback-canvas').css('cursor', 'default');
                        var sy = $(document).scrollTop(),
                            dh = $(window).height();
                        $('#feedback-helpers').hide();
                        $('#feedback-highlighter').hide();
                        if (!settings.screenshotStroke) {
                            redraw(ctx, false);
                        }
                        html2canvas($('body'), {
                            onrendered: function (canvas) {
                                if (!settings.screenshotStroke) {
                                    redraw(ctx);
                                }
                                _canvas = $('<canvas id="feedback-canvas-tmp" width="' + w + '" height="' + dh + '"/>').hide().appendTo('body');
                                _ctx = _canvas.get(0).getContext('2d');
                                _ctx.drawImage(canvas, 0, sy, w, dh, 0, 0, w, dh);
                                imgsrc = img = _canvas.get(0).toDataURL();
                                $(document).scrollTop(sy);
                                post.img = img;
                                settings.onScreenshotTaken(post.img);
                                if (settings.showDescriptionModal) {
                                    $('#feedback-canvas-tmp').remove();
                                    $('#feedback-overview').show();
                                    $('#feedback-overview-description-text>textarea').remove();
                                    $('#feedback-overview-screenshot>img').remove();
                                    $('<textarea id="feedback-overview-note">' + $('#feedback-note').val() + '</textarea>').insertAfter('#feedback-overview-description-text h3:eq(0)');
                                    $('#feedback-overview-screenshot').append('<img class="feedback-screenshot" id="imge" src="' + img + '" />');
                                    $("#feedback-highlighter").css("height", "520");
                                    $("#feedback-highlighter").css("width", "850");
                                    $('#feedback-highlighter').css({ top: '50%', left: '50%', margin: '-' + ($('#feedback-highlighter').height() / 2) + 'px 0 0 -' + ($('#feedback-highlighter').width() / 2) + 'px' });
                                    var width = $("#feedback-highlighter").css("width");
                                    var half = width / 2;
                                    $("#feedback-highlighter").css("marginLeft", "-" + half + "px");

                                    $('#show').show();
                                    $('#hide').hide();
                                    $('#feedback-highlighter').show();
                                }
                                else {
                                    $('#feedback-module').remove();
                                    close();
                                    _canvas.remove();
                                }
                            },
                            proxy: settings.proxy,
                            letterRendering: settings.letterRendering
                        });
                    }
                    count1 = 2;
                });
                $('#error').hide();
                $(document).on('click', '#btnsentrequest', function () {

                    if ($('#txtdiscriptiom').val() == '') {
                        $("#txtdiscriptiom").css({
                            "border-color": "red"
                        });
                        $('#error').show();
                    }
                    else {
                        if (count == 1) {

                            $("#feedback-overview").hide();
                            $("#feedback-highlighter").hide();
                            canDraw = false;
                            if (imagerequired == 'YES') {

                                var Pic = document.getElementById("feedback-canvas").toDataURL("image/png");
                                Pic = Pic.replace(/^data:image\/(png|jpg);base64,/, "");
                                src = imgsrc;
                                src = src.replace(/^data:image\/(png|jpg);base64,/, "");                                

                            } else { src = "NO Image"; }
                           
                            var x = count;
                            var description = $('#txtdiscriptiom').val();
                            browsertype = "Browser Name =" + browserName + ',  Full version = ' + fullVersion + ',  Cookies Enabled =' + cookie + ",  User Agent =" + nAgt;
                            pageinfo = window.location.href;
                            $('#pginfo').html(pageinfo);
                            jQuery.ajax({
                                type: "POST",
                                url: "/Handlers/Support.ashx",
                                data: { 'description': description, 'pagedata': pageinfo, 'browserinfo': browsertype, 'fullvertion': fullVersion, 'useragent': nAgt, 'attachimage': imagerequired,'imageData':src },
                                success: function (data) {
                                    $("#feedback-highlighter").hide();
                                    $('#feedback-submit-success').show();
                                    $('#feedback-canvas').hide();
                                },
                                error: function (data) {
                                    count = 2;
                                }
                            });

                            if ($('#txtdiscriptiom').val().length > 0) {
                                $('#feedback-submit-success,#feedback-submit-success').remove();
                                $('#feedback-overview').hide();

                                post.img = img;
                                post.note = $('#txtdiscriptiom').val();
                                var data = { feedback: JSON.stringify(post) };
                                $.ajax({
                                    url: settings.ajaxURL,
                                    dataType: 'json',
                                    type: 'POST',
                                    data: data,
                                    success: function () {
                                        $('#feedback-module').append(settings.tpl.submitSuccess);
                                    },
                                    error: function () {
                                        // $('#feedback-module').append(settings.tpl.submitError);
                                        ShowMessage(LoadLanguageText("FeedbackMsg"), "Success");
                                        close();
                                    }
                                });
                            }
                            else {
                                $('#feedback-overview-error').show();
                            }
                            $("#feedback-overview").hide();
                            $('#btnscreen').show();
                            $('#imgblk').hide();
                        }
                        count = 2;
                    }

                });
                $('#txtdiscriptiom').html("");
            });
        }
        function close() {
            canDraw = false;
            $(document).off('mouseenter mouseleave', '.feedback-helper');
            $(document).off('mouseup keyup');
            $(document).off('mousedown', '.feedback-setblackout');
            $(document).off('mousedown', '.feedback-sethighlight');
            $(document).off('mousedown click', '#feedback-close');
            $(document).off('mousedown', '#feedback-canvas');
            $(document).off('click', '#feedback-highlighter-next');
            $(document).off('click', '#feedback-highlighter-back');
            $(document).off('click', '#feedback-welcome-next');
            $(document).off('click', '#feedback-overview-back');
            $(document).off('mouseleave', 'body');
            $(document).off('mouseenter', '.feedback-helper');
            $(document).off('selectstart dragstart', document);
            $('#feedback-module').off('click', '.feedback-wizard-close,.feedback-close-btn');
            $(document).off('click', '#btnsentrequest');
            if (settings.highlightElement) {
                $(document).off('click', '#feedback-canvas');
                $(document).off('mousemove', '#feedback-canvas');
            }
            $('[data-highlighted="true"]').removeAttr('data-highlight-id').removeAttr('data-highlighted');
            $('#feedback-module').remove();
            $('.feedback-btn').show();

            settings.onClose.call(this);
        }

        function redraw(ctx, border) {
            border = typeof border !== 'undefined' ? border : true;
            ctx.clearRect(0, 0, $('#feedback-canvas').width(), $('#feedback-canvas').height());
            ctx.fillStyle = 'rgba(102,102,102,0.25)';
            ctx.fillRect(0, 0, $('#feedback-canvas').width(), $('#feedback-canvas').height());
            $('.feedback-helper').each(function () {
                //if ($(this).attr('data-type') == 'highlight')
                    //if (border)
                    //    drawlines(ctx, parseInt($(this).css('left'), 10), parseInt($(this).css('top'), 10), $(this).width(), $(this).height());

            });
            $('.feedback-helper').each(function () {
                if ($(this).attr('data-type') == 'highlight')
                    ctx.clearRect(parseInt($(this).css('left'), 10), parseInt($(this).css('top'), 10), $(this).width(), $(this).height());
            });
            $('.feedback-helper').each(function () {
                if ($(this).attr('data-type') == 'blackout') {

                    ctx.fillRect(parseInt($(this).css('left'), 10), parseInt($(this).css('top'), 10), $(this).width(), $(this).height());
                }
            });
        }
        function drawlines(ctx, x, y, w, h) {
            ctx.strokeStyle = settings.strokeStyle;
            ctx.shadowColor = settings.shadowColor;
            ctx.shadowOffsetX = settings.shadowOffsetX;
            ctx.shadowOffsetY = settings.shadowOffsetY;
            ctx.shadowBlur = settings.shadowBlur;
            ctx.lineJoin = settings.lineJoin;
            ctx.lineWidth = settings.lineWidth;
            ctx.strokeRect(x, y, w, h);
            ctx.shadowOffsetX = 0;
            ctx.shadowOffsetY = 0;
            ctx.shadowBlur = 0;
            ctx.lineWidth = 1;
        }

    };

}(jQuery));

