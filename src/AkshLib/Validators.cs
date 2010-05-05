using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Text.RegularExpressions;

namespace Akshar.Lib.Validators
    {
        public sealed class Validator
        {

            public const string REGEMAIL = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";

            private Ctrl[] cs;
            private bool writeTo = false;
            private List<string> msgs;

            public void Set(Ctrl[] cs)
            {
                foreach (var c in cs)
                {
                    if (c is RequiredCtrl)
                        c.Validate = ValidateRequired;
                    else if (c is RangeCtrl)
                        c.Validate = ValidateRange;
                    else if (c is RegexCtrl)
                        c.Validate = validateRegex;
                    else if (c is CustomCtrl)
                        c.Validate = validateCustom;
                }
                this.cs = cs;
                msgs = new List<string>(cs.Length);
            }

            public bool IsValid(out string msg)
            {
                this.msgs.Clear();
                foreach (var c in cs)
                {
                    if (!c.Validate(c))
                    {
                        msg = msgs.First();
                        return false;
                    }
                }
                msg = string.Empty;
                return true;
            }

            public bool ValidateAll(out List<string> msgs)
            {
                this.msgs.Clear();
                foreach (var c in cs)
                {
                    c.Validate(c);
                }
                msgs = this.msgs;
                if (msgs.Count > 0)
                    return true;
                else
                    return false;
            }

            private bool ValidateRange(Ctrl ctrl)
            {
                var c = (RangeCtrl)ctrl;
                string val = GetText(c.source);
                if (!(val.Length < c.min || val.Length > c.max))
                {
                    SetText(c.target, string.Empty);
                    return true;
                }
                if (string.IsNullOrEmpty(c.msg)) SetText(c.target, c.name + "'s length is out of specified range.");
                else SetText(c.target, c.msg);
                return false;
            }

            private bool ValidateRequired(Ctrl ctrl)
            {
                var c = (RequiredCtrl)ctrl;
                string val = GetText(c.source);
                if (!string.IsNullOrEmpty(val))
                {
                    SetText(c.target, string.Empty);
                    return true;
                }
                if (string.IsNullOrEmpty(c.msg)) SetText(c.target, c.name + " is required.");
                else SetText(c.target, c.msg);
                return false;
            }

            private bool validateRegex(Ctrl ctrl)
            {
                var c = (RegexCtrl)ctrl;
                string val = GetText(c.source);
                if (Regex.IsMatch(c.Regexp, val))
                {
                    SetText(c.target, string.Empty);
                    return true;
                }
                if (string.IsNullOrEmpty(c.msg)) SetText(c.target, c.name + " is in invalid format.");
                else SetText(c.target, c.msg);
                return false;
            }

            private bool validateCustom(Ctrl ctrl)
            {
                var c = (CustomCtrl)ctrl;
                if (c.test(c))
                {
                    SetText(c.target, string.Empty);
                    return true;
                }
                SetText(c.target, c.msg);
                return false;
            }


            private string GetText(Control c)
            {
                if (c is TextBox)
                    return ((TextBox)c).Text;

                return string.Empty;
            }

            private void SetText(Control c, string value)
            {
                if (!string.IsNullOrEmpty(value))
                    msgs.Add(value);

                if (!writeTo)
                    return;

                if (c is TextBox)
                    ((TextBox)c).Text = value;
                else if (c is Label)
                    ((Label)c).Text = value;
            }

        }

        #region child classes
        public delegate bool Validate(Ctrl c);

        public abstract class Ctrl
        {
            public Control source, target;
            public string msg = string.Empty, name = string.Empty;

            public Validate Validate;

            public abstract string GetClientValidateCall();
        }

        public sealed class RequiredCtrl : Ctrl
        {

            public override string GetClientValidateCall()
            {
                return string.Format(@"validator.setRequired('{0}', '{1}', '{2}', '{3}');", source.ClientID, target.ClientID, msg, name);
            }
        }

        public sealed class RangeCtrl : Ctrl
        {
            public int min, max;

            public override string GetClientValidateCall()
            {
                return string.Format(@"validator.setRange('{0}', {1}, {2}, '{3}', '{4}', '{5}');", source.ClientID, min, max, target.ClientID, msg, name);
            }
        }

        public sealed class RegexCtrl : Ctrl
        {
            public string Regexp = string.Empty, flags = string.Empty;

            public override string GetClientValidateCall()
            {
                return string.Format(@"validator.setRegexp('{0}', {1}, '{2}', '{3}', '{4}', '{5}');", source.ClientID, GetRegexp(), flags, target.ClientID, msg, name);
            }

            private string GetRegexp()
            {
                switch (Regexp)
                {
                    case Validator.REGEMAIL:
                        return "validator.regEmail";
                    default:
                        return "/" + Regexp + "/";
                }
            }

        }

        public sealed class CustomCtrl : Ctrl
        {
            public delegate bool CustomHandler(CustomCtrl c);

            public object o;
            public CustomHandler test;
            public string clientHandler = string.Empty;

            public override string GetClientValidateCall()
            {
                return string.Empty;
            }


        }
        #endregion
    }
