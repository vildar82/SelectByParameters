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
            vm.Window = this;
            DataContext = vm;
        }
    }
}
