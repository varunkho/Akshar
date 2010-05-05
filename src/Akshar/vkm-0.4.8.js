/*
* Virtual Keyboard Manager (VKM) V0.4.8
* Copyright© 2010-2011 Varun Khosla
* Permission is hereby granted, free of charge, to any person obtaining
* a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including
* without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
* permit persons to whom the Software is furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be
* included in all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
* NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
* OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*
* Date: 2011-03-18 18:0:0 -0900 (Fri, 18 Mar 2011)
*/

window.onload = function() {
    if (typeof initVKM !== "undefined")
        initVKM();
    }

var Statuses = {waiting: 0,ready: 1,error: 2,unavailable: 3},
VKLTypes={key: "KEY", character: "CHARACTER"},
RuleTypes = { regexp: "REGX", text: "TEXT" },
RuleDirections = { left: "LEFT", right: "RIGHT" },
wb = /[ \s]/;

(function() {

    VKM = {
        loaded: function(vkl, name, type) {
            var objVKL = this.VKLExists(name, type);
            if (objVKL !== null)
                objVKL.parse(vkl);
            // else
                // alert("not found:" + name + "," + type + "\r\nReceive:" + vkl);
        },

        load: function(args, objVKL) {
            var vscript = document.createElement("script");
            vscript.setAttribute("type", "text/javascript");
            vscript.setAttribute("src", "http://localhost:4025/service.aspx?method=GetVKL&name=" + encodeURIComponent(objVKL.name) + "&type=" + objVKL.type + "&callVkm=yes");
            document.documentElement.getElementsByTagName("HEAD")[0].appendChild(vscript);
        },

        VKLs: [], regCtrls: [], defaultVKL: null,
        isBound: function(control) {
            if (this.getCAks(control)) return true;
            else return false;
        },

        createVKL: function(name, type, nonblocking) {
            if (String.isNullOrEmpty(name)) return null;
            type = (type.toUpperCase() == VKLTypes.character ? VKLTypes.character : VKLTypes.key);
            nonblocking = (true == nonblocking);

            var newVKL = new VKL(name, type, nonblocking);
            this.load("{'langCode':'', 'vklName':'" + newVKL.name + "', 'vklType':'" + newVKL.type + "'}", newVKL);
            this.VKLs.push(newVKL);
            return newVKL;
        },

        bindControl: function(control, VKL, keypressHandler) {
            var newAks = new Aks(control, VKL); this.regCtrls.push(newAks);
            if (typeof keypressHandler === "function") newAks.fnKeyPress = keypressHandler; return newAks;
        },

        rebindControl: function(control, VKL, keyHandler) {
            var aks = this.getCAks(control);
            if (aks) {
                aks.VKL = VKL;
                aks.suspended = false;
                if (typeof keyHandler === "function") aks.fnKeyPress = keyHandler;
                else aks.fnKeyPress = null;
            }
        },

        unbindControl: function(ctrl) {
            var newRCList = [];
            for (var i in this.regCtrls) {
                if (this.regCtrls[i].control === ctrl) {
                    this.regCtrls[i].dispose();
                } else {
                    newRCList.push(this.regCtrls[i]);
                }
            }
            this.regCtrls = newRCList;
        },

        suspendControl: function(control) {
            var aks = this.getCAks(control);
            if (aks)
                aks.suspended = true;
        },

        resumeControl: function(control) {
            var aks = this.getCAks(control);
            if (aks)
                aks.suspended = false;
        },

        getCAks: function(ctrl) {
            for (var i in this.regCtrls) { if (this.regCtrls[i].control === ctrl) return this.regCtrls[i]; }
            return null;
        },

        // Internal
        getAks: function(ctrl) {
            for (var i in this.regCtrls) { if (this.regCtrls[i].wrapped === ctrl) return this.regCtrls[i]; }
            return null;
        },

        VKLExists: function(VKLName, type) {
            var name = VKLName.toLowerCase();
            type = (type.toUpperCase() == VKLTypes.character ? VKLTypes.character : VKLTypes.key);
            for (var i in this.VKLs) { if (this.VKLs[i] !== null && this.VKLs[i].name.toLowerCase() == name && this.VKLs[i].type == type) return this.VKLs[i]; }
            if (this.defaultVKL !== null && this.defaultVKL.name.toLowerCase() == name && this.defaultVKL.type == type) return this.defaultVKL; else return null;
        }
    }
})()


