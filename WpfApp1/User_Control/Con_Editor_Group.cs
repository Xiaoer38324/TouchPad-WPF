using System.Windows.Input;
using WpfApp1.Logic.UILogic;

namespace WpfApp1.User_Control
{
    internal class Con_Editor_Group:Con_Editor_Hidder
    {
        protected override void Mouse_Down(object sender, MouseButtonEventArgs e)
        {
            griddestiny = (int)Editor_GobalVar.GetIns().logic.GetSetting(EdiorSetting.GidDestiny);
            attachdismin = griddestiny / 10;
            attachdismax = griddestiny - attachdismin;
            enableattach = (bool)Editor_GobalVar.GetIns().logic.GetSetting(EdiorSetting.EnableAttach);
            isdrag = true;
           // Editor_GobalVar.GetIns().logic.SetActive(this);

            e.Handled = true;
        }
    }
}
