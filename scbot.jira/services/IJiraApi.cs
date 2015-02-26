using System.Threading.Tasks;

namespace scbot.services.jira
{
    public interface IJiraApi
    {
        Task<JiraBug> FromId(string id);
    }
}