//<script id="VKL">
    function VKL(name, type, nonblocking) {
        this.name = name; this.nonblocking = nonblocking; this.type = type;
    }
    VKL.prototype = {
        name: '', status: Statuses.waiting, language: '', langCode: '', author: '', type: '', nonblocking: false, seq: null,
        parse: function(str) {
            if (String.isNullOrEmpty(str)) {
                this.status = Statuses.unavailable;
                alert("The VKL " + this.name + " is not available.");
                return;
            }

            var xdoc = parseXml(str);
            if (xdoc === null || typeof xdoc !== "object") {
                this.status = Statuses.error;
                // alert((String.isNullOrEmpty(xdoc) ? "" : "Parse error (" + xdoc + ").") + "\nThe string received was:\n" + str);
                alert("error parsing VKL '" + this.name + "'. You should remove and try adding it again.");
                return;
            }

            this.ready(xdoc);
        },

        ready: function(xdoc) {
            try {
                this.xdoc = xdoc.documentElement;
                var props = this.xdoc.getElementsByTagName("props")[0],
lang = props.getElementsByTagName("language")[0];
                this.language = GetText(lang);
                this.langCode = lang.getAttribute("code");
                this.author = GetText(props.getElementsByTagName("author")[0]);
                this.type = (GetText(props.getElementsByTagName("type")[0]).toUpperCase() == VKLTypes.character ? VKLTypes.character : VKLTypes.key);
                this.mappings = { maps: new Array(this.xdoc.getElementsByTagName("mappings")[0].childNodes.length) };
                this.build(this.xdoc.getElementsByTagName("mappings")[0], this.mappings);
                this.status = Statuses.ready;
            } catch (e) {
                alert("Error while preparing VKL: " + e.message);
                this.status = Statuses.error;
            }
        },

        build: function(xparent, map) {
            var el;
            for (var i = 0; i < xparent.childNodes.length; i++) {
                var node = xparent.childNodes[i];
                if (node.nodeType != 1)
                    continue; // To avoid any text nodes to be processed due to indentation.

                if (node.tagName == "map") {
                    var s = (node.getAttribute("shift") == "true" ? "s" : "");
                    if (node.hasChildNodes()) {
                        switch (nodeExists(node, "map", "rule")) {
                            case "rule":
                                el = { text: node.getAttribute("text"), hasRules: true, isSequence: false, rules: [], leftlen: node.getAttribute("rleftlen"), rightlen: node.getAttribute("rrightlen") };
                                this.build(node, el);
                                break;
                            case "map":
                                el = { isSequence: true, maps: new Array(node.childNodes.length) };
                                this.build(node, el);
                                break;
                        }
                    } else el = { text: node.getAttribute("text"), hasRules: false, isSequence: false };
                    map.maps["m" + node.getAttribute("key").toString() + s] = el;
                } else {
                    el = { type: node.getAttribute("type"), dir: node.getAttribute("direction"), text: node.getAttribute("text") };
                    if (el.type == RuleTypes.regexp) {
                        el.pattern = new RegExp(node.getAttribute("pattern"), "g"); el.length = node.getAttribute("len");
                    } else {
                        el.pattern = node.getAttribute("pattern"); el.length = el.pattern.length;
                    }
                    map.rules.push(el);
                }
            }
        },

        map: function(key, chr, shift, aks) {
            if (this.status != Statuses.ready) return;

            var index = "m" + (this.type == VKLTypes.key ? key.toString() : chr.toString()) + ((shift) ? "s" : ""),
            map = (this.seq == null ? this.mappings : this.seq).maps[index];
            if (map != null) {
                if (map.isSequence) {
                    this.seq = map;
                    aks.write("");
                    return;
                } else {
                    if (map.hasRules) {
                        var lt = null, rt = null, mt, i;

                        for (i = 0; i < map.rules.length; i++) {
                            var rule = map.rules[i];
                            if (rule.dir == RuleDirections.left) {
                                if (lt == null)
                                    lt = aks.lcText(map.leftlen);
                                if (lt.length < rule.length)
                                    continue;
                                mt = (lt.length == rule.length ? lt : lt.substring(lt.length, lt.length - rule.length));
                            } else {
                                if (rt == null)
                                    rt = aks.rcText(map.rightlen);
                                if (rt.length < rule.length)
                                    continue;
                                mt = (rt.length == rule.length ? rt : rt.substring(0, rule.length));
                            }

                            if (rule.type == RuleTypes.text) {
                                if (mt == rule.pattern) {
                                    aks.writeAt(rule.dir, rule.text, rule.length);
                                    break;
                                }
                            } else {
                                if (mt.search(rule.pattern) == 0) {
                                    aks.writeAt(rule.dir, rule.text, rule.length);
                                    break;
                                }
                            }

                        }
                        if (i == map.rules.length) // if no match found, write default.
                            aks.write(map.text);

                    } else
                        aks.write(map.text);
                }
            } else
                aks.setAttribute("vklText", '');

            this.seq = null;
        }
    }
