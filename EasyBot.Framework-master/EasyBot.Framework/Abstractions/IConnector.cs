using System.Threading.Tasks;

namespace EasyBot.Framework.Abstractions
{
    public interface IConnector<TModel>
    {
        Task SendActivity(TModel activity);
    }
}
