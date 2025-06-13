using System.Windows.Controls;
using WpfApp1.Attributes;

namespace WpfApp1
{
    public struct ConBaseInfo {
        public string name { get; set; }
        public string remarks{ get; set; }
        public EControlType type { get; set; }
        public uint con_id=uint.MaxValue;
        public ConBaseInfo(string name, string remarks, EControlType type)
        {
            this.name = name;
            this.remarks = remarks;
            this.type = type;
        }
    }
    internal struct ConBaseData
    {
        
        public ConBaseInfo conBaseInfo;
        public Control con;

        public ConBaseData(uint con_id, ConBaseInfo info, Control con)
        {
            conBaseInfo = info;
            conBaseInfo.con_id = con_id;
            this.con = con;
        }
    }
    internal enum EditorUiEvent
    {
        ADD,ACTIVE,Flash,Delete,FlashConToData,CloseWindow
    }
    internal interface EditorUIEvent_Listern
    {
        void AcceptEvent(EditorUiEvent eevent,object obj);
    }
    internal interface EditorUIEvent_Listerner
    {
        void AddListern(EditorUIEvent_Listern lis);
        void RemoveListern(EditorUIEvent_Listern lis);
        void TriggerAllListern(EditorUiEvent eevent,object obj);
    }
}