//</script>

//<script id="VBC">
    function Aks(ctrl, VKL) {
        this.control = this.wrapped = ctrl; this.VKL = VKL;
        ctrl.setAttribute("vklInput", true);
        switch (this.wrapped.tagName.toLowerCase()) {
            case 'textarea': this.wrappedType = "ta"; break;
            case 'input': if (this.wrapped.type.toLowerCase() == "text") this.wrappedType = "te"; break;
            case 'div': this.wrappedType = "re"; break;
            case 'iframe':
                if (ctrl.contentDocument) {
                    this.wrapped = ctrl.contentDocument; this.wrappedType = "cd"; this.outerWindow = ctrl.contentWindow;
                    // For IE8 issue (see keyEvent)
                    this.wrapped.wrapper = this;
                }
                else {
                    this.wrapped = ctrl.contentWindow.document.body; this.wrappedType = "bd";
                }
                break;
            default: this.wrappedType = 'unknown';
        }
        if (this.wrapped && this.wrapped.setAttribute)
            this.wrapped.setAttribute("vklInput", true);
        bindEvents(this.wrapped, "keydown keypress keyup", this.keyEvent);
    }
    var ltNodes = [], rtNodes = [];
    Aks.prototype = {
        wrapped: null, wrappedType: null, VKL: null, scode: 0, fnKeyPress: null, suspended: false, control: null,
        dispose: function() {
            try {
                unbindEvents(this.wrapped, "keydown keypress keyup", this.keyEvent);
            } catch (e) { }
            this.fnKeyPress = null; this.VKL = null;
            this.wrapped = null; this.control = null;
        },

        keyEvent: function(E) {
            var e = E || window.event, wrapped = e.currentTarget || e.srcElement,
me = VKM.getAks(wrapped);

            // for IE8: contentDocument is now a prop of IFrame, but key event bind to it still have srcElement equals to body
            if (!me) {
                if (!e.srcElement.document.wrapper) {
                    alert("Aks wrapper not found");
                    return;
                }
                me = e.srcElement.document.wrapper;
            }

            // If a client keypress handler is registered, execute it.
            if (me.fnKeyPress && me.fnKeyPress(e) === true) // if handled, do not process.
                return cancelEvent(e, true);

            if (me.suspended || !me.VKL)
                return;

            if (e.altKey || e.ctrlKey)
                return;
            switch (e.type.toLowerCase()) {
                case 'keypress':
                    // To prevent non character generating keypress event to be processed (for firefox).
                    if (e.charCode == 0) return;
                    switch (me.VKL.status) {
                        case Statuses.waiting: case Statuses.error: if (me.VKL.nonblocking) return;
                            break;
                        case Statuses.ready:
                            switch (me.scode) {
                                case 32:
                                    me.write(" ");
                                    break;
                                case 188:
                                    me.write(",");
                                    break;
                                case 190:
                                    me.write(".");
                                    break;
                                case 13:
                                    return;
                                default:
                                    me.VKL.map(me.scode, String.fromCharCode(e.which || e.keyCode), e.shiftKey, me);
                                    break;
                            }
                    }
                    me.setAttribute("keyEnter", "true");
                    return cancelEvent(e, true);
                    break;
                case 'keydown':
                    me.scode = e.which || e.keyCode;
                    //                    me.control.setAttribute("keyEnter", me.scode);
                    //                                        me.wrapped.setAttribute("keyEnter", me.scode);
                    break;
                case 'keyup':
                    me.scode = null;
                    me.removeAttribute("keyEnter");
                    break;
            }
        },

        getIERange: function() {
            switch (this.wrappedType) {
                case "cd":
                    return this.wrapped.selection.createRange();
                    break;
                case "bd":
                    return this.wrapped.document.selection.createRange();
                    break;
                default:
                    return document.selection.createRange();
                    break;
            }
        },

        write: function(text) {
            this.setAttribute("vklText", text);
            if (String.isNullOrEmpty(text)) return;
            var range;

            if (document.selection) {
                range = this.getIERange();
                if (range != null) {
                    range.text = text;
                    range.collapse(false);
                }
            } else {
                try {
                    if ((this.wrappedType == "te" || this.wrappedType == "ta") && this.wrapped.selectionStart) {
                        var selstart = this.wrapped.selectionStart, content = GetText(this.wrapped);
                        if (selstart == 0) setText(this.wrapped, text + content);
                        else if (selstart == (content.length - 1)) setText(this.wrapped, content + text);
                        else setText(this.wrapped, content.substr(0, selstart) + text + content.substr(selstart));
                        this.wrapped.selectionEnd = 0; this.wrapped.selectionStart = selstart + text.length;
                    }

                    else if (window.getSelection()) {
                        range = (this.wrappedType == "cd" ? this.outerWindow.getSelection().getRangeAt(0) : window.getSelection().getRangeAt(0));
                        if (!range.collapsed)
                            range.deleteContents();

                        var txNode = document.createTextNode(text);
                        range.insertNode(txNode);
                        range.setStartAfter(txNode);
                    }
                }
                catch (e) {
                    alert(e.message);
                }
            }
        },

        lcText: function(num) {
            ltNodes = [];
            var range, text = '';
            try {
                if (document.selection) { // IE
                    range = this.getIERange();
                    if (typeof range.text === "string" && range.text.length > 0)
                        return ""; // rule works if and only if no text is selected.

                    range.moveStart("character", -num);
                    text = range.text;
                    range.collapse(false);
                    return text;
                } else {
                    if ((this.wrappedType == "te" || this.wrappedType == "ta") && this.wrapped.selectionStart) {
                        if (this.wrapped.selectionEnd > 0)
                            return "";

                        var si = this.wrapped.selectionStart;
                        if (si >= num) return GetText(this.wrapped).substr(si - num, num);
                        else return GetText(this.wrapped).substr(0, si);

                    } else if (window.getSelection()) {
                        range = (this.wrappedType == "cd" ? this.outerWindow : window).getSelection().getRangeAt(0);
                        if (!range.collapsed)
                            return "";

                        var sn = range.startContainer, so = range.startOffset;
                        if (sn.nodeType == 3) {
                            if (so >= num) {
                                ltNodes.push({ text: sn.textContent.substr(so - num, num), node: sn, offset: so });
                                return ltNodes[0].text;
                            } else if (so > 0) {
                                var str = sn.textContent.substr(0, so);
                                var si = str.search(wb);
                                if (si >= 0) {
                                    ltNodes.push({ text: (si == (so - 1) ? "" : str.substr(si + 1)), node: sn, offset: so });
                                    return ltNodes[0].text;
                                } else {
                                    ltNodes.push({ text: str, node: sn, offset: so });
                                    num -= str.length;
                                }
                            }
                        }
                        else
                            sn = sn.childNodes[so];

                        if (sn.previousSibling)
                            enumLText(sn.previousSibling, num, true);
                        else {
                            var ppnode = getParentPrevSibling(sn);
                            if (ppnode)
                                enumLText(ppnode, num, true);
                        }

                        for (i = ltNodes.length - 1; i >= 0; i--)
                            text += ltNodes[i].text;
                        return text;
                    }
                }
            } catch (exp) {
                alert(exp.message);
            }
            return '';
        },

        rcText: function(num) {
            rtNodes = [];
            var range, text = '';
            try {
                if (document.selection) { // IE
                    range = this.getIERange();
                    if (typeof range.text === "string" && range.text.length > 0)
                        return ""; // rule works if and only if no text is selected.

                    range.moveEnd("character", num);
                    text = range.text;
                    range.collapse(true);
                    return text;
                } else {
                    if ((this.wrappedType == "te" || this.wrappedType == "ta") && this.wrapped.selectionStart) {
                        if (this.wrapped.selectionEnd > 0)
                            return "";

                        return GetText(this.wrapped).substr(this.wrapped.selectionStart, num);
                    } else if (window.getSelection()) {
                        range = (this.wrappedType == "cd" ? this.outerWindow : window).getSelection().getRangeAt(0);
                        if (!range.collapsed)
                            return null;

                        var sn = range.startContainer, so = range.startOffset;
                        if (sn.nodeType == 3) {
                            if ((sn.textContent.length - so) >= num) {
                                rtNodes.push({ text: sn.textContent.substr(so, num), node: sn, offset: so });
                                return rtNodes[0].text;
                            } else if (sn.textContent.length > so) {
                                var str = sn.textContent.substr(so);
                                var si = str.search(wb);
                                if (si >= 0) {
                                    rtNodes.push({ text: (si == 0 ? "" : str.substr(0, si)), node: sn, offset: so });
                                    return rtNodes[0].text;
                                } else {
                                    rtNodes.push({ text: str, node: sn, offset: so });
                                    num -= str.length;
                                }
                            }
                        }
                        else
                            sn = sn.childNodes[so];

                        if (sn.nextSibling)
                            enumRText(sn.nextSibling, num, true);
                        else {
                            var pnNode = getParentNextSibling(sn);
                            if (pnNode)
                                enumRText(pnNode, num, true);
                        }

                        for (i = 0; i < rtNodes.length; i++)
                            text += rtNodes[i].text;
                        return text;
                    }
                }
            } catch (exp) {
                alert(exp.message);
            }
            return '';
        },

        writeAt: function(dir, text, num) {
            // This function must be preceded by a call to either of cText functions, depending on direction.
            this.setAttribute("vklText", text);
            if (typeof text !== "string" || text.length == 0) return;
            var range;
            if (document.selection) {
                range = this.getIERange();
                if (dir == RuleDirections.left) range.moveStart("character", -num);
                else range.moveEnd("character", num);
                range.text = text;
                range.collapse(false);
            } else {
                var si, content, i = 0;
                if (dir == RuleDirections.left) {

                    if (ltNodes.length) {
                        range = (this.wrappedType == "cd" ? this.outerWindow : window).getSelection().getRangeAt(0);
                        if (ltNodes[0].offset) {
                            content = ltNodes[0].node.textContent, si = ltNodes[0].offset;
                            if (si >= num) {
                                ltNodes[0].node.textContent = content.substr(0, si - num) + text + content.substr(si);
                                range.setStart(ltNodes[0].node, (si - num) + text.length)
                                return;
                            } else {
                                ltNodes[0].node.textContent = text + content.substr(si);
                                range.setStart(ltNodes[0].node, text.length);
                                num -= si;
                            }
                            i = 1;
                        }

                        for (; i < ltNodes.length && num > 0; i++) {
                            content = ltNodes[i].node.textContent;
                            if (content.length <= num) {
                                ltNodes[i].node.textContent = ''; num -= content.length;
                            } else {
                                ltNodes[i].node.textContent = content.substring(0, content.length - num);
                                break;
                            }
                        }

                        // absense of an offset means that the carret is on the first character of a text node
                        if (!ltNodes[0].offset) {
                            ltNodes[0].node.textContent = ltNodes[0].node.textContent + text;
                            range.setStartAfter(ltNodes[0].node);
                        }

                    } else {
                        si = this.wrapped.selectionStart, content = GetText(this.wrapped);
                        setText(this.wrapped, content.substr(0, si - num) + text + content.substr(si));
                    }

                } else { // right side

                    if (rtNodes.length) {
                        range = (this.wrappedType == "cd" ? this.outerWindow : window).getSelection().getRangeAt(0);
                        if (rtNodes[0].offset) {
                            content = rtNodes[0].node.textContent, si = rtNodes[0].offset;
                            if ((content.length - si) > num) {
                                rtNodes[0].node.textContent = content.substr(0, si) + text + content.substr(si + num);
                                range.setStart(rtNodes[0].node, si + text.length);
                                return;
                            } else {
                                rtNodes[0].node.textContent = content.substr(0, si) + text;
                                range.setStartAfter(rtNodes[0].node);
                                num -= si;
                            }
                            i = 1;
                        }

                        for (; i < rtNodes.length && num > 0; i++) {
                            content = rtNodes[i].node.textContent;
                            if (content.length <= num) {
                                rtNodes[i].node.textContent = ''; num -= content.length;
                            } else {
                                rtNodes[i].node.textContent = content.substring(content.length, content.length - num);
                                break;
                            }
                        }

                        if (!rtNodes[0].offset) {
                            rtNodes[0].node.textContent = text + rtNodes[0].node.textContent;
                            if (rtNodes[0].node.textContent.length == text.length)
                                range.setStartAfter(rtNodes[0].node);
                            else range.setStartAfter(rtNodes[0].node, text.length);
                        }

                    } else {
                        si = this.wrapped.selectionStart, content = GetText(this.wrapped);
                        setText(this.wrapped, content.substr(0, si) + text + content.substr(si + num));
                    }
                }
            }
        },

        setAttribute: function(attr, value) {
            if (this.control.setAttribute) this.control.setAttribute(attr, value);
            if (this.wrapped.setAttribute) this.wrapped.setAttribute(attr, value);
        },
        removeAttribute: function(attr) {
            if (this.control.removeAttribute) this.control.removeAttribute(attr);
            if (this.wrapped.removeAttribute) this.wrapped.removeAttribute(attr);
        }
    }
