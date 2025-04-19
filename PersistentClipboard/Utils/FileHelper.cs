using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PersistentClipboard.Utils
{
    public static class FileHelper
    {
        public static void ClearFiles(string folderPath, IEnumerable<string> filesToKeep = null)
        {
            string[] files = Directory.GetFiles(folderPath);
            
            if (filesToKeep == null)
            {
                filesToKeep = new List<string>();
            }

            foreach (string file in files)
            {
                if (!filesToKeep.Contains(file))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch { }
                }
            }
        }
    }
}
