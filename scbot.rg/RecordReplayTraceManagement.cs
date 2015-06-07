using scbot.core.bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using scbot.core.utils;
using System.IO;
using System.Diagnostics;

namespace scbot.rg
{
    public class RecordReplayTraceManagement : RegexCommandMessageProcessor
    {
        public static IFeature Create(ICommandParser commandParser)
        {
            return new BasicFeature("recordreplay", "delete record/replay traces for a branch", "use `delete traces for <branch>` to force everything to be regenerated", new RecordReplayTraceManagement(commandParser));
        }

        private const string c_RecordReplayBase = @"\\sqlcomparetestdata.red-gate.com\sqlcomparetestdata\RecordReplay\";

        public RecordReplayTraceManagement(ICommandParser commandParser)
            : base(commandParser, Commands)
        {
        }

        public static Dictionary<Regex, MessageHandler> Commands
        {
            get
            {
                return new Dictionary<Regex, MessageHandler>
                {
                    { new Regex(@"delete traces for (?<branch>[^ ]+)"), DeleteTracesFor },
                    //{ new Regex(@"init[^ ]* traces for (?<branch>[^ ]+)"), InitTracesFor },
                };
            }
        }

        private static MessageResult DeleteTracesFor(Message message, Match args)
        {
            var branch = args.Group("branch");
            var path = PathForBranch(branch);
            Trace.TraceInformation("DeleteTracesFor " + path);
            try
            {
                Directory.Delete(path, true);
                return Response.ToMessage(message, "Deleted " + path);
            }
            catch (Exception e)
            {
                return Response.ToMessage(message, "Failed to delete " + path + ": " + e.Message);
            }
        }

        private static MessageResult InitTracesFor(Message message, Match args)
        {
            var branch = args.Group("branch");
            var path = PathForBranch(branch);
            Trace.TraceInformation("InitTracesFor " + path);
            try
            {
                CopyDirectory(PathForBranch("master"), path, true, true);
                return Response.ToMessage(message, "Copied traces from master into " + path);
            }
            catch (Exception e)
            {
                return Response.ToMessage(message, "Failed to copy traces to " + path + ": " + e.Message);
            };
        }

        private static string PathForBranch(string branch)
        {
            return SanitizeSlashesInBranch(Path.Combine(c_RecordReplayBase, branch));
        }

        private static string SanitizeSlashesInBranch(string path)
        {
            return Path.GetFullPath(path);
        }

        // https://msdn.microsoft.com/en-us/library/bb762914
        // How to: Copy Directories (sigh)
        private static void CopyDirectory(string sourceDirName, string destDirName, bool copySubDirs, bool overwrite)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }
            else if (overwrite)
            {
                Directory.Delete(destDirName, true);
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, overwrite);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    CopyDirectory(subdir.FullName, temppath, copySubDirs, overwrite);
                }
            }
        }

    }
}
