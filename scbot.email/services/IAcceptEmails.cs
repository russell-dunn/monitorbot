using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.email.services
{
    public interface IAcceptEmails
    {
        void Accept(Email emailMessage);
    }
}
