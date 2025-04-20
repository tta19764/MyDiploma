using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;

namespace HumanResourcesApp.Models
{
    public partial class PositionDisplayModel : ObservableObject
    {
        [ObservableProperty]
        private string positionTitle = string.Empty;
        [ObservableProperty]
        private string description = string.Empty;
        [ObservableProperty]
        private bool isEdditing;

        public int PositionId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
