using System.Threading.Tasks;

namespace monitorbot.jira.services
{
    public interface IJiraApi
    {
        Task<JiraBug> FromId(string id);
    }
}