//</script>

//<script id="client">
    function registerVKLDefault(VKLName, VKLType, nonblocking) {
        if (VKM.defaultVKL !== null) return;

        VKLType = (VKLType.toUpperCase() == VKLTypes.character ? VKLTypes.character : VKLTypes.key);
        VKM.defaultVKL = VKM.createVKL(VKLName, VKLType, nonblocking);
    }

    function registerTextCtrl(ctrlId, VKLName, VKLType, nonblocking, keyHandler) {
        var ctrl = _(ctrlId);
        // Is it a valid control and is it not already bound?
        if (ctrl == null || VKM.getAks(ctrl)) return false;

        VKLType = (VKLType.toUpperCase() == VKLTypes.character ? VKLTypes.character : VKLTypes.key);

        if (String.isNullOrEmpty(VKLName)) {
            // Bound it with default VKL if one is present.
            if (VKM.defaultVKL != null)
                VKM.bindControl(ctrl, VKM.defaultVKL, keyHandler);
            else
                return false;
        } else {
            // Is the requested VKL already created?
            var isVKL = VKM.VKLExists(VKLName, VKLType);
            if (isVKL != null) {
                VKM.bindControl(ctrl, isVKL, keyHandler);
            } else {
                // Create a new VKL and associate with the control.
                var VKL = VKM.createVKL(VKLName, VKLType, nonblocking);
                VKM.bindControl(ctrl, VKL, keyHandler);
            }
        }
        return true;
    }

