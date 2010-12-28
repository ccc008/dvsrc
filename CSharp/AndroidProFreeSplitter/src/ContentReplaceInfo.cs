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
        public ContentReplaceInfo(String filesMask, String searchString, String replaceString, treplace_kinds kindReplace) {
            m_RegExp = new Regex(Utils.FileMask2Regexp(filesMask), RegexOptions.IgnoreCase);
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

    }
}
