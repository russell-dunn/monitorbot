using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.review.services
{
    class GithubReference
    {
        public readonly string Branch;
        public readonly string Commit;
        public readonly int Issue;
        public readonly string Repo;
        public readonly string User;
        public readonly string BaseBranch;

        private GithubReference(string user, string repo, string commit, string branch, string baseBranch, int issue)
        {
            User = user;
            Repo = repo;
            Commit = commit;
            Branch = branch;
            BaseBranch = baseBranch;
            Issue = issue;
        }

        public static GithubReference FromCommit(string user, string repo, string commit)
        {
            return new GithubReference(user, repo, commit, null, null, -1);
        }

        public static GithubReference FromBranch(string user, string repo, string branch)
        {
            return new GithubReference(user, repo, null, branch, null, -1);
        }

        public static GithubReference FromComparison(string user, string repo, string branch, string baseBranch)
        {
            return new GithubReference(user, repo, null, branch, baseBranch, -1);
        }

        public static GithubReference FromIssue(string user, string repo, int issue)
        {
            return new GithubReference(user, repo, null, null, null, issue);
        }
    }
}
