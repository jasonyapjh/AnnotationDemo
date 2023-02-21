using Base.ViewModels.Base;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace AnnotationsCreator.Workspace.Production
{
    public class ProductionViewModel : BaseViewModel
    {
      //  public HWindowInspectionViewModel HWindowStationA;
     /*   public HSmartWindowControlWPF StationAHWindowCtrl
        {
            set;
            get;
        }*/
        public ProductionViewModel(IContainerExtension containerExtension, IEventAggregator eventAggregator, IRegionManager regionManager, IDialogService dialogService) : base(containerExtension, eventAggregator, regionManager, dialogService)
        {
            //HWindowStationA = Containers.Resolve<HWindowInspectionViewModel>(ScreenEntity.LeftInspHWindow);
           // StationAHWindowCtrl = HWindowStationA.HWindowCtrl;
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

        }
    }
}
