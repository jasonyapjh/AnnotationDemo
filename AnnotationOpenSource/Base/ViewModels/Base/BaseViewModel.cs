using Base.Common;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.ViewModels.Base
{
    public abstract class BaseViewModel : NotifyPropertyChangedBase, INavigationAware
    {
        public IRegionManager Regions;
        //  public IContainerProvider ContainerProvider;
        public IEventAggregator Events;
        public IDialogService DialogService;
        public IContainerExtension Containers;
        public BaseViewModel()
        {

        }
        public BaseViewModel(IRegionManager regionManager) : this()
        {
            Regions = regionManager;
        }

        protected BaseViewModel(IContainerExtension containerExtension) : this()
        {
            Containers = containerExtension;
        }
        protected BaseViewModel(IEventAggregator eventAggregator) : this()
        {
            Events = eventAggregator;
        }


        public BaseViewModel(IContainerExtension containerExtension, IEventAggregator eventAggregator) : this(containerExtension)
        {
            Events = eventAggregator;
        }

        public BaseViewModel(IContainerExtension containerExtension, IEventAggregator eventAggregator, IDialogService dialogService) : this(containerExtension, eventAggregator)
        {
            DialogService = dialogService;
        }

        public BaseViewModel(IContainerExtension containerExtension, IEventAggregator eventAggregator, IRegionManager regionManager) : this(containerExtension, eventAggregator)
        {
            Regions = regionManager;
        }

        public BaseViewModel(IContainerExtension containerExtension, IEventAggregator eventAggregator, IRegionManager regionManager, IDialogService dialogService) : this(containerExtension, eventAggregator, regionManager)
        {
            Containers = containerExtension;
            DialogService = dialogService;
        }
        public abstract void OnNavigatedTo(NavigationContext navigationContext);
        public abstract bool IsNavigationTarget(NavigationContext navigationContext);
        public abstract void OnNavigatedFrom(NavigationContext navigationContext);
    }
}