//    </script>


    function bindEvents(ctrl, events, fn) {
        if (!ctrl) return;
        var es = events.split(' ');
        for (var e in es) {
            if (ctrl.addEventListener)
                ctrl.addEventListener(es[e], fn, false);
            else if (ctrl.attachEvent)
                ctrl.attachEvent('on' + es[e], fn);
            else
                eval('ctrl.' + es[e] + '= fn;');
        }
    }

        function unbindEvents(ctrl, events, fn) {
        if (!ctrl) return;
        var es = events.split(' ');
        for (var e in es) {
            if (ctrl.removeEventListener)
                ctrl.removeEventListener(es[e], fn, false);
            else if (ctrl.detachEvent)
                ctrl.detachEvent('on' + es[e], fn);
            else
                eval('ctrl.' + es[e] + '= null;');
        }
    }

    function setText(elem, text) {
        if (typeof elem.value !== "undefined") elem.value = text;
                else if (typeof elem.textContent !== "undefined") elem.textContent = text;
        else if (typeof elem.innerText !== "undefined") elem.innerText = text;
        else if (typeof elem.text !== "undefined") elem.text = text;
        else if (typeof elem.nodeValue !== "undefined") elem.nodeValue = text;
    }

    function GetText(elem) {
        if (typeof elem.value !== "undefined") return elem.value;
        else if (typeof elem.text !== "undefined") return elem.text;
        else if (typeof elem.textContent !== "undefined") return elem.textContent;
        else if (typeof elem.innerText !== "undefined") return elem.innerText;
        else if (typeof elem.nodeValue !== "undefined") return elem.nodeValue;
            }

    function firstElement(parent) {
        var i;
        for (i = 0; i < parent.childNodes.length;i++)
            if (parent.childNodes[i].nodeType == 1)
            return parent.childNodes[i];

return null;
}

