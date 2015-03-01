using System.Collections.Generic;

namespace scbot.review.diffparser
{
    static class DiffParser
    {
        private static void Log(string str)
        {
            // Console.WriteLine(str);
        }

        public static List<DiffLine> ParseDiff(string diff)
        {
            var lines = diff.Split('\n');
            var result = new List<DiffLine>(lines.Length);
            for (int line = 0; line < lines.Length;)
            {
                var lineStr = lines[line];
                if (lineStr.Length == 0)
                {
                    line++;
                    continue;
                }
                switch (lineStr[0])
                {
                    case 'd': ParseHeader(result, lines, ref line); break;
                    case '@': ParseChunk(result, lines, ref line); break;
                    default: Log("unknown " + lineStr); line++; break;
                }
            }

            return result;
        }

        private static void ParseHeader(List<DiffLine> result, string[] lines, ref int line)
        {
            result.Add(new GitDiffHeader(lines[line]));
            line++;
            while (line < lines.Length)
            {
                var lineStr = lines[line];
                switch (lineStr[0])
                {
                    case 'i': Log("index " + lineStr); break;
                    case 'm': Log("mode " + lineStr); break;
                    case 'n': Log("new file " + lineStr); break;
                    case 'd': Log("deleted " + lineStr); break;
                    case 's': Log("similarity " + lineStr); break;
                    case 'r': Log("renamed " + lineStr); break;
                    case '-': result.Add(new OldFile(lineStr)); break;
                    case '+': result.Add(new NewFile(lineStr)); break;
                    default: return;
                }
                line++;
            }
        }

        private static void ParseChunk(List<DiffLine> result, string[] lines, ref int line)
        {
            result.Add(new ChunkHeader(lines[line]));
            line++;
            while (line < lines.Length)
            {
                var lineStr = lines[line];
                if (lineStr.Length == 0) return;
                switch (lineStr[0])
                {
                    case ' ': result.Add(new ContextLine(lineStr)); break;
                    case '+': result.Add(new AddedLine(lineStr)); break;
                    case '-': result.Add(new RemovedLine(lineStr)); break;
                    default: return;
                }
                line++;
            }
        }

        
    }
}
