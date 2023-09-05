using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ac
{
    public class ViewModel : INotifyPropertyChanged
    {
        private bool _isDetailSearchVisible = false;

        public bool IsDetailSearchVisible
        {
            get { return _isDetailSearchVisible; }
            set
            {
                if (_isDetailSearchVisible != value)
                {
                    _isDetailSearchVisible = value;
                    OnPropertyChanged(nameof(IsDetailSearchVisible));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
