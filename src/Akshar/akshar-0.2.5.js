    /* akshar-0.2.5 Copyright (C) 2010-2011 Varun Khosla, all rights reserved. No part of this software may be reproduced, copied or disseminated without the written permission of the author. */

$(function() {
    var theMenu = _("aksMenubar"), mfPop = _("mfPop"), moPop = _("moPop"), mlPop = _("mlPop"), mdPop = _("mdPop"),
mimg = _("marked"), akshFrame = _("aksharContainer"), accView = true, doctitle = _('doctitle'), isInDialog = false, statusPane = _("statusPane"), tfFor = _('tfFor'), tfBack=_("tfBack");
opopup = null;
    // Firefox seems to be messing up with iframe.document! Not remembering the previous reference, so always take the latest!
    function akshRoot() { return akshFrame.contentWindow.document; }

var navKeys = {ESCAPE: 27, LEFT: 37, UP: 38, RIGHT: 39, DOWN: 40 };
$('#aksMenubar').bind("keydown", function(e) {
if (e.which == navKeys.ESCAPE) {
if (opopup) {
menuVisible(opopup, false);
opopup.mainMenu.focus();
opopup = null;
} else
akshRoot().body.focus();
return;
}


            if (opopup == null)
                return;

            switch (e.which) {
                            case navKeys.LEFT:
                    $(opopup).prevAll(".submenu").each(function() {
                        if (!isDisabled(this.mainMenu)) {
                            exchange(opopup, this);
                            return false;
                        }
                    });
                    break;
                case navKeys.UP:
                    $(document.activeElement).parent(".menuitem").prevAll().each(function() {
                        var i1 = $("a:not([disabled])", this);
                        if (i1.length > 0) {
                            i1[0].focus();
                            return false;
                        }
                    });
                    break;
                case navKeys.RIGHT:
                    $(opopup).nextAll(".submenu").each(function() {
                        if (!isDisabled(this.mainMenu)) {
                            exchange(opopup, this);
                            return false;
                        }
                    });
                    break;
                case navKeys.DOWN:
                    $(document.activeElement).parent(".menuitem").nextAll().each(function() {
                        var i1 = $("a:not([disabled])", this);
                        if (i1.length > 0) {
                            i1[0].focus();
                            return false;
                        }
                    });
                    break;
            }


            function exchange(src, dest) {
                menuVisible(src, false);
                opopup = dest;
                menuVisible(opopup, true);
            }
        });


    function menuShowHide(e) {
        // var thePopup = $(theMenu.rows[1].cells[this.parentNode.cellIndex]).children("div")[0];
        if (isDisabled(this))
            return;
                    var thePopup = _($(this).attr("initial") + "Pop");
            // alert(thePopup.offsetLeft);
            if (isNull(thePopup)) return;
            switch (e.type.toLowerCase()) {
                case "mouseover":
                    if (accView) return;
                    // Open the popup menu if another is opened.
                    if (!isNull(opopup) && opopup !== thePopup) {
                        // Hide the previously opened popup
                        menuVisible(opopup, false);
                        opopup = thePopup;
                        menuVisible(opopup, true);
                    }
                    break;
                case "click":
                    if (opopup === thePopup) {
                        menuVisible(opopup, false);
                        opopup = null;
                    } else {
                        if (!isNull(opopup)) menuVisible(opopup, false);
                        opopup = thePopup;
                        menuVisible(opopup, true);
                    }
                    return false;
            } }

        function menuVisible(o, visible) {
            if (visible) {
                $(o).trigger("opening");
                o.style.visibility = "visible";
                var i1 = $("a:first", o);
                if (i1.length) {
                    if (!isDisabled(i1[0]))
                        i1[0].focus();
                    else
                        o.focus();
                }
            } else {
            o.style.visibility = "hidden";
            }
        }

        function markedMenu(parent, item) {
            var old = $("img[id='marked']", parent);
            if (old.length) {
old.parent().attr("checked", false); old.remove();
}
            $(item).prepend($(mimg).clone().css("display", "inline"));
$(item).attr("checked", true);
        }

        function unmarkedMenu(item) {
        var old = $("img[id='marked']", item);
        if (old.length) {
            old.parent().attr("checked", false); old.remove();
        }
        }

        function isMarked(item) {
            if ($(item).attr("checked") == "true") return true;
            else return false;
        }

/* mdi */
var DocStates = {created:1,dirty:2,unchanged:3},
EventTypes = { closing: "closing", saved: "saved", errorSaving: "errSaving" };
    (function() {

        function akshDoc(name, text, location) {
            this.name = name;
            if (typeof text === "undefined") {
                this.text = "";
                this.state = DocStates.created;
            } else { // Initialize existing document
                this.state = DocStates.unchanged; this.location = location;
                var i = text.indexOf("</aks>");
                if (i > 0) {
                    var header = text.substring(0, i + 6);
                    text = text.substring(i + 6);
                    try {
                    var xHed = parseXml(header);
                    if (typeof xHed === "object") {
                        var rvkl = xHed.getElementsByTagName("rvkl")[0];
                        if (rvkl) {
                            var vkl = VKM.VKLExists(rvkl.getAttribute("name"), rvkl.getAttribute("type"));
                            if (vkl == null) {
                                this.rVkl = VKM.createVKL(rvkl.getAttribute("name"), rvkl.getAttribute("type"));
                                addVklMenuItem(this.rVkl);
                            } else
                                this.rVkl = vkl;
                        }
                    }
                } catch (e) { }
                }
                this.text = text;
            }
        }

        akshDoc.prototype = {
            name: null, text: null, isCurrent: false, location: null, state: null,
            rVkl: null,
            hClosed: [], hSaved: [], hErrSaving: [],

            canClose: function() {
                // Get the latest text.
                if (this.isCurrent) akshDocs.setCurrent(this);

                switch (this.state) {
                    case DocStates.unchanged:
                        return true;
                    case DocStates.created:
                        if (String.isNullOrEmpty(this.text))
                            return true;
                        else return false;
                    default:
                        return false;
                }
            },

            getText: function() { return this.text; },
            setText: function(text) {
                // New document is always dirty.
                if (this.state === DocStates.unchanged && this.text != text)
                    this.state = DocStates.dirty;
                this.text = text;
            },

            getVkl: function() { return this.rVkl; },
            setVkl: function(vkl) {
                this.rVkl = vkl;
                if (!this.isCurrent)
                    return;

                if (vkl == null)
                    VKM.suspendControl(akshFrame)
                else {
                    VKM.rebindControl(akshFrame, vkl, doKeyInput);
                }
            },

            focusChanged: function(isCurrent) {
                this.isCurrent = isCurrent;
                if (isCurrent) {
                    this.setVkl(this.rVkl);
                }
            },

            save: function(path, closeAfter) {
                var header = '', self = this;
                if (this.rVkl != null)
                    header = "<aks><rvkl name=\"" + this.rVkl.name + "\" type=\"" + this.rVkl.type + "\"></rvkl></aks>";

                if (this.isCurrent)
                    akshDocs.setCurrent(this);

                path = (typeof path === "undefined" || String.isNullOrEmpty(path) ? this.location : path);
                AksStatus.setInfo("Saving " + this.name, true);
                $.ajax({ type: "POST", url: "akshar.aspx/SaveDoc",
                    data: "{'path':'" + path + "','text':'" + escape(header + self.text) + "'}",
                    contentType: "application/json; charset=utf-8", dataType: "json",
                    success: function(msg) {
                        if (msg.hasOwnProperty("d")) msg = msg.d;
                        var response;
                        eval("response = " + msg);
                        if (response.result == "success") {
                            self.name = path.substring(path.lastIndexOf("/") + 1);
                            self.location = path;
                            self.state = DocStates.unchanged;
                            AksStatus.setInfo(self.name + " saved");
                            self._raiseEvent(EventTypes.saved, self);
                            if (closeAfter)
                                self.close();
                        } else {
                            AksStatus.setInfo("");
                            self._raiseEvent(EventTypes.errorSaving);
                            alert("The document couldn't be saved: " + response.msg);
                        }
                    },
                    error: function(XMLHttpRequest, textStatus, errorThrown) {
                    AksStatus.setInfo("");
                        self._raiseEvent(EventTypes.errorSaving);
                        alert("The document couldn't be saved: an unknown, communication-specific error occurred.");
                    }
                })
            },

            close: function() {
                this._raiseEvent(EventTypes.closing, this);
                this._destroy();
            },

            addListener: function(type, fn) {
                if (typeof fn !== "function") return;
                switch (type) {
                    case EventTypes.closing: addHandler(this.hClosed, fn); break;
                    case EventTypes.saved: addHandler(this.hSaved, fn); break;
                    case EventTypes.errorSaving: addHandler(this.hErrSaving, fn); break;
                }
            },

            removeListener: function(type, fn) {
                if (typeof fn !== "function") return;
                switch (type) {
                    case EventTypes.closing: removeHandler(this.hClosed, fn); break;
                    case EventTypes.saved: removeHandler(this.hSaved, fn); break;
                    case EventTypes.errorSaving: removeHandler(this.hErrSaving, fn); break;
                }
            },

            // internal
            _destroy: function() {
                this.hClosed = null; this.hSaved = null; this.text = null; this.state = null; this.rVkl = null;
            },

            _raiseEvent: function(type, args) {
                var handlers = null;
                switch (type) {
                    case EventTypes.closing: handlers = this.hClosed; break;
                    case EventTypes.saved: handlers = this.hSaved; break;
                    case EventTypes.errorSaving: handlers = this.hErrSaving; break;
                }
                if (handlers == null) return;

                if (args)
                    for (var h in handlers) {
                    try {
                        if (handlers[h] !== null && typeof handlers[h] === "function")
                            handlers[h](args);
                    } catch (e) { }
                }
                else
                    for (var h in handlers) {
                    try {
                        if (handlers[h] !== null && typeof handlers[h] === "function")
                            handlers[h]();
                    } catch (e) { }
                }
            }

        }

        // Insures that only one instance of a function exist.
        function addHandler(array, fn) {
            for (var h in array)
                if (array[h] === fn)
                return;

            array.push(fn);
        }

        // Sets handler instance to null.
        function removeHandler(array, fn) {
            for (var h in array)
                if (array[h] === fn)
                array[h] = null;
        }

        // documents
        var docs = [];

        akshDocs = {
            createNew: function() {
                var name = "Document " + (docs.length + 1);
                var newDoc = new akshDoc(name);
                this._initDoc(newDoc);
            },

            open: function(path) {
                var self = this;
                AksStatus.setInfo("Opening " + path, true);
                $.ajax({ type: "POST", url: "akshar.aspx/GetDoc",
                    data: "{'path':'" + path + "'}",
                    contentType: "application/json; charset=utf-8", dataType: "json",
                    success: function(msg) {
                        if (msg.hasOwnProperty("d")) msg = msg.d;
                        var response;
                        eval("response = " + msg);
                        if (response.result == "success") {
                            var newDoc = new akshDoc(path.substring(path.lastIndexOf("/") + 1), unescape(response.text), path);
                            self._initDoc(newDoc);
                        } else
                            alert("The document couldn't be opened: " + response.msg);
                    },
                    error: function(XMLHttpRequest, textStatus, errorThrown) {
                        alert("The document couldn't be opened: an unknown, communication-specific error occurred.");
                    },
                    complete: function(xhr, status) {
                        AksStatus.setInfo("");
                    }
                })
            },

            documents: function() {
                var dcs = [];
                for (var d in docs)
                    if (docs[d] !== null)
                    dcs.push(docs[d]);

                return dcs;
            },

            setCurrent: function(doc) {
                if (akshFrame.current) {
                    // Need to propogate the text in all cases to have updated states and text props.
                    akshFrame.current.setText(akshRoot().body.innerHTML);
                    if (akshFrame.current === doc)
                        return;

                    akshFrame.current.focusChanged(false);
                } else {
                    this._initAkshFrame();
                }

                akshRoot().body.innerHTML = doc.getText();
                this._setTitle(doc.name);
                akshFrame.current = doc;
                doc.focusChanged(true);
            },

            getCurrent: function() {
                if (akshFrame.current)
                    return akshFrame.current;
                else return null;
            },

            saveAll: function() {

            },

            closeAll: function() {
                var openDocs = this.documents();
                for (var d in openDocs) {
                    this.setCurrent(openDocs[d]);
                    openDocs[d].close();
                }
            },

            // Internal
            _initDoc: function(newDoc) {
                newDoc.addListener(EventTypes.closing, this._docClosing);
                newDoc.addListener(EventTypes.saved, this._docSaved);
                newDoc.index = docs.push(newDoc) - 1; // returns new length so decreament by 1.
                akshDocs.setCurrent(newDoc);
            },

            _setTitle: function(docName) {
                var title = (String.isNullOrEmpty(docName) ? "Akshar" : docName + " - Akshar");
                setText(doctitle, title);
            },

            _initAkshFrame: function() {
                akshRoot().designMode = "on";
                akshRoot().open();
                akshRoot().write("<html><head><title>Akshar Document Window</title></head><body style=\"width:100%;overflow:scroll;\"></body></html>");
                akshRoot().close();
                VKM.bindControl(akshFrame, null, doKeyInput);
                menu.enable();
            },

            _disposeAkshFrame: function() {
                VKM.unbindControl(akshFrame);
                akshRoot().body.innerHTML = "";
                akshRoot().designMode = "off";
                akshFrame.current = null;
                akshDocs._setTitle("");
                menu.disable();
            },

            // Event handlers
            _docClosing: function(doc) {
                docs[doc.index] = null;
                for (var d in docs) {
                    if (docs[d] != null) {
                        akshDocs.setCurrent(docs[d]);
                        return;
                    }
                }

                akshDocs._disposeAkshFrame();
            },

            _docSaved: function(doc) {
                if (akshDocs.getCurrent() === doc)
                    akshDocs._setTitle(doc.name);
            }
        }

    })();


/* menu */
    var aksUtil = {
        trueString: "true", falseString: "false"
    }

    var lk = false, scact = '';
    function doKeyInput(e) {
        var keyCode = (e.which || e.keyCode);
switch (e.type.toLowerCase()) {
case "keydown":
if (!lk) return;

switch (keyCode) {
case 8:
 if (scact.length == 1) scact = '';
else if (scact.length > 1) scact = scact.substr(0, (scact.length - 1));
break;
default:
return;
}
break;
case "keypress":
    // To prevent non character generating keypress event to be processed (for firefox).
    if (e.charCode == 0) return;

    switch (keyCode) {
        case 35: // QF key
            if (!lk) lk = true; // Begin quick format mode .
            else {
                if (scact == '') { lk = false; return; } // Pressed consecutively so exit out of the quick format mode and let this key be passed.

                // Reset quick format but don't let this key be passed.
                lk = false
                scact = '';
            }
            break;
        case 32: // QF apply key
            if (!lk) return; // Not in quick format mode so pass as it is.

            lk = false; // Reset quick format.
            if (scact != '') {
                // Apply (if any) action.
                menu.applyCmd(scact);
                scact = '';
            }
            break;
        default:
            if (!lk) return; // Not in quick format mode so pass as it is.
            else scact += String.fromCharCode(keyCode); // append to action
            break;
    }
    break;
default:
    return;
}

return true; // handled, stop processing further.
}

var menu = {
    file: null,
    applyCmd: function(act, args) {
        if (String.isNullOrEmpty(act))
            return;
        var num = parseInt(act);
        if (!isNaN(num)) {
            if (num > akshDocs.documents().length) return;
            var docs = akshDocs.documents();
            for (i = 0, di = 0; i < docs.length; i++) {
                if (docs[i]) {
                    di++;
                    if (di == num) {
                        akshDocs.setCurrent(docs[i])
                        return;
                    }
                }
            }
        }

        //            var tx = VKM.regCtrls[0].lcText(2);
        // alert(tx);
        //            VKM.regCtrls[0].writeAt(RuleDirections.left, "hello", 2);
        var range;
        if (document.selection)
            range = akshRoot().selection.createRange();
        else
            range = akshRoot();

        switch (act.toLowerCase()) {
            case 'a':
                break;
            case 'ol':
                range.execCommand('InsertOrderedList');
                break;
            case 'ul':
                range.execCommand('InsertUnorderedList');
                break;
            case 'i':
                range.execCommand('Italic');
                break;
            case 'b':
                range.execCommand('Bold');
                break;
            case 'u':
                range.execCommand('Underline');
                break;
            case 'jc':
                range.execCommand('JustifyCenter');
                break;
            case 'jl':
                range.execCommand('JustifyLeft');
                break;
            case 'jr':
                range.execCommand('JustifyRight');
                break;
            case 'hr':
                range.execCommand('InsertHorizontalRule');
                break;
            case 'p':
                range.execCommand('InsertParagraph');
                break;
            case 'a':
                range.execCommand('CreateLink');
                break;
            case 'cls':
                range.execCommand('RemoveFormat');
                break;
            case 'sup':
range.execCommand('Superscript');
                break;
            case 'sub':
                range.execCommand('Subscript');
                break;
            case "n":
                akshDocs.createNew();
                break;
            case "o":
                dlgHands.showOpen();
                break;
            case "s":
                this.save(false);
                break;
            case "sa":
                this.save(true);
                break;
            case "c":
                this.close();
                break;
            case "m":
                this.file.focus();
                break;
            case "mf": case "me": case "mv": case "ml": case "md": case "mt": case "mh":
                var mi = $(theMenu).find("a[initial=" + act + "]:first");
                if (mi.length) {
                    $(mi[0]).trigger("click");
                }
                break;
        }

        if (act.length > 2) {
            switch (act.substr(0, 2).toLowerCase()) {
                case 'fc':
                    range.execCommand('ForeColor', false, act.substr(2).toLowerCase());
                    break;
                case 'bc':
                    range.execCommand('BackColor', false, act.substr(2).toLowerCase());
                    break;
                case 'fs':
                    range.execCommand('FontSize', false, act.substr(2).toLowerCase());
                    break;
            }
        }

    },

    isDisabled: false,

    disable: function() {
        this.isDisabled = true;
        $("#mfSave,#mfSaveAs,#mfClose,#mvFormat,#mlanguage,#mdocument").addClass("disabledMI").attr("disabled", "disabled");
        this.formattingBar(true);
        try {
            if (akshFrame.contentDocument)
                $(akshFrame.contentDocument).unbind("click keydown", this.hide);
            else
                $(akshFrame.contentWindow.document.body).unbind("click keydown", this.hide);
        } catch (e) { }
    },

    enable: function() {
        $("#mfSave,#mfSaveAs,#mfClose,#mvFormat,#mlanguage,#mdocument").removeClass("disabledMI").removeAttr("disabled");
        if (akshFrame.contentDocument)
            $(akshFrame.contentDocument).bind("click keydown", this.hide);
        else
            $(akshFrame.contentWindow.document.body).bind("click keydown", this.hide);
        this.isDisabled = false;
    },

    hide: function(e) {
        if (opopup != null) {
            menuVisible(opopup, false); opopup = null;
        }
    },

    close: function() {
        var curDoc = akshDocs.getCurrent();
        if (!curDoc)
            return;
        if (curDoc.canClose()) {
            curDoc.close();
            return;
        }
        var isNew = false, msg = (curDoc.state == DocStates.created ? (isNew = true, "Would you like to save '" + curDoc.name + "'?") : "Would you like to save the changes to '" + curDoc.name + "'?");

        dlgHands.showMessage("Akshar", msg, function(button) {
            switch (button) {
                case MsgButtons.cancel:
                    return;
                case MsgButtons.no:
                    curDoc.close();
                    break;
                case MsgButtons.yes:
                    if (isNew)
                        dlgHands.showSave(curDoc, function(doc, path) { curDoc.save(path, true); });
                    else
                        curDoc.save(null, true);
                    break;
            }
        });
    },

    save: function(saveAs) {
        var curDoc = akshDocs.getCurrent();
        if (curDoc) {
            if (saveAs || curDoc.state == DocStates.created) {
                dlgHands.showSave(curDoc);
            } else
                curDoc.save(null);
        }
    },

    tbFormat: null, mvFormat: null,
    formattingBar: function(hide) {
        if (typeof hide === "undefined" && !this.tbFormat.opened) {
            this.tbFormat.opened = true;
            this.tbFormat.style.display = "block";
            markedMenu(this.mvFormat.parentNode, this.mvFormat.parentNode);
        } else {
            this.tbFormat.opened = false;
            this.tbFormat.style.display = "none";
            unmarkedMenu(this.mvFormat.parentNode);
        }
    }

};


/* status */
    (function() {
        var infoTid, bar;
        var timeout = 10000;

        $(function() {
            bar = _("statusPane");
        })

        function clearInfo() {
            infoTid = null;
            setText(bar, "");
        }

        AksStatus = {
            setInfo: function(text, dontTimeout) {
                if (infoTid)
                    window.clearTimeout(infoTid);

                setText(bar, text);
                if (!String.isNullOrEmpty(text) && !dontTimeout && window.setTimeout) {
                    infoTid = window.setTimeout(clearInfo, timeout);
                    bar.scrollIntoView();
                }
            }

        };

    })();


/* dialog handlers */
var MouseButtons = {Left:1,Right:2},
Dialogs = {open:"dlgOpen",save:"dlgSave"},
MsgButtons = {yes:1,no:2,cancel:4, ok: 8};

    (function() {
        var jOpen, jSave,jMessage,jInput,
        odocs = null, odmBy = null,
                cofPop,codPop,
                invalidFName=/[\\/:*?"<>|']|\.\./;


$(document).ready(function() {
cofPop = $("<div tabindex=-1 class='c-menu'><a tabindex=0 onclick='' id='bdel'>Delete</a></div>");
        codPop = $("<div tabindex=-1 class='c-menu'><a tabindex=0 onclick='' id='bdel'>Delete</a><a tabindex=0 onclick='' id='bnew'>New Folder</a></div>");
codPop[0].miDel = codPop.find("#bdel");

                        jMessage=$("<div tabindex=-1></div>").dialog({
                    autoOpen: false, modal: true, 
                    buttons: { Yes: function() { jMessage.dialog("close");jMessage.fn(MsgButtons.yes); },
                    No: function() { jMessage.dialog("close");jMessage.fn(MsgButtons.no); },
                    Cancel: function() { jMessage.dialog("close");jMessage.fn(MsgButtons.cancel); }
                    } // buttons obj
});

jInput=$("<div tabindex=-1></div>").dialog({
                    autoOpen: false, modal: true, 
                    buttons: { Ok: function() {
var txInput = $("#txInput", jInput)[0];
if (String.isNullOrEmpty(txInput.value) )
return;
var o = jInput.o;
                    if ((o.regex && (o.regex.exec(txInput.value) != null) != o.regMustPass) || (o.len > 0 && txInput.value.length > o.len)) {
alert(o.invalidMsg);
txInput.focus();
return;
                    }
                    jInput.button  = MsgButtons.ok;
                    jInput.dialog("close");o.fn(MsgButtons.ok, txInput.value);
                    jInput.o=null;
                                        },
                    Cancel: function() { 
                    jInput.button  = MsgButtons.cancel;
                    jInput.dialog("close");jInput.o.fn(MsgButtons.cancel);
                    jInput.o=null;
                    }
                    } // buttons obj
})
.bind("dialogclose", function() {
if (jInput.button === null)
jInput.o = null;
});

                })

        dlgHands = function() { };
        dlgHands.showMessage = function(title,message,fn) {
jMessage.dialog("option", "title", title);
jMessage.html(message);
jMessage.fn = fn;
jMessage.dialog("open");
jMessage[0].focus();
        }

var jInputOptions= { title:"User Prompt",
msg:"Value: ",fn:null,defValue:"",len:0,regex:null,regMustPass:null,invalidMsg:"Input value is not in a correct format."
}

dlgHands.showInput = function(o) {

o = jQuery.extend(jInputOptions, o);
jInput.dialog("option", "title", o.title);
jInput.html(o.msg + "<input style=\"margin-left:10px;\" type=\"text\" id=\"txInput\" value=\"" + o.defValue + "\" " + (o.len > 0 ? "maxlength=" + o.len.toString() : "") + " />");
jInput.o = o;
// jInput.maxLen = (typeof len === "number" ? len : 0);
// jInput.regex = inputRegexp;
// jInput.rpass = regMustPass;
// jInput.invalidMsg = invalidMsg;
jInput.button = null;
jInput.dialog("open");
jInput[0].focus();
        }

        dlgHands.showOpen = function() {
            if (jOpen == null)
                jOpen = $("<div tabindex=-1></div>").dialog({
                    autoOpen: false, modal: true, title: "Open Document", width: 500,closeOnEscape:false,
                    open: doOpen, buttons: { Open: function(e) {
                    var path =  $("#txPath", this).val(), file = $("#txFile", this).val();
if (String.isNullOrEmpty(path) || String.isNullOrEmpty(file))
{
return;
}

if (String.isNullOrEmpty(jOpen.cmds)) { 
jOpen.dialog("close");
akshDocs.open(path + file);
} else { // Preserve so that it can be opened once IO ops done.
jOpen.filePath = path + file;
jOpen.dialog("close");
}
                    },
                    Refresh: function() { refreshListings(Dialogs.open); }
                    } // buttons obj
                })
                .bind("click", { dlgid: Dialogs.open }, doClicked)
                .bind("keydown", { dlgid: Dialogs.open }, osKeyDown)
.bind("dialogbeforeclose", {dlgid:Dialogs.open, fn: function(success) {
jOpen.cmds = "";
if (success) {
jOpen.dialog("close");
if (!String.isNullOrEmpty(jOpen.filePath))
akshDocs.open(jOpen.filePath);
}
jOpen.filePath = null;
}}, execCommands);

            jOpen.cmds = "";
            jOpen.filePath = null;
            cofPop.remove(); codPop.remove();
            jOpen.dialog("open");
            jOpen[0].focus();
        }

dlgHands.showSave = function(akshDoc, callback) {
            if (jSave == null)
createSaveDlg();

            jSave.cmds = "";
            jSave.filePath = null;
            cofPop.remove(); codPop.remove();
            akshDoc.addListener(EventTypes.saved, docSaved);
            jSave.doc = akshDoc;
            jSave.callback = callback;
            jSave.dialog("open");
            jSave[0].focus();
        }

function createSaveDlg() {
                jSave = $("<div tabindex=-1></div>").dialog({
                    autoOpen: false, modal: true, title: "Save Document", width: 500,
                    open: dsOpen, buttons: { Save: function() {
                    var path =  $("#txPath", this).val(), file = $("#txFile", this).val();
if (String.isNullOrEmpty(path) || String.isNullOrEmpty(file))
{
return;
}

if (invalidFName.exec(file) || file.length > 20) {
alert('The name cannot be greater than 20 characters in length and cannot contain any of the following characters: \ / : * ? .. " < > | \' ');
return;
}
var exist = false;
for (var f in jSave.selDir.files)
if (jSave.selDir.files[f].name.toLowerCase() == file.toLowerCase()) { exist = true;break; }
if (exist && !confirm("The document with the name '" + file + "' already exist, do you want to replace it?"))
{
return;
}

if (String.isNullOrEmpty(jSave.cmds)) { 
jSave.dialog("close");
if (typeof jSave.callback === "function") // Usually following is for save and close functionality.
jSave.callback(jSave.doc, path + file);
else
jSave.doc.save(path + file);
} else { // Preserve so that it can be Saved once IO ops done.
jSave.filePath = path + file;
jSave.dialog("close");
}
                    },
                    Refresh: function() { refreshListings(Dialogs.save); }
                    } // buttons obj
                })
                .bind("click", { dlgid: Dialogs.save }, doClicked)
                .bind("keydown", { dlgid: Dialogs.save }, osKeyDown)
.bind("dialogbeforeclose", {dlgid:Dialogs.save, fn: function(success) {
jSave.cmds = "";
if (success) {
jSave.dialog("close");
if (!String.isNullOrEmpty(jSave.filePath)) {
if (typeof jSave.callback === "function") // Usually following is for save and close functionality.
jSave.callback (jSave.doc, jSave.filePath);
else
jSave.doc.save(jSave.filePath);
}
}
jSave.filePath = null;
}}, execCommands);
}

function execCommands(e) {
var dlg = (e.data.dlgid == Dialogs.open ? jOpen : jSave);
if (!String.isNullOrEmpty(dlg.cmds)) {
var dlgWait = $("<div tabindex=-1>Executing commands ....</div>").dialog({autoOpen: false,title:'Please Wait',modal:true});
dlgWait.dialog("open");
dlgWait[0].focus();
$.ajax({ type: "POST", url: "akshar.aspx/ExecCommand",
                    data: "{'cmd':'" + dlg.cmds + "','IncludeListingOnError':true}",
                    contentType: "application/json; charset=utf-8",dataType: "json",
                    success: function(msg) {
                        if (msg.hasOwnProperty("d")) msg = msg.d;
                        var response;
                        eval("response = " + msg);
                        if (response.result == "failure") {
                        if (dlgWait.dialog("isOpen"))
                        dlgWait.html(response.msg);
                            odocs = response.o;
                            odmBy = 'err';
                            if (e.data.fn)
                            e.data.fn(false);
                        } else {
dlgWait.dialog("destroy");
if (e.data.fn)
e.data.fn(true);
}
                    },
                    error: function(XMLHttpRequest, textStatus, errorThrown) { alert(textStatus); }
                })
                return false;
}
}

        function doClicked(e) {
            var pli, dWin = $("#" + e.data.dlgid, this),
            dlg = (e.data.dlgid == Dialogs.open ? jOpen : jSave);
            pli = $(e.target).parents("li:first")[0];
            cofPop.remove(); codPop.remove();
            switch (e.target.tagName.toUpperCase()) {
                case "A":
                    switch (e.target.getAttribute("type")) {
                        case "dir":
                            $("#txPath", dWin[0]).val(pli.o.path);
                            $("#txFile", dWin[0]).val("");
if (dlg.selDirItem)
$(dlg.selDirItem).removeClass("fileSelected");
$(e.target).addClass("fileSelected");
dlg.selDirItem=e.target;
                            dlg.selDir = pli.o;
                            var flst = $("<ul class='fileLV'></ul>");
                            for (var f in pli.o.files) {
                                var li = $("<li><img onclick='' src='images/o.gif' id='cOptions' alt='Options' title='Options' /><a onclick='' type='file'>" + pli.o.files[f].name + "</a></li>")[0];
                                li.odir = pli.o; li.o = pli.o.files[f];
                                flst.append(li);
                            }
                            $("div.rpane", dWin[0]).html(flst[0]);
                            break;
                        case "file":
                            $("#txFile", dWin[0]).val(pli.o.name);
                            if (dlg.selFileItem)
$(dlg.selFileItem).removeClass("fileSelected");
$(e.target).addClass("fileSelected");
dlg.selFileItem=e.target;
                            break;
                        default:
                            switch (e.target.id) {
                                case "bdel":
                                    if (!pli.o.dirs) {
                                        if (confirm("Really wish to delete the file '" + pli.o.name + "'?")) {
                                            dlg.cmds += "delf:" + pli.odir.path + pli.o.name + "|";
                                            for (i = 0; i < pli.odir.files.length; i++)
                                            if (pli.odir.files[i].name == pli.o.name) {
                                            pli.odir.files.splice[i, 1];
                                            break;
                                            }
                                            $(pli).remove();
                                            odmBy = e.data.dlgid;
                                        }

                                    } else {

                                                                            if (!pli.o.isRoot && confirm("Really wish to delete the folder '" + pli.o.name + "' and all its contents?")) {
                                            dlg.cmds += "deld:" + pli.o.path.substring(0, pli.o.path.length - 1) + "|";
                                            for (i = 0; i < pli.odir.dirs.length; i++)
                                            if (pli.odir.dirs[i].name == pli.o.name) {
                                            pli.odir.dirs.splice[i, 1];
                                            break;
}
if (pli.odir.dirs.lenght == 0) {
// $(pli.parentNode).sibling("#bexp").remove();
$(pli.parentNode).remove();
} else $(pli).remove();
                                            odmBy = e.data.dlgid;
                                        }
                                    }
                                    break;
                                case "bnew":
dlgHands.showInput({
msg:"Folder Name:",defValue:"New Folder",len:20,
regex:invalidFName,regMustPass:false,
invalidMsg:'The name cannot be greater than 20 characters in length and cannot contain any of the following characters: \ / : * ? .. " < \' > | ',
fn:inputSubmit
});
                                    break;
                            }
                    }
                    break;
                case "IMG":
                    switch (e.target.id) {
                        case "cOptions":
                            if (!pli.o.dirs) { // file options
                                $(pli).append(cofPop);
                                cofPop[0].style.top = (e.target.offsetTop + 22).toString() + "px";
cofPop[0].style.left = (e.target.offsetLeft + 10).toString() + "px";
                                cofPop[0].focus();
                                                            } else {
                                                                if (pli.o.isRoot)
                                                                 $(codPop[0].miDel).addClass("disabledMI").attr("disabled", true);
                                else $(codPop[0].miDel).removeClass("disabledMI").removeAttr("disabled");
                                if (pli.o.dirs.length)
                                $("ul:first", pli).before(codPop);
                                                                else $(pli).append(codPop);
                                codPop[0].style.top = (e.target.offsetTop + 22).toString() + "px";
codPop[0].style.left = (e.target.offsetLeft + 10).toString() + "px";
                                codPop[0].focus();
                            }
                            // alert(codPop[0].offsetTop);
                            // alert(codPop[0].offsetLeft);
                            // alert(codPop[0].offsetParent.innerHTML);
                            // codPop.css("z-index", 20000);
                            break;
                        case "bexp":
                        var subLst =$(e.target).siblings("ul");
if (subLst.length) {
if (isDisplayed(subLst[0]))
subLst.hide();
else subLst.show();
}
                                // e.target.setAttribute("tog", false);
                            // } else {
                                // $(e.target).siblings("ul").show();
                                // e.target.setAttribute("tog", true);
                            break;
                    }
                    break;
            }

function inputSubmit(button, newDir) {
if (button != MsgButtons.ok)
return;

for (var d in pli.o.dirs)
if (pli.o.dirs[d].name.toLowerCase() == newDir.toLowerCase()) {
alert("Folder with the name '" + newDir + "' already exists.");
break;
}

var li = $("<li><img onclick='' src='images/o.gif' id='cOptions' alt='Options' title='Options' /><img onclick='' src='images/dir.gif' id='bexp' alt='Expand' title='Expand' /><a onclick='' type='dir'>" + newDir + "</a></li>")[0];
li.odir = pli.o;
li.o = {name:newDir,dirs:[],files:[], path:pli.o.path + newDir + "/"};
pli.o.dirs.push(li.o);
dlg.cmds += "newd:" + pli.o.path + newDir + "|";
if (pli.o.dirs.length == 1) {
// pli.appendChild($("<img onclick='' src='images/dir.gif' id='bexp' alt='Expand' title='Expand' />")[0]);
pli.appendChild($("<ul class='dirTrV'></ul>").append(li)[0]);
} else $(pli).children("ul").append(li);
odmBy = e.data.dlgid;
}
        }

        function doOpen(e) {
        jOpen.contentWin = this;
                        if (odocs == null) {
loadDList(this, initOpen);
            } else if (!jOpen.hasInitialized || odmBy !== null && odmBy !== Dialogs.open) {
                initOpen(this);
                odmBy = null;
                            }
                                    }

        function initOpen(win) {
            $(win).html($("#dlgOpen").clone().show()).find("div.lpane").append(getDList()).find("a:first").trigger("click");
                    jOpen.hasInitialized = true;
        }

function dsOpen(e) {
jSave.contentWin = this;
                        if (odocs == null) {
loadDList(this, initSave);
            } else if (!jSave.hasInitialized || odmBy !== null && odmBy !== Dialogs.save) {
                initSave(this);
                odmBy = null;
                            }
        }

        function initSave(win) {
            $(win).html($("#dlgSave").clone().show()).find("div.lpane").append(getDList()).find("a:first").trigger("click");
                    jSave.hasInitialized = true;
        }

function loadDList(win, fn) {
$(win).html("<p>Loading ...</p>");
                $.ajax({
                    type: "POST",
                    url: "akshar.aspx/GetDocsListings",
                    data: "{}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function(msg) {
                        if (msg.hasOwnProperty("d")) msg = msg.d;
                        var response;
                        eval("response = " + msg);
                        if (response.result == "success") {
                            odocs = response.o;
                            fn(win);
                                                    } else
                            $(win).html(response.msg);
                    },
                    error: function(XMLHttpRequest, textStatus, errorThrown) { alert(textStatus); }
                })
                }

        function getDList() {
            var ul = $("<ul class='dirTrV'></ul>");
            var lst = ul.clone();
            createList(lst, odocs);
            return lst[0];

            function createList(el, from, path, parent) {
                var li = $("<li><img onclick='' src='images/o.gif' id='cOptions' alt='Options' title='Options' /><img onclick='' src='images/dir.gif' id='bexp' alt='Expand' title='Expand' /><a onclick='' type='dir'>" + from.name + "</a></li>")[0];
                if (from.isRoot) {
                    li.isRoot = true;
                    from.path = "/";
                } else {
                    from.path = path + from.name + "/";
                    li.odir = parent;
                }
                li.o = from;
                if (from.dirs.length) {
                    // li.appendChild($("<img onclick='' src='images/dir.gif' id='bexp' alt='Expand' title='Expand' />")[0]);
                    var slst = lst.clone().hide();
                    li.appendChild(slst[0]);
                    for (var dir in from.dirs) {
                        createList(slst, from.dirs[dir], from.path, from);
                    }
                }
                el.append(li);
            }
        }

function refreshListings(dlgid) {
var otherDlg, curDlg = (dlgid == Dialogs.save ? (otherDlg=jOpen,jSave) : (otherDlg=jSave,jOpen));
cofPop.remove(); codPop.remove();
oDocs = null;
curDlg.filePath=null;
curDlg.cmds="";
curDlg.selDir=curDlg.selDirItem=curDlg.selFileItem=null;
curDlg.hasInitialized=false;
if (otherDlg) {
otherDlg.selDir=otherDlg.selDirItem=otherDlg.selFileItem=null;
otherDlg.hasInitialized=false;
}
if (dlgid == Dialogs.save) {
loadDList(jSave.contentWin, initSave);
} else {
loadDList(jOpen.contentWin, initOpen);
}
}

function docSaved(doc) {
doc.removeListener(EventTypes.saved, arguments.callee);
var files = jSave.selDir.files;
for (var f in files) {
if (files[f].name.toLowerCase() == doc.name) {
if (files[f].name != doc.name) {
files[f].name = doc.name; // In case letter case changes.
odmBy="onsave";
}
return;
}
} // for
files.push({name: doc.name});
odmBy="onsave";
}

function osKeyDown(e) {
var dlg = (e.data.dlgid == Dialogs.save ? jSave : jOpen);
if (e.which==$.ui.keyCode.ESCAPE) {
// If a context menu is active, close it.
if (codPop.parents().length || cofPop.parents().length) {
cofPop.remove(); codPop.remove();
} else { // Close the dialog
dlg.dialog("close");
}
}
}

    })();

/* menu handlers */
var mlDef = _('mlDef'),
mlAdd = _('mlAdd'), mlAddSep;

function mlOpening(e) {
    var marked = false, curVkl = akshFrame.current.getVkl();
    $("a", mlPop).each(function(i) {
        if (typeof this.parentNode.vkl === "undefined")
            return;
        var vkl = this.parentNode.vkl;
        if (!marked && vkl === curVkl) {
            markedMenu(mlPop, this.parentNode);
            marked = true;
        }
        var miDisabled = true;
        switch (vkl.status) {
            case Statuses.waiting: setText(this, vkl.name + "      (Loading)"); break;
            case Statuses.unavailable: setText(this, vkl.name + "      (Unavailable)"); break;
            case Statuses.error: setText(this, vkl.name + "      (Error)"); break;
            default:
                miDisabled = false;
                setText(this, vkl.name);
                break;
        }
        if (!miDisabled) {
            if (isDisabled(this))
                $(this).removeClass("disabledMI").removeAttr("disabled");
        } else {
        if (!isDisabled(this))
            $(this).addClass("disabledMI").attr("disabled", "disabled");
        }
    })
if (!marked)
markedMenu(mlPop, mlDef.parentNode);
}

function mdOpening(e) {
    mdPop.innerHTML = "";
var docs = akshDocs.documents();
    for (var i in docs) {
        var newItem = $("<div class='menuitem'><a onclick='' tabindex=0>" + docs[i].name + "</a></div>")[0];
        newItem.doc = docs[i];
        if (docs[i] === akshDocs.getCurrent())
            markedMenu(mdPop, newItem);

        $(mdPop).append(newItem);
    }
}

function getActualMI(src) {
    var mItem = null;
    if (src.tagName == "A")
        return src;
    else {
        src.tagName == "DIV" && (mItem = $(src).children("a")) || src.tagName == "IMG" && (mItem = $(src).siblings("a"));
        return (mItem !== null && mItem.length > 0 ? mItem[0] : null);
    }
}

function mfClicked(e) {
    e.stopPropagation();
    var mItem = getActualMI(e.target);
    if (mItem == null)
        return;
        if (isDisabled(mItem))
            return;
    menuVisible(opopup, false);
    var curDoc;
    switch (mItem.id) {
        case 'mfNew': akshDocs.createNew();
            akshRoot().body.focus();
            break;
        case 'mfOpen':
            dlgHands.showOpen();
            break;
        case 'mfSave':
            menu.save(false);
            break;
        case "mfSaveAs":
            menu.save(true);
            break;
        case "mfClose":
            menu.close();
            break;
    }
}

function mvClicked(e) {
    e.stopPropagation();
    var mItem = getActualMI(e.target);
    if (mItem == null)
        return;
    if (isDisabled(mItem))
        return;
    menuVisible(opopup, false);

    switch (mItem.id) {
        case 'mvFormat':
            menu.formattingBar();
            break;
    }
}

function mlClicked(e) {
    e.stopPropagation();
    var mItem = getActualMI(e.target);
    if (mItem == null)
        return;
    if (isDisabled(mItem))
        return;
        menuVisible(opopup, false);
        if (isMarked(mItem.parentNode))
    return;

    switch (mItem.id) {
        case 'mlAdd':
            var vkls = [];
            for (var v in VKM.VKLs)
            if (VKM.VKLs[v] != null)
                vkls.push({ name: VKM.VKLs[v].name, type: VKM.VKLs[v].type });

            showMD("addvkl.aspx", { vkls: vkls, fn: addVKL });
            break;
        case 'mlDef': akshFrame.current.setVkl(null);
                    break;
                default:
                    if (mItem.parentNode.vkl)
                        akshFrame.current.setVkl(mItem.parentNode.vkl);
    }
    akshRoot().body.focus();
}

function mdClicked(e) {
    var mItem = getActualMI(e.target);
    if (mItem == null)
        return;
    menuVisible(opopup, false);
    if (isMarked(mItem.parentNode))
        return;

    if (mItem.parentNode.doc)
        akshDocs.setCurrent(mItem.parentNode.doc);
    akshRoot().body.focus();
}

function addVKL(vkls) {
    $("div[isVkl]", mlPop).remove();
    mlAddSep.remove();
    mlAddSep.isAdded = false;
    var docs = akshDocs.documents();

    for (var v in VKM.VKLs) {
        var obj = VKM.VKLs[v];
        if (obj === null)
            continue;

    var exist = vkls[obj.name.toUpperCase() + (obj.type == VKLTypes.character ? "_c" : "")];
    if (!exist) {
    // Need to remove all references - user deselected it.
    for (var d in docs) {
        if (docs[d].getVkl() === obj)
            docs[d].setVkl(null);
    }
    VKM.VKLs[v] = null;
    continue;

    } else {
    addVklMenuItem(obj, exist.name);
    exist.processed = true;
    }
}

var names = '', types = '';
for (var v in vkls) {
names+=vkls[v].name+"|";
types+=vkls[v].type+"|";
if (vkls[v].processed)
continue;

var vkl = VKM.createVKL(vkls[v].name, vkls[v].type, true);
addVklMenuItem(vkl);
}

$.ajax({ type: "POST", url: "account.aspx/SetVKLs",
                    data: "{'name':'" + names + "','type':'" + types + "'}",
                    contentType: "application/json; charset=utf-8",dataType: "json"
})
}

function addVklMenuItem(vkl, name) {
    if (!mlAddSep.isAdded) {
        $(mlAdd.parentNode).before(mlAddSep);
        mlAddSep.isAdded = true;
    }

    name = (String.isNullOrEmpty(name) ? vkl.name : name);
    var newItem = $("<div isVkl=true class='menuitem'><a onclick='' tabindex=0>" + name + "</a></div>")[0];
    newItem.vkl = vkl;
    mlAddSep.before(newItem);
}


    var oColors = { AliceBlue: "F0F8FF", AntiqueWhite: "FAEBD7", Aqua: "00FFFF", Aquamarine: "7FFFD4", Azure: "F0FFFF", Beige: "F5F5DC", Bisque: "FFE4C4", Black: "000000", BlanchedAlmond: "FFEBCD", Blue: "0000FF", BlueViolet: "8A2BE2", Brown: "A52A2A", BurlyWood: "DEB887", CadetBlue: "5F9EA0", Chartreuse: "7FFF00", Chocolate: "D2691E", Coral: "FF7F50", CornflowerBlue: "6495ED", Cornsilk: "FFF8DC", Crimson: "DC143C", Cyan: "00FFFF", DarkBlue: "00008B", DarkCyan: "008B8B", DarkGoldenrod: "B8860B", DarkGray: "A9A9A9", DarkGreen: "006400", DarkKhaki: "BDB76B", DarkMagenta: "8B008B", DarkOliveGreen: "556B2F", DarkOrange: "FF8C00", DarkOrchid: "9932CC", DarkRed: "8B0000", DarkSalmon: "E9967A", DarkSeaGreen: "8FBC8F", DarkSlateBlue: "483D8B", DarkSlateGray: "2F4F4F", DarkTurquoise: "00CED1", DarkViolet: "9400D3", DeepPink: "FF1493", DeepSkyBlue: "00BFFF", DimGray: "696969", DodgerBlue: "1E90FF", FireBrick: "B22222", FloralWhite: "FFFAF0", ForestGreen: "228B22", Fuchsia: "FF00FF", Gainsboro: "DCDCDC", GhostWhite: "F8F8FF", Gold: "FFD700", Goldenrod: "DAA520", Gray: "808080", Green: "008000", GreenYellow: "ADFF2F", Honeydew: "F0FFF0", HotPink: "FF69B4", IndianRed: "CD5C5C", Indigo: "4B0082", Ivory: "FFFFF0", Khaki: "F0E68C", Lavender: "E6E6FA", LavenderBlush: "FFF0F5", LawnGreen: "7CFC00", LemonChiffon: "FFFACD", LightBlue: "ADD8E6", LightCoral: "F08080", LightCyan: "E0FFFF", LightGoldenrodYellow: "FAFAD2", LightGreen: "90EE90", LightGrey: "D3D3D3", LightPink: "FFB6C1", LightSalmon: "FFA07A", LightSeaGreen: "20B2AA", LightSkyBlue: "87CEFA", LightSlateGray: "778899", LightSteelBlue: "B0C4DE", LightYellow: "FFFFE0", Lime: "00FF00", LimeGreen: "32CD32", Linen: "FAF0E6", Magenta: "FF00FF", Maroon: "800000", MediumAquamarine: "66CDAA", MediumBlue: "0000CD", MediumOrchid: "BA55D3", MediumPurple: "9370DB", MediumSeaGreen: "3CB371", MediumSlateBlue: "7B68EE", MediumSpringGreen: "00FA9A", MediumTurquoise: "48D1CC", MediumVioletRed: "C71585", MidnightBlue: "191970", MintCream: "F5FFFA", MistyRose: "FFE4E1", Moccasin: "FFE4B5", NavajoWhite: "FFDEAD", Navy: "000080", OldLace: "FDF5E6", Olive: "808000", OliveDrab: "6B8E23", Orange: "FFA500", OrangeRed: "FF4500", Orchid: "DA70D6", PaleGoldenrod: "EEE8AA", PaleGreen: "98FB98", PaleTurquoise: "AFEEEE", PaleVioletRed: "DB7093", PapayaWhip: "FFEFD5", PeachPuff: "FFDAB9", Peru: "CD853F", Pink: "FFC0CB", Plum: "DDA0DD", PowderBlue: "B0E0E6", Purple: "800080", Red: "FF0000", RosyBrown: "BC8F8F", RoyalBlue: "4169E1", SaddleBrown: "8B4513", Salmon: "FA8072", SandyBrown: "F4A460", SeaGreen: "2E8B57", Seashell: "FFF5EE", Sienna: "A0522D", Silver: "C0C0C0", SkyBlue: "87CEEB", SlateBlue: "6A5ACD", SlateGray: "708090", Snow: "FFFAFA", SpringGreen: "00FF7F", SteelBlue: "4682B4", Tan: "D2B48C", Teal: "008080", Thistle: "D8BFD8", Tomato: "FF6347", Turquoise: "40E0D0", Violet: "EE82EE", Wheat: "F5DEB3", White: "FFFFFF", WhiteSmoke: "F5F5F5", Yellow: "FFFF00", YellowGreen: "9ACD32" };


    $(document).ready(function() {
            menu.file = _("mfile");
        menu.tbFormat = _("tbFormat");
        menu.tbFormat.opened = false;
        menu.mvFormat = _('mvFormat');
        mlAddSep = $("<hr class=\"m-separator\" />");
        mlAddSep.isAdded = false;
        for (var c in oColors) {
            var opt = document.createElement("option"), copt = document.createElement("option");
            copt.text = opt.text = c;
            copt.value = opt.value = oColors[c];
            tfFor.options.add(opt);
            tfBack.options.add(copt);
        }
        tfBack.selectedIndex = tfFor.selectedIndex = tfBack.prevIndex = tfFor.prevIndex = -1;

        if (typeof window.settings !== "undefined" && settings.vkls) {
            for (var v in settings.vkls) {
                var vkl = VKM.createVKL(settings.vkls[v].name, settings.vkls[v].type, true);
                addVklMenuItem(vkl);
            }
        }

        akshDocs.createNew();


        $("#mfile, #medit, #mview, #mlanguage, #mdocument,#mtools,#mhelp").each(function() {
            var mpop = _(this.getAttribute("initial") + "Pop");
            mpop.style.left = this.parentNode.offsetLeft.toString() + "px";
            // alert(mpop.id);
            // alert(mpop.style.left);
            mpop.mainMenu = this;
            switch (mpop.id) {
                case "mfPop":
                    $(mpop).bind("click", mfClicked);
                    break;
                case "mlPop":
                    markedMenu(mpop, $("#mlDef", mpop)[0].parentNode);
                    $(mpop).bind("opening", mlOpening).bind("click", mlClicked);
                    break;
                case "mdPop":
                    $(mpop).bind("opening", mdOpening).bind("click", mdClicked);
                    break;
                case "mvPop":
                    $(mpop).bind("click", mvClicked);
                    break;
            }
        }).bind("click mouseover", menuShowHide);

        $("a", theMenu).disableSelection();
        $(doctitle).disableSelection();

        $("#aview").click(function() {
            if (accView) { accView = false; $(this).text("Turn off mouse over effects for menu navigation."); }
            else { accView = true; $(this).text("Turn on mouse over effects."); }
        }).trigger("click").disableSelection();

        $("body").bind("click keydown", function(e) {
            if (e.type == "keydown" && e.which != $.ui.keyCode.ESCAPE)
                return;

            if (opopup != null) {
                menuVisible(opopup, false); opopup = null;
            }
        })

        $("#tfFor,#tfBack").bind("click change keyup", function() {
            if (this.selectedIndex >= 0 && this.selectedIndex != this.prevIndex) {
                if (this === tfFor)
                    menu.applyCmd("fc" + this.options[this.selectedIndex].value);
                else
                    menu.applyCmd("bc" + this.options[this.selectedIndex].value);
            }
            this.prevIndex = this.selectedIndex;
        })
        $("a", menu.tbFormat).disableSelection().bind("click", function() {
            var cmd = this.getAttribute("cmd");
            if (!String.isNullOrEmpty(cmd))
                menu.applyCmd(cmd);
        })

    }) // ready
});