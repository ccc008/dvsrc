using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace apfsplitter {
    struct ContentDeleteInfo {
        public ContentDeleteInfo(String filesMask, String startMacros, String endMacros) {
            this.FilesMask = filesMask;
            this.StartMacros = startMacros;
            this.EndMacros = endMacros;
        }
        public readonly String FilesMask;
        public readonly String StartMacros;
        public readonly String EndMacros;

        public bool IsFileApplicable(String srcFileName) {
            return true; //!TODO
        }
    }
}
