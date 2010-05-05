contextHelp = false;
// null, undefined, 0 and false are equivalent if used in a condition: such condition always evaluates to false
// If a function argument is omitted, it evaluates to undefined (one can also use arguments.length to exactly infer the number of arguments passed, and by comparing it with funcname.length, we can determine exactly as to how many number of arguments have been omitted.)

function _(id) { return document.getElementById(id); }
    function isDefined(val) { if (val == undefined) return false; else return true; }
    function isNull(val) { if (val == null) return true; else return false; }
    function isString(val) { if (typeof val == "string") return true; else return false; }
    function isFunction(val) { if (typeof val == "function") return true; else return false; }
    function isObject(val) { if (typeof (val) == "object") return true; else return false; }
    String.isNullOrEmpty = function(val) {
        // if not string, also return true!
    if (typeof val !== "string" || val === null || val === "") return true; else return false;
    }
    String.prototype.startsWith = function(s, caseSensitive) {
        if (String.isNullOrEmpty(s) || s.length > this.length)
            return false;

        var start = this.substr(0, s.length);
                if (start == s)
            return true;
        else if (caseSensitive && start.toUpperCase() == s.toUpperCase())
            return true;
        else
            return false;
    }
    String.prototype.toUnicode = function(delimiter) {
        var strcode = '';
        for (i = 0; i < this.length; i++) {
            if (delimiter)
                strcode += (i == 0 ? '' : delimiter) + new String(this.charCodeAt(i));
            else
                strcode += new String(this.charCodeAt(i));
        }
        return strcode;
    }

    function setText(ctrl, text) {
        if (typeof ctrl.value !== "undefined") ctrl.value = text;
                else if (typeof ctrl.textContent !== "undefined") ctrl.textContent = text;
        else if (typeof ctrl.innerText !== "undefined") ctrl.innerText = text;
        else if (typeof ctrl.nodeValue !== "undefined") ctrl.nodeValue = text;
        else if (typeof ctrl.text !== "undefined") ctrl.text = text;
    }

    function setColExp(ctrl, title, img) {
        ctrl.setAttribute("titke", title);
        ctrl.setAttribute("alt", title);
        ctrl.setAttribute("src", img);
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

    function makeVisible(o, visible) {
        if (visible)
            o.style.display = "";
        else
            o.style.display = "none";
    }
    function isDisabled(ctrl) {
        return String.isNullOrEmpty(ctrl.getAttribute("disabled")) ? false : true;
    }
    function isVisible(ctrl) {
        if (ctrl.style.display == "none" || ctrl.style.visibility == "hidden")
            return false;
        else
            return true;
    }

    function isDisplayed(ctrl) {
        if (ctrl.style.display == "none") return false;
        else return true;
    }

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




    (function() {
        var vtreq = 1, vtrng = 2, vtreg = 3;
        var valinfos = new Array(0);

        function validateRange(ctrl, ectrl) {
            if (!(ctrl.value.length < this.min || ctrl.value.length > this.max)) {
                setText(ectrl, "");
                return true;
            }
            if (String.isNullOrEmpty(this.msg)) setText(ectrl, this.name + "'s length is out of specified range.");
            else setText(ectrl, this.msg);
            return false;
        }

        function validateRequired(ctrl, ectrl) {
            if (!String.isNullOrEmpty(ctrl.value)) {
                setText(ectrl, "");
                return true;
            }
            if (String.isNullOrEmpty(this.msg)) setText(ectrl, this.name + " is required.");
            else setText(ectrl, this.msg);
            return false;
        }

        function validateRegex(ctrl, ectrl) {
            if (this.regex.test(ctrl.value)) {
                setText(ectrl, "");
                return true;
            }
            if (String.isNullOrEmpty(this.msg)) setText(ectrl, this.name + " is in invalid format.");
            else setText(ectrl, this.msg);
            return false;
        }

        function validateCustom(ctrl, ectrl) {
            if (this.test(ctrl, this.args)) {
                setText(ectrl, "");
                return true;
            }
            setText(ectrl, this.msg);
            return false;
        }

        function validate(e) {
            var rt = null, rf = null;
            var vi = (isDefined(e.data) ? e.data : (rt = true, rf = false, e));
            if (isDefined(vi.req) && !vi.req.validate(vi.ctrl, vi.ectrl))
                return rf;

            if (isDefined(vi.rng) && !vi.rng.validate(vi.ctrl, vi.ectrl))
                return rf;

            if (isDefined(vi.reg) && !vi.reg.validate(vi.ctrl, vi.ectrl))
                return rf;

            if (isDefined(vi.cus) && !vi.cus.validate(vi.ctrl, vi.ectrl))
                return rf;
            return rt;
        }

        validator = {
            setRange: function(ctrl, min, max, ectrl, msg, name) {
                if (String.isNullOrEmpty(msg)) msg = new String();
                if (String.isNullOrEmpty(name)) name = new String();
                valinfos[initCtrl(ctrl, ectrl)].rng = {
                    min: min, max: max,
                    msg: msg, name: name, validate: validateRange
                };
            }, // setRange

            setRequired: function(ctrl, ectrl, msg, name) {
                if (String.isNullOrEmpty(msg)) msg = new String();
                if (String.isNullOrEmpty(name)) name = new String();
                valinfos[initCtrl(ctrl, ectrl)].req = {
                    msg: msg, name: name, validate: validateRequired
                };

            }, // setRequired
            setRegexp: function(ctrl, ptrn, flags, ectrl, msg, name) {
                if (String.isNullOrEmpty(msg)) msg = new String();
                if (String.isNullOrEmpty(name)) name = new String();
                var re;
                if (ptrn.test) // Does pattern represent an actual regular expression object?
                    re = ptrn;
                else // ptrn is supposed to be string pattern, compile it.
                    re = new RegExp().compile(ptrn, flags);

                valinfos[initCtrl(ctrl, ectrl)].reg = { regex: re,
                    msg: msg, name: name, validate: validateRegex
                };
            }, // setRegexp
            setCustom: function(ctrl, fn, ectrl, msg, args) {
                if (String.isNullOrEmpty(msg)) msg = new String("Invalid Input");
                valinfos[initCtrl(ctrl, ectrl)].cus = { test: fn, args: args,
                    msg: msg, validate: validateCustom
                };
            }, // setCustom

            isOk: function() {
                for (var i in valinfos)
                    if (!validate(valinfos[i]))
                    return false;

                return true;
            }, // isOk
            regEmail: /\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*/
        }

        function existed(ctrl) {
            for (i = 0; i < valinfos.length; i++)
                if (valinfos[i].ctrl == ctrl)
                return i;
            return -1;
        }
        function initCtrl(ctrl, ectrl) {
            var c = _(ctrl), index = existed(c);
            if (index == -1) {
                index = (valinfos.push({ ctrl: c, ectrl: _(ectrl) }) - 1);
                $(c).bind("blur", valinfos[index], validate);
                // bindEvents(c, "change", validate);
            }
            return index;
        }

    })();

    $(document).ready(function() {
        if (contextHelp) {
            $("img.cs-chbut").click(function() {
                chbClicked(this);
            }).hover(function() {
                if (!this.tog)
                    makeVisible(this.chelp, true);
            }, function() {
                if (!this.tog)
                    makeVisible(this.chelp, false);
            });

            function chbClicked(chb, show) {
                var chel = chb.chelp || $(".chelp", chb.parentElement)[0];
                chb.chelp = chel;
                show = (typeof show !== "undefined" ? show : !(chb.tog));
                if (show) {
                    makeVisible(chel, true);
                    setColExp(chb, "Collapse", "images/collapse.gif");
                } else {
                    makeVisible(chel, false);
                    setColExp(chb, "Expand", "images/expand.gif");
                }
                chb.tog = show;
            }

            $("#chAll").click(function(e) {
                var show = !(typeof this.tog !== "undefined" ? this.tog : this.attributes.getNamedItem("tog").nodeValue == 1);
                $("img.cs-chbut").each(function() {
                    chbClicked(this, show);
                });
                setText(this, (show ? "Collapse All" : "Expand All"));
                this.tog = show;
            }).trigger("click");
        }
    });      // ready

    function showMD(url, args) {
        if (window.showModalDialog) {
            window.showModalDialog(url, args, "resizable=no,status=no,scroll=no");
        }
        else if (window.openDialog) {
            window.openDialog(url, "addvkl", "modal,resizable=no,status=no,scrollbars=no", args);
        }
    }