function nodeExists(parent, t1, t2) {
    for (i = 0; i < parent.childNodes.length; i++)
        if (parent.childNodes[i].tagName == t1)
        return t1;
    else if (parent.childNodes[i].tagName == t2)
        return t2;

    return "";
}

function getParentPrevSibling(node) {
    if (node.parentNode.tagName != "BODY") { if (node.parentNode.previousSibling) return node.parentNode.previousSibling; else return getParentPrevSibling(node.parentNode); } else return null;
    }

function enumLText(node, num, trvsup) {
    var fnum = 0;
    if (num <= 0) return 0;
    // if (!node) return;

    switch (node.nodeType) {
        case 3:
            if (node.length >= num) {
                ltNodes.push({ text: node.textContent.substring(node.length, node.length - num), node: node });
                // ltNodes.push("\n");
                return num;
            } else {
                var str = node.textContent, si = str.search(wb);
                if (si >= 0) {
                    ltNodes.push({ text: (si == (str.length - 1) ? "" : str.substr(si + 1)), node: node });
                    // ltNodes.push("\n");
                    return num;
                } else {
                    ltNodes.push({ text: str, node: node });
                    fnum += str.length;
                }
            }
            break;
        case 1:
            if (node.tagName == "BR") {
                // ltNodes.push("\n");
                return num;
            } else {
                for (var i = node.childNodes.length - 1; i >= 0; i--) {
                    fnum += enumLText(node.childNodes[i], num - fnum, false);
                    if (fnum >= num)
                        return fnum;
                    // if (typeof ltNodes[ltNodes.length - 1] === "string")
                    // return fnum;
                }}}

    if (trvsup) {
        if (node.previousSibling)
            fnum += enumLText(node.previousSibling, num - fnum, true);
        else {
            var ppnode = getParentPrevSibling(node);
            if (ppnode)
                fnum += enumLText(ppnode, num - fnum, true);
        }
    }
    return fnum;
}

