using System;
using System.ComponentModel;

namespace HumanResourcesApp.Models
{
    public partial class PositionDisplayModel : INotifyPropertyChanged
    {
        private string _positionTitle;
        private string _description;
        private bool _isEdditing;

        public int PositionId { get; set; }
        public DateTime? CreatedAt { get; set; }

        public string PositionTitle
        {
            get => _positionTitle;
            set
            {
                _positionTitle = value;
                OnPropertyChanged(nameof(PositionTitle));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public bool IsEdditing
        {
            get => _isEdditing;
            set
            {
                _isEdditing = value;
                OnPropertyChanged(nameof(IsEdditing));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
