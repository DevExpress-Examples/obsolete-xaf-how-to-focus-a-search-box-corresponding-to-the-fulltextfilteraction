using Demo;
using System;
using DevExpress.XtraBars;
using System.Windows.Forms;
using System.Configuration;
using DevExpress.ExpressApp;
using DevExpress.XtraBars.Ribbon;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Templates;

namespace WinSolution.Win {
    static class Program {
        //Dennis: This is necessary to easily access the application in static methods.
        private static WinSolutionWindowsFormsApplication winApplication;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            EditModelPermission.AlwaysGranted = System.Diagnostics.Debugger.IsAttached;
            winApplication = new WinSolutionWindowsFormsApplication();

            winApplication.CustomizeTemplate += winApplication_CustomizeTemplate;

            winApplication.Modules[0].BusinessClasses.Add(typeof(DemoObject1));
            winApplication.Modules[0].BusinessClasses.Add(typeof(DemoObject2));
            if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null)
                winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            else
                winApplication.ConnectionString = CodeCentralExampleInMemoryDataStoreProvider.ConnectionString;
            try {
                DevExpress.ExpressApp.InMemoryDataStoreProvider.Register();
                                winApplication.ConnectionString = DevExpress.ExpressApp.InMemoryDataStoreProvider.ConnectionString;
                winApplication.Setup();
                CreateDemoObjects(winApplication);
                winApplication.Start();
            } catch (Exception e) {
                winApplication.HandleException(e);
            }
        }
        private static void CreateDemoObjects(XafApplication application) {
            using (IObjectSpace os = application.CreateObjectSpace()) {
                DemoObject1 obj1 = os.CreateObject<DemoObject1>();
                DemoObject1 obj2 = os.CreateObject<DemoObject1>();
                DemoObject2 obj3 = os.CreateObject<DemoObject2>();
                obj1.Name = "DemoObject11";
                obj2.Name = "DemoObject12";
                obj3.Name = "DemoObject21";
                os.CommitChanges();
            }
        }
        private static void winApplication_CustomizeTemplate(object sender, CustomizeTemplateEventArgs e) {
            if (e.Context == TemplateContext.ApplicationWindow)
                ((Form)e.Template).Shown += applicationWindowTemplate_Shown;
        }
        //Dennis: This code is necessary to focus the filter editor in the toolbar the first time the main form is shown.
        private static void applicationWindowTemplate_Shown(object sender, EventArgs e) {
            winApplication.ViewShown += winApplication_ViewShown;
            FocusFilterEditor();
        }
        //Dennis: This code is necessary to focus the filter editor in the toolbar sequentially when a root ListView is shown.
        private static void winApplication_ViewShown(object sender, ViewShownEventArgs e) {
            FocusFilterEditor();
        }
        //Dennis: This routime checkes whether the FullTextFilterAction is available and focuses the corresponding item in the toolbar.
        private static void FocusFilterEditor() {
            Window mainWindow = winApplication.MainWindow;
            FilterController filterController = mainWindow.GetController<FilterController>();
            XtraFormTemplateBase template = mainWindow.Template as XtraFormTemplateBase;
            BarManager barManager = template.BarManager;
            BarItem filterBarItem = barManager.Items["Filter by Text"];
            if (filterController.FullTextFilterAction.Active.ResultValue
                && filterController.FullTextFilterAction.Enabled.ResultValue)
                if (barManager is RibbonBarManager
                    && filterBarItem != null && filterBarItem.Links.Count > 0) {
                    BarItemLink filterBarItemLink = filterBarItem.Links[0];
                    filterBarItemLink.Focus();
                }
                else
                    filterBarItem.PerformClick();
        }
    }
}