namespace SelectByParameters.UI
{
    using System.Collections.ObjectModel;
    using Data;

    public class GroupVm
    {
        public Group Group { get; set; }
        public ObservableCollection<ParameterVm> Parameters { get; set; } = new ObservableCollection<ParameterVm>();
    }
}
