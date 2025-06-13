using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Attributes
{
    internal class A_Bar_ReturnNewAndOld : A_Bar
    {
        public A_Bar_ReturnNewAndOld(A_Bar_Par par) : base(par)
        {
        }
        public override object InitGetView(out Panel planel)
        {
            object result = base.InitGetView( out planel);
            bar.RemoveHandler(Slider.ValueChangedEvent,eventhandle);
            bar.AddHandler(Slider.ValueChangedEvent, new RoutedPropertyChangedEventHandler<double>(delegate (object sender, RoutedPropertyChangedEventArgs<double> e) {
                Editor_GobalVar.GetIns().callback(id, new double[] { e.NewValue, e.OldValue });
            }));
            return result;
        }
    }
}
