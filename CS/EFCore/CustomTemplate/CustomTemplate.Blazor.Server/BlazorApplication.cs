using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ApplicationBuilder;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.SystemModule;
using CustomTemplate.Module.BusinessObjects;
using Microsoft.EntityFrameworkCore;
using DevExpress.ExpressApp.EFCore;
using DevExpress.EntityFrameworkCore.Security;
using CustomTemplate.Blazor.Server.Templates;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Blazor.SystemModule;

namespace CustomTemplate.Blazor.Server;

public class CustomTemplateBlazorApplication : BlazorApplication {
    public CustomTemplateBlazorApplication() {
        ApplicationName = "CustomTemplate";
        CheckCompatibilityType = DevExpress.ExpressApp.CheckCompatibilityType.DatabaseSchema;
        DatabaseVersionMismatch += CustomTemplateBlazorApplication_DatabaseVersionMismatch;
    }
    protected override void OnSetupStarted() {
        base.OnSetupStarted();
#if DEBUG
        if(System.Diagnostics.Debugger.IsAttached && CheckCompatibilityType == CheckCompatibilityType.DatabaseSchema) {
            DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
        }
#endif
    }
    protected override IFrameTemplate CreateDefaultTemplate(TemplateContext context) {
        if (context == TemplateContext.ApplicationWindow) {
            return new CustomMainFormTemplate((IModelOptionsBlazor)Model.Options) { AboutInfoString = AboutInfo.Instance.GetAboutInfoString(this) };
        }
        return base.CreateDefaultTemplate(context);
    }
    private void CustomTemplateBlazorApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e) {
#if EASYTEST
        e.Updater.Update();
        e.Handled = true;
#else
        if(System.Diagnostics.Debugger.IsAttached) {
            e.Updater.Update();
            e.Handled = true;
        }
        else {
            string message = "The application cannot connect to the specified database, " +
                "because the database doesn't exist, its version is older " +
                "than that of the application or its schema does not match " +
                "the ORM data model structure. To avoid this error, use one " +
                "of the solutions from the https://www.devexpress.com/kb=T367835 KB Article.";

            if(e.CompatibilityError != null && e.CompatibilityError.Exception != null) {
                message += "\r\n\r\nInner exception: " + e.CompatibilityError.Exception.Message;
            }
            throw new InvalidOperationException(message);
        }
#endif
    }
}
