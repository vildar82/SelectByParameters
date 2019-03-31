namespace SelectByParameters.Lib.Mvvm
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using JetBrains.Annotations;

    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Window Window { get; set; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public IDisposable HideWindow()
        {
            return new ActionUsage(HideMe, VisibleMe);
        }

        public void HideMe()
        {
            if (Window != null) Window.Visibility = Visibility.Hidden;
        }

        public void VisibleMe()
        {
            if (Window != null) Window.Visibility = Visibility.Visible;
        }

        public virtual void OnClosed()
        {
        }
    }
}
