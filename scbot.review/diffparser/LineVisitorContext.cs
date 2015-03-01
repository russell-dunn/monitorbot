using System;

namespace scbot.review.diffparser
{
    class LineVisitorContext : IDiffLineVisitor
    {
        public string CurrentOldFile = null;
        public string CurrentNewFile = null;
        public string CurrentContext = null;
        public string CurrentFunctionName = null;
        public int CurrentNewFileLineNumber = -1;
        public string CurrentLineText = null;
        private int StartOfRemovedLines = -1;
        private int StartOfAddedLines = -1;

        public virtual void Visit(OldFile file)
        {
            CurrentOldFile = file.Line;
        }

        public virtual void Visit(ContextLine line)
        {
            CurrentLineText = line.Line;
            CurrentNewFileLineNumber++;
        }

        public virtual void Visit(RemovedLine line)
        {
            CurrentLineText = line.Line;
            CurrentNewFileLineNumber++;
            if (StartOfRemovedLines == -1) StartOfRemovedLines = CurrentNewFileLineNumber;
        }

        public virtual void Visit(AddedLine line)
        {
            CurrentLineText = line.Line;
            if (StartOfAddedLines == -1 && StartOfRemovedLines != -1)
            {
                CurrentNewFileLineNumber = StartOfRemovedLines;
                StartOfAddedLines = CurrentNewFileLineNumber;
            }
            else
            {
                CurrentNewFileLineNumber++;
            }
        }

        public virtual void Visit(NewFile file)
        {
            CurrentNewFile = file.Line.Trim();
        }

        public virtual void Visit(ChunkHeader header)
        {
            var parts = header.Line.Split(new[] { ' ' }, 4);
            var oldRange = parts[0];
            var newRange = parts[1].Substring(1);
            var contextLine = parts.Length == 4 ? parts[3] : "";
            CurrentContext = contextLine;
            CurrentFunctionName = GetFunctionName(contextLine);
            CurrentNewFileLineNumber = GetCurrentLineNumber(newRange);
            CurrentLineText = null;
            StartOfRemovedLines = -1;
            StartOfAddedLines = -1;
        }

        private static int GetCurrentLineNumber(string newRange)
        {
            return Int32.Parse(newRange.Substring(0, newRange.IndexOf(','))) - 1;
        }

        private static string GetFunctionName(string contextLine)
        {
            var firstParen = contextLine.IndexOf('(');
            if (firstParen == -1) return null;
            var spaceBeforeParen = contextLine.LastIndexOf(' ', firstParen) + 1;
            if (spaceBeforeParen == -1) spaceBeforeParen = 0;
            return contextLine.Substring(spaceBeforeParen, firstParen - spaceBeforeParen);
        }

        public virtual void Visit(GitDiffHeader header)
        {
            CurrentOldFile = CurrentNewFile = null;
            CurrentContext = CurrentFunctionName = null;
            CurrentNewFileLineNumber = -1;
            CurrentLineText = null;
        }
    }
}
