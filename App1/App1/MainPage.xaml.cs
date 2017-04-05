using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }




        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (inkCanvas.InkPresenter.StrokeContainer.GetStrokes().Any())
            {
                var fileSave = new FileSavePicker();

                fileSave.FileTypeChoices.Add("PNG", new string[] { ".png" });

                fileSave.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

                var storageFile = await fileSave.PickSaveFileAsync();

                if (storageFile != null)
                {
                    //dataText.Text = $"selected {storageFile.FileType}";
                    using (var stream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await SaveInkToStream(stream, storageFile.FileType);
                    }
                }
            }
        }

        private async Task SaveInkToStream(IRandomAccessStream stream, string fileType)
        {
            //var strokeContainerWidth = inkCanvas.ActualWidth;
            //var strokeContainerHeight = inkCanvas.ActualHeight;

            var strokeContainerBoundRect = inkCanvas.InkPresenter.StrokeContainer.BoundingRect;

            rectangle.RenderTransform = new TranslateTransform
            {
                X = strokeContainerBoundRect.X,
                Y = strokeContainerBoundRect.Y,

            };
            rectangle.Height = strokeContainerBoundRect.Height;
            rectangle.Width = strokeContainerBoundRect.Width;

            CanvasDevice device = CanvasDevice.GetSharedDevice();

            using (var renderTarget = new CanvasRenderTarget(device, (float)strokeContainerBoundRect.Width, (float)strokeContainerBoundRect.Height, 96f))
            {

                using (CanvasDrawingSession ds = renderTarget.CreateDrawingSession())
                {
                    ds.Transform = new Matrix3x2() { M11 = 1, M22 = 1, Translation = new Vector2((float)-strokeContainerBoundRect.X, (float)-strokeContainerBoundRect.Y)};
                    ds.Clear(Colors.White);
                    ds.DrawInk(inkCanvas.InkPresenter.StrokeContainer.GetStrokes());
                }

                await renderTarget.SaveAsync(stream, CanvasBitmapFileFormat.Png, 1f);
            }
        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.FileTypeFilter.Clear();
            openPicker.FileTypeFilter.Add(".isf");
            openPicker.FileTypeFilter.Add(".gif");

            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);

                // Read from file.
                using (var inputStream = stream.GetInputStreamAt(0))
                {
                    await inkCanvas.InkPresenter.StrokeContainer.LoadAsync(inputStream);
                }
                stream.Dispose();

            }
        }
    }
}
