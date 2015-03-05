using System;
using System.Collections.Generic;
using NUnit.Framework;
using scbot.review.diffparser;
using System.Diagnostics;

namespace scbot.review.tests
{
    class DiffParsingTests
    {
        private void Log(string str) { Trace.WriteLine(str); }

        #region test diffs

        private const string c_ExampleDiff = @"diff --git a/data/Sweetiebot.sass b/data/Sweetiebot.sass
index 3b9cc06..bdbb5fb 100644
--- a/data/Sweetiebot.sass
+++ b/data/Sweetiebot.sass
@@ -163,3 +163,4 @@ well butterscotch loves me and isn't that what counts?
 y'all silly people need Celestia
 y'all silly people need jesus
 Мой корабль полон угрей!
+*takes out one earbud* ""not guilty, your honor""
diff --git a/modules/SweetieLookup.py b/modules/SweetieLookup.py
index de9cf17..53a6760 100644
--- a/modules/SweetieLookup.py
+++ b/modules/SweetieLookup.py
@@ -253,7 +253,7 @@ def random_reddit_link(self, subreddit, domain_filter=None):
         link = choice['data']['url']
         text = choice['data']['title']
         html = '<a href=""{}"">{}</a>'.format(link, text)
-        plain = '{} [{}]'.format(text, link)
+        plain = '{} [ {} ]'.format(text, link)
         return MessageResponse(plain, None, html=html)
 
     @botcmd
@@ -280,7 +280,7 @@ def woon(self, message):
         text = random.choice(link_title_data)['data']['body']
         text = re.split('\.|!|\?', text)[0]
         html = '<a href=""{}"">{}</a>'.format(link, text)
-        plain = '{} [{}]'.format(text, link)
+        plain = '{} [ {} ]'.format(text, link)
         return MessageResponse(plain, None, html=html)
 
     def get_children_of_type(self, reddit_data, kind):";

        private const string c_AddedFile = @"diff --git a/scbot.review/Extensions.cs b/scbot.review/Extensions.cs
new file mode 100644
index 0000000..4e89371
--- /dev/null
+++ b/scbot.review/Extensions.cs
@@ -0,0 +1,22 @@
+using scbot.review.diffparser;
+using scbot.review.reviewer;
+using System;
+using System.Collections.Generic;
+using System.Linq;
+using System.Text;
+using System.Threading.Tasks;
+
+namespace scbot.review
+{
+    public static class Extensions
+    {
+        internal static IEnumerable<DiffComment> Review(this IDiffReviewer reviewer, List<DiffLine> diffLines)
+        {
+            foreach (var line in diffLines)
+            {
+                line.Accept(reviewer);
+            }
+            return reviewer.Comments;
+        }
+    }
+}";

        #endregion

        [Test]
        public void CanParseDiff()
        {
            List<DiffLine> result = DiffParser.ParseDiff(c_ExampleDiff);

            foreach (var l in result) { Log(l.ToString()); }
        }
        
        [Test]
        public void CanVisitDiff()
        {
            List<DiffLine> result = DiffParser.ParseDiff(c_ExampleDiff);

            var visitor = new LineVisitorContext();
            foreach (var l in result)
            {
                l.Accept(visitor);
                Trace.WriteLine(string.Format("{0} {1} {2}", visitor.CurrentNewFile, visitor.CurrentNewFileLineNumber, visitor.CurrentLineText ?? visitor.CurrentFunctionName ?? visitor.CurrentContext));
            }
        }

        [Test]
        public void CanParseAddedFile()
        {
            List<DiffLine> result = DiffParser.ParseDiff(c_AddedFile);

            var visitor = new LineVisitorContext();
            foreach (var l in result)
            {
                l.Accept(visitor);
                Trace.WriteLine(string.Format("{0} {1} {2}", visitor.CurrentNewFile, visitor.CurrentNewFileLineNumber, visitor.CurrentLineText ?? visitor.CurrentFunctionName ?? visitor.CurrentContext));
            }

            Assert.AreEqual(22, visitor.CurrentNewFileLineNumber);
        }
    }
}
