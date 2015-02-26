using System.Threading.Tasks;

namespace scbot.jira.services
{
    public interface IJiraApi
    {
        Task<JiraBug> FromId(string id);
    }
}