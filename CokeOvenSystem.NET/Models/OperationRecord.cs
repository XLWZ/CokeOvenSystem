using CommunityToolkit.Mvvm.ComponentModel;

namespace CokeOvenSystem.Models
{
    public partial class OperationRecord : ObservableObject
    {
        [ObservableProperty]
        private int _ovenNumber = 1;

        [ObservableProperty]
        private string _chamber = string.Empty;

        [ObservableProperty]
        private DateTime _previousPushTime = DateTime.Now;

        [ObservableProperty]
        private DateTime _newLoadTime = DateTime.Now;
    }
}