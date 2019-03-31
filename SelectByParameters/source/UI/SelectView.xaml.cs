namespace SelectByParameters.UI
{
    public partial class SelectView
    {
        public SelectView(SelectVm vm)
            : base(vm)
        {
            InitializeComponent();
            Closed += (sender, args) => vm.OnClosed();
        }
    }
}
