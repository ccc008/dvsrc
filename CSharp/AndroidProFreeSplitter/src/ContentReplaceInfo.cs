using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace apfsplitter {
    struct ContentReplaceInfo {
        public ContentReplaceInfo(String filesMask, String searchString, String replaceString) {
            this.FilesMask = filesMask;
            this.SearchString = searchString;
            this.ReplaceString = replaceString;
        }
        public readonly String FilesMask;
        public readonly String SearchString;
        public readonly String ReplaceString;

        public bool IsFileApplicable(String srcFileName) {
            return true; //!TODO
        }

    }
}
