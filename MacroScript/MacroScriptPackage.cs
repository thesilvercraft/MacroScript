global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;
using EnvDTE;
using EnvDTE80;
using System.Runtime.InteropServices;
using System.Threading;

namespace MacroScript
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.MacroScriptString)]
    [ProvideToolWindow(typeof(MainToolWindo))]
    public sealed class MacroScriptPackage : ToolkitPackage
    {
        public static DTE2 dte;
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.RegisterCommandsAsync();
            await MainToolWindoCommand.InitializeAsync(this);
            dte = (DTE2)GetService(typeof(DTE));
        }
    }
}