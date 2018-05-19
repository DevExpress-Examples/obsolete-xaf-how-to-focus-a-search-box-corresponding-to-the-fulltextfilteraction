using System;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;

namespace WinSolution.Win {
    [DefaultClassOptions]
    public class DemoObject1 : BaseObject {
        public DemoObject1(Session session) : base(session) { }
        private string _Name;
        public string Name {
            get { return _Name; }
            set { SetPropertyValue("Name", ref _Name, value); }
        }
    }
    [DefaultClassOptions]
    public class DemoObject2 : BaseObject {
        public DemoObject2(Session session) : base(session) { }
        private string _Name;
        public string Name {
            get { return _Name; }
            set { SetPropertyValue("Name", ref _Name, value); }
        }
    }
}
