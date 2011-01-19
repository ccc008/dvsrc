using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace apfsplitter {
    public enum treplace_kinds {
        REGEX
        , DIRECT
        , XML
    }
    struct ContentReplaceInfo {
        private Regex m_RegExp;
        private Dictionary<String, String> m_AdditionalParams;

        public ContentReplaceInfo(String filesMask, String searchString, String replaceString, treplace_kinds kindReplace) {
            m_RegExp = new Regex(Utils.FileMask2Regexp(filesMask), RegexOptions.IgnoreCase);
            m_AdditionalParams = new Dictionary<String, String>();
            this.SearchString = searchString;
            this.ReplaceString = replaceString;
            this.Kind = kindReplace;
        }
        public readonly String SearchString;
        public readonly String ReplaceString;
        public readonly treplace_kinds Kind;        

        public bool IsFileApplicable(String srcFileName) {
            return m_RegExp.Match(srcFileName).Success;
        }

        public void SetAdditionalParam(String paramName, String paramValue) {
            m_AdditionalParams[paramName] = paramValue;
        }

        public String GetParam(String paramName) {
            if (!m_AdditionalParams.ContainsKey(paramName)) return null;
            if (m_AdditionalParams == null) return null;
            return m_AdditionalParams[paramName];
        }
    }
}