function enumRText(node, num, trvsup) {
    var fnum = 0;
    if (num <= 0) return 0;
    // if (!node) return;

    switch (node.nodeType) {
        case 3:
            if (node.length >= num) {
                rtNodes.push({ text: node.textContent.substr(0, num), node: node });
                // rtNodes.push("\n");
                return num;
            } else {
                var str = node.textContent, si = str.search(wb);
                if (si >= 0) {
                    rtNodes.push({ text: (si == 0 ? "" : str.substr(0, si)), node: node });
                    // rtNodes.push("\n");
                    return num;
                } else {
                    rtNodes.push({ text: str, node: node });
                    fnum += str.length;
                }
            }
            break;
        case 1:
            if (node.tagName == "BR") {
                // rtNodes.push("\n");
                return num;
            } else {
                for (var i = 0; i < node.childNodes.length; i++) {
                    fnum += enumRText(node.childNodes[i], num - fnum, false);
                    if (fnum >= num)
                        return fnum;
                    // if (typeof rtNodes[rtNodes.length - 1] === "string")
                    // return fnum;
                }}}

    if (trvsup) {
        if (node.nextSibling)
            fnum += enumRText(node.nextSibling, num - fnum, true);
        else {
            var pnNode = getParentNextSibling(node);
            if (pnNode)
                fnum += enumRText(pnNode, num - fnum, true);
        }
    }
    return fnum;
}

