Imports Microsoft.VisualBasic
Imports Demo
Imports System
Imports DevExpress.XtraBars
Imports System.Windows.Forms
Imports System.Configuration
Imports DevExpress.ExpressApp
Imports DevExpress.XtraBars.Ribbon
Imports DevExpress.ExpressApp.Security
Imports DevExpress.ExpressApp.SystemModule
Imports DevExpress.ExpressApp.Win.Templates

Namespace WinSolution.Win
	Friend NotInheritable Class Program
		'Dennis: This is necessary to easily access the application in static methods.
		Private Shared winApplication As WinSolutionWindowsFormsApplication
		''' <summary>
		''' The main entry point for the application.
		''' </summary>
		Private Sub New()
		End Sub
		<STAThread> _
		Shared Sub Main()
			Application.EnableVisualStyles()
			Application.SetCompatibleTextRenderingDefault(False)
			EditModelPermission.AlwaysGranted = System.Diagnostics.Debugger.IsAttached
			winApplication = New WinSolutionWindowsFormsApplication()

			AddHandler winApplication.CustomizeTemplate, AddressOf winApplication_CustomizeTemplate

			winApplication.Modules(0).BusinessClasses.Add(GetType(DemoObject1))
			winApplication.Modules(0).BusinessClasses.Add(GetType(DemoObject2))
			If ConfigurationManager.ConnectionStrings("ConnectionString") IsNot Nothing Then
				winApplication.ConnectionString = ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString
			Else
				winApplication.ConnectionString = CodeCentralExampleInMemoryDataStoreProvider.ConnectionString
			End If
			Try
				winApplication.Setup()
				CreateDemoObjects(winApplication)
				winApplication.Start()
			Catch e As Exception
				winApplication.HandleException(e)
			End Try
		End Sub
		Private Shared Sub CreateDemoObjects(ByVal application As XafApplication)
			Using os As IObjectSpace = application.CreateObjectSpace()
				Dim obj1 As DemoObject1 = os.CreateObject(Of DemoObject1)()
				Dim obj2 As DemoObject1 = os.CreateObject(Of DemoObject1)()
				Dim obj3 As DemoObject2 = os.CreateObject(Of DemoObject2)()
				obj1.Name = "DemoObject11"
				obj2.Name = "DemoObject12"
				obj3.Name = "DemoObject21"
				os.CommitChanges()
			End Using
		End Sub
		Private Shared Sub winApplication_CustomizeTemplate(ByVal sender As Object, ByVal e As CustomizeTemplateEventArgs)
			If e.Context = TemplateContext.ApplicationWindow Then
				AddHandler (CType(e.Template, Form)).Shown, AddressOf applicationWindowTemplate_Shown
			End If
		End Sub
		'Dennis: This code is necessary to focus the filter editor in the toolbar the first time the main form is shown.
		Private Shared Sub applicationWindowTemplate_Shown(ByVal sender As Object, ByVal e As EventArgs)
			AddHandler winApplication.ViewShown, AddressOf winApplication_ViewShown
			FocusFilterEditor()
		End Sub
		'Dennis: This code is necessary to focus the filter editor in the toolbar sequentially when a root ListView is shown.
		Private Shared Sub winApplication_ViewShown(ByVal sender As Object, ByVal e As ViewShownEventArgs)
			FocusFilterEditor()
		End Sub
		'Dennis: This routime checkes whether the FullTextFilterAction is available and focuses the corresponding item in the toolbar.
		Private Shared Sub FocusFilterEditor()
			Dim mainWindow As Window = winApplication.MainWindow
			Dim filterController As FilterController = mainWindow.GetController(Of FilterController)()
			Dim template As XtraFormTemplateBase = TryCast(mainWindow.Template, XtraFormTemplateBase)
			Dim barManager As BarManager = template.BarManager
			Dim filterBarItem As BarItem = barManager.Items("Filter by Text")
			If filterController.FullTextFilterAction.Active.ResultValue AndAlso filterController.FullTextFilterAction.Enabled.ResultValue Then
				If TypeOf barManager Is RibbonBarManager AndAlso filterBarItem IsNot Nothing AndAlso filterBarItem.Links.Count > 0 Then
					Dim filterBarItemLink As BarItemLink = filterBarItem.Links(0)
					filterBarItemLink.Focus()
				Else
					filterBarItem.PerformClick()
				End If
			End If
		End Sub
	End Class
End Namespace