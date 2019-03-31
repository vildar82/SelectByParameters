namespace SelectByParameters.Lib.Mvvm
{
    using System.Windows;

    public class BaseWindow : Window
    {
        public BaseWindow()
        {
        }

        public BaseWindow(BaseViewModel vm)
        {
            if (vm == null) return;
            vm.Window = this;
            DataContext = vm;
            Closed += (o, e) => vm.OnClosed();
        }
    }
}
