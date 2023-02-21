using AnnotationOpenSource.Events;
using AnnotationOpenSource.Module;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AnnotationOpenSource.Shell
{
    public class AppBootstrapper : PrismBootstrapper
    {

        protected override DependencyObject CreateShell()
        {
            return new Shell();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
          
        }

        protected override void InitializeModules()
        {
          
            base.InitializeModules();
        }
        protected override void InitializeShell(DependencyObject shell)
        {
            base.InitializeShell(shell);
            App.Current.MainWindow = (Window)this.Shell;
            App.Current.MainWindow.Show();
        }
        protected override void OnInitialized()
        {
            var eventAggregator = Container.Resolve<IEventAggregator>();
            eventAggregator?.GetEvent<AllModulesLoaded>().Publish();

            Process proc = Process.GetCurrentProcess();
            int count = Process.GetProcesses().Where(p => p.ProcessName == proc.ProcessName).Count();

            if (count > 1)
            {
                MessageBox.Show("Already an Instance is running...");
                App.Current.Shutdown();
            }

     
            base.OnInitialized();

        }
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
           
            Type ModuleLocatorType = typeof(ShellModuleLocator);

            moduleCatalog.AddModule(new ModuleInfo
            {
                ModuleName = ModuleLocatorType.Name,
                ModuleType = ModuleLocatorType.AssemblyQualifiedName
            });

            //moduleCatalog.AddModule(typeof(WindowsModuleLocator));
            base.ConfigureModuleCatalog(moduleCatalog);
         

        }

   
    }
}
