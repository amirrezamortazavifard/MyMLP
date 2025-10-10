using System.Threading.Tasks;
using System.Windows;

namespace MyMLP.Services
{
    public interface IUISnapshotService
    {
        Task SaveAsPngAsync(FrameworkElement element, string filePath);
    }
}