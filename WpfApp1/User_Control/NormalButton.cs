using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApp1.User_Control
{
    public class NormalButton : Image
    {
        byte[] pixels;
        Int32Rect int32Rect;
        BitmapSource source;
        public NormalButton()
        {
         }
        protected override void OnInitialized(EventArgs e)
        {
            pixels = new byte[4];
            int32Rect = new Int32Rect();
            int32Rect.Width = 1;
            int32Rect.Height = 1;
            Stretch = Stretch.Fill;
            base.OnInitialized(e);
        }
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            // Get value of current pixel
            // BitmapSource test =(BitmapSource)OpacityMask.GetValue(ImageBrush.ImageSourceProperty);
            source = (OpacityMask == null) ? (BitmapSource)Source : (BitmapSource)OpacityMask.GetValue(ImageBrush.ImageSourceProperty);
            int32Rect.X= (int)(hitTestParameters.HitPoint.X /
                ActualWidth * source.PixelWidth)-1;
            int32Rect.Y= (int)(hitTestParameters.HitPoint.Y /
                ActualHeight * source.PixelHeight)-1;
            int32Rect.X = int32Rect.X < 0 ? 0 : int32Rect.X;
            int32Rect.Y = int32Rect.Y < 0 ? 0 : int32Rect.Y;
            source.CopyPixels(int32Rect,pixels,4, 0);
            // Check alpha channel
            if (pixels[3]==255)
            {
                return new PointHitTestResult(this, hitTestParameters.HitPoint);
            }
            else
            {
                return null;
            }
        }
        protected override GeometryHitTestResult HitTestCore(GeometryHitTestParameters hitTestParameters)
        {
            return base.HitTestCore(hitTestParameters);
        }
    }
}
