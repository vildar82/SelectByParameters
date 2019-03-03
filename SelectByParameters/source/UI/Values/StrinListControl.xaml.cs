namespace SelectByParameters.UI.Values
{
    using System.Windows.Controls;

    public partial class StrinListControl : UserControl
    {
        public StrinListControl(StringListVm vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
