using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraSharp
{
    public static class ExtraPath
    {
        public static string MakeFileRelated(string res, string sub)
        {
            res = Path.GetFullPath(res);
            sub = Path.GetFullPath(sub);
            if (res == sub) return Path.GetFileName(sub);

            var sep = Path.DirectorySeparatorChar;
            var len = 0;
            while (len < res.Length && len < sub.Length && res[len] == sub[len]) len++;
            while (len > 0 && res[len - 1] != sep) len--;
            res = res.Remove(0, len);
            sub = sub.Remove(0, len);
            string result = "";
            foreach (var c in res)
                if (c == sep) result += ".." + sep;
            return result + sub;
        }
        public static string MakeDirectoryRelated(string dir, string sub)
        {
            dir = Path.GetFullPath(dir);
            sub = Path.GetFullPath(sub);
            if (dir == sub) return ".";

            var sep = Path.DirectorySeparatorChar;
            var len = 0;
            while (len < dir.Length && len < sub.Length && dir[len] == sub[len]) len++;
            dir = dir.Remove(0, len);
            sub = sub.Remove(0, len);

            if (sub.Length == 0) return "";
            if (sub[0] == sep) return sub.Remove(0, 1);
            
            string result = "";
            foreach (var c in dir)
                if (c == sep) result += ".." + sep;

            return result + sub;
        }
    }
}
