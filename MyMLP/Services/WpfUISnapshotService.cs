using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace MyMLP.Services
{
    public class WpfUISnapshotService : IUISnapshotService
    {
        public async Task SaveAsPngAsync(FrameworkElement element, string filePath)
        {
            if (element == null || string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(element), "Element and filePath cannot be null.");
            }
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            if (element.ActualWidth <= 0 || element.ActualHeight <= 0)
            {
                return;
            }

            var renderSize = new Size(element.ActualWidth, element.ActualHeight);

            element.Measure(renderSize);
            element.Arrange(new Rect(new Point(0, 0), renderSize));

            int pixelWidth = (int)Math.Max(1, Math.Ceiling(renderSize.Width));
            int pixelHeight = (int)Math.Max(1, Math.Ceiling(renderSize.Height));

            var renderTargetBitmap = new RenderTargetBitmap(
        pixelWidth,
        pixelHeight,
        96, 96,
        PixelFormats.Pbgra32);

            renderTargetBitmap.Render(element);

            var pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                pngEncoder.Save(fileStream);
            }
        });
        }
    }
}