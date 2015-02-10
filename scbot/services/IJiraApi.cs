using System.Threading.Tasks;

namespace scbot.services
{
    public interface IJiraApi
    {
        Task<JiraBug> FromId(string id);
    }
}