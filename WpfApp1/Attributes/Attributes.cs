using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp1.Attributes
{
    /// <summary>
    /// 用于管理属性Ui的类，一种控件的属性对应一个
    /// </summary>
    internal class Com_Attributes
    {
        private Grid view;
        AttributeOperInterFace data;
        bool initviewed;
        Dictionary<uint, object> coms;//储存每个属性对应的UI控件(可变化部分)
        public void FlashData(Control con,uint which)
        {
            data.FlashData(con, which);
        }
        private bool HalfSearch(uint[] ids,uint target)
        {
            if (ids == null) return false;
            int i = 0;
            int j = ids.Length-1;
            while (true)
            {

                if (j < i) return false;
                if (ids[(i + j) / 2] == target) return true;
                else if (ids[(i + j) / 2] > target) j= (i + j) / 2-1;
                else i= (i + j) / 2+1;
            }
        }
        //组件更新
        public void SetData(AttributeOperInterFace data)
        {
            this.data = data;
            Editor_GobalVar.GetIns().callback = data.Inform;
            ComponentType type;
            uint[] hide = (uint[])data.GetDataById(1024);
            foreach (Attr_Lable lable in data.GetAttr_Lables())
            {
                foreach (uint index in lable.index)
                {
                    type=data.GetTypeById(index);
                    switch (type)
                    {
                        case ComponentType.A_Bar:
                           ((Slider)((object[])coms[index])[0]).Value =((A_Bar_Par)data.GetDataById(index)).p;
                            if (!HalfSearch(hide, index))
                            {

                                ((Slider)((object[])coms[index])[0]).Visibility = Visibility.Visible;
                                ((TextBox)((object[])coms[index])[1]).Visibility = Visibility.Visible;
                            }
                            else {
                                ((Slider)((object[])coms[index])[0]).Visibility = Visibility.Collapsed;
                                ((TextBox)((object[])coms[index])[1]).Visibility = Visibility.Collapsed;
                            }
                            break;
                        case ComponentType.A_Bar_NAO:
                            ((Slider)((object[])coms[index])[0]).Value = ((A_Bar_Par)data.GetDataById(index)).p;
                            if (!HalfSearch(hide, index))
                            {

                                ((Slider)((object[])coms[index])[0]).Visibility = Visibility.Visible;
                                ((TextBox)((object[])coms[index])[1]).Visibility = Visibility.Visible;
                            }
                            else
                            {
                                ((Slider)((object[])coms[index])[0]).Visibility = Visibility.Collapsed;
                                ((TextBox)((object[])coms[index])[1]).Visibility = Visibility.Collapsed;
                            }
                            break;
                        case ComponentType.A_Combox:
                            ((ComboBox)coms[index]).SelectedIndex = ((A_Combox_Par)data.GetDataById(index)).defaultindex;
                            
                            if (!HalfSearch(hide, index)) 
                                ((ComboBox)coms[index]).Visibility = Visibility.Visible;
                            else
                                ((ComboBox)coms[index]).Visibility = Visibility.Collapsed;
                            break;
                        case ComponentType.A_ResourceSelect:
                            ((Label)coms[index]).Content = ((A_ResourceSelect_Par)data.GetDataById(index)).path;
                           
                            if (!HalfSearch(hide, index))
                                ((Label)coms[index]).Visibility = Visibility.Visible;
                            else
                                ((Label)coms[index]).Visibility = Visibility.Collapsed;
                            break;
                        case ComponentType.A_TwoTextEditorWithDouble:
                            var xp = (A_TwoTextEditorWithDouble_Par)data.GetDataById(0);
                            ((TextBox[])coms[index])[0].Text = ((A_TwoTextEditorWithDouble_Par)data.GetDataById(index)).p1.ToString();
                            ((TextBox[])coms[index])[1].Text = ((A_TwoTextEditorWithDouble_Par)data.GetDataById(index)).p2.ToString();
                            if (!HalfSearch(hide, index))
                            {
                                ((TextBox[])coms[index])[0].Visibility = Visibility.Visible;
                                ((TextBox[])coms[index])[1].Visibility = Visibility.Visible;
                            }
                            else {
                                ((TextBox[])coms[index])[0].Visibility = Visibility.Collapsed;
                                ((TextBox[])coms[index])[1].Visibility = Visibility.Collapsed;
                            }
                            break;
                        case ComponentType.A_TwoTextEditorWithText:
                            ((TextBox[])coms[index])[0].Text = ((A_TwoTextEditorWithText_Par)data.GetDataById(index)).p1;
                            ((TextBox[])coms[index])[1].Text = ((A_TwoTextEditorWithText_Par)data.GetDataById(index)).p2;

                            if (!HalfSearch(hide, index)) {
                                ((TextBox[])coms[index])[0].Visibility = Visibility.Visible;
                                ((TextBox[])coms[index])[1].Visibility = Visibility.Visible;
                            }
                            else
                            {
                                ((TextBox[])coms[index])[0].Visibility = Visibility.Collapsed;
                                ((TextBox[])coms[index])[1].Visibility = Visibility.Collapsed;
                            }
                            break;
                        case ComponentType.A_Button:
                            ((Button)(coms[index])).Content = ((A_Button_Par)data.GetDataById(index)).name;
                            if (!HalfSearch(hide, index))
                                ((Button)coms[index]).Visibility = Visibility.Visible;
                            else
                                ((Button)coms[index]).Visibility = Visibility.Collapsed;
                            break;
                            break;
                        case ComponentType.None:
                            break;
                            default:
                            MessageBox.Show("未知控件");
                            break;
                    }
                    
                }
            }
        }
        public void Init(AttributeOperInterFace data)
        {
            if (!initviewed)
            {
                InitView(data);
                initviewed = true;
            }
        }
        private void InitView(AttributeOperInterFace data)
        {
            this.data = data;
            coms=new Dictionary<uint, object>();
            view = new Grid();
            Attr_Lable[] datas = data.GetAttr_Lables();
            int i=0;
            Editor_GobalVar.GetIns().callback = data.Inform;
            foreach (Attr_Lable lable in datas)
            {
                RowDefinition definition = new RowDefinition();
                definition.Height = GridLength.Auto;
                view.RowDefinitions.Add(definition);
                Expander expander = new Expander();
                expander.Header = lable.name;
                int j = 0;
                Border border = new Border();
                border.BorderBrush = Brushes.Gray;
                border.BorderThickness = new Thickness(1);
                Grid onelable=new Grid();
                foreach (uint index in lable.index)
                {
                    RowDefinition definition2 = new RowDefinition();
                    definition2.Height = GridLength.Auto;
                    onelable.RowDefinitions.Add(definition2);
                    Panel p;
                    object obj = _Ins_Creator.Create(data.GetTypeById(index), data.GetDataById(index)).InitGetView(out p);
                    if(obj!=null)
                    coms.Add(index,obj);
                    p.SetValue(Grid.RowProperty, j);
                    onelable.Children.Add(p);
                    j++;
                }
                expander.Content = onelable;
                expander.SetValue(Grid.RowProperty, i);
                border.SetValue(Grid.RowProperty, i);
                view.Children.Add(expander);
                view.Children.Add(border);
                i++;
            }
        }
        public Com_Attributes()
        {
        }
        public Grid GetView()
        {
            return view;
        }
        public object GetCom(uint index)
        {
            if(coms.ContainsKey(index))
            return coms[index];
            return null;
        }
    }
}
