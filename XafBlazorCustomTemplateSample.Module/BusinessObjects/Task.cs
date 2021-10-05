using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace XafBlazorCustomTemplateSample.Module.BusinessObjects {
    [DefaultClassOptions]
    public class Task : BaseObject {
        private string title;
        public Task(Session session) : base(session) { }

        public string Title {
            get => title;
            set => SetPropertyValue(nameof(Title), ref title, value);
        }
    }
}