function getParentNextSibling(node) {
    if (node.parentNode.tagName != "BODY") { if (node.parentNode.nextSibling) return node.parentNode.nextSibling; else return getParentNextSibling(node.parentNode); } else return null;
}

function getXHR() {
        var xhr = window.ActiveXObject ? new ActiveXObject("Microsoft.XMLHTTP") : new XMLHttpRequest();
        if (xhr.readyState == null) {
                        xhr.readyState = 1;
            xhr.addEventListener("load",
                                    function() {
                                        xhr.readyState = 4;
                                        if (typeof xhr.onreadystatechange === "function")
                                            xhr.onreadystatechange();
                                    },
                                    false);
        }
        return xhr;
    }

    String.isNullOrEmpty = function(val) {
        if (typeof val !== "string" || val === null || val === "") return true; else return false;
    }

    function parseXml(xml) {
        var xDoc = null;
        try {
            xDoc = new ActiveXObject("Microsoft.XMLDOM");
            xDoc.async = false;

            try {
                xDoc.loadXML(xml);
            } catch (e) {
                return e.message;
            }

        } catch (e) {
            try {
                parser = new DOMParser();

                try {
                    xDoc = parser.parseFromString(xml, "text/xml");
                } catch (e) {
                    return e.message;
                }

            } catch (e) { }
        }
        return xDoc;
    }

    function _(id) { return document.getElementById(id); }
    function cancelEvent(e, stopPropagation) {
        e.returnValue = false;
        if (e.preventDefault)
            e.preventDefault();

        if (stopPropagation) {
            e.cancelBubble = true;
            if (e.stopPropagation)
                e.stopPropagation();
        }
        return false;
    }
