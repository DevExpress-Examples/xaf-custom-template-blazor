using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace XafBlazorCustomTemplateSample.Module.BusinessObjects
{
    [DefaultClassOptions]
    public class Contact : BaseObject
    {
        private string name;
        public Contact(Session session) : base(session) { }

        public string Name
        {
            get => name;
            set => SetPropertyValue(nameof(Name), ref name, value);
        }
    }
}
