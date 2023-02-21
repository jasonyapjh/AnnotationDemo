using AnnotationOpenSource.Events;
using AnnotationOpenSource.Framework;
using Base.ViewModels.Base;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnotationOpenSource.Shell
{
    public class ShellViewModel : BaseViewModel
    {
        public ShellViewModel(IContainerExtension containerExtension, IEventAggregator eventAggregator, IRegionManager regionManager, IDialogService dialogService) : base(containerExtension, eventAggregator, regionManager, dialogService)
        {
            Events.GetEvent<AllModulesLoaded>().Subscribe(OnAllModulesLoaded);
        }
        private void OnAllModulesLoaded()
        {
            Events.GetEvent<AllModulesLoaded>().Unsubscribe(OnAllModulesLoaded);
            Regions.RequestNavigate(RegionManagerEntity.ShellRegionSource, ScreenEntity.MainContent);
        }
        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Regions.RequestNavigate(RegionManagerEntity.ShellRegionSource, ScreenEntity.MainContent);
        }
    }
}
