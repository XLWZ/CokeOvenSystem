using CommunityToolkit.Mvvm.ComponentModel;

namespace CokeOvenSystem.Models
{
    public partial class TemperatureRecord : ObservableObject
    {
        [ObservableProperty]
        private string _date = DateTime.Now.ToString("yyyy-MM-dd");

        [ObservableProperty]
        private string _timePoint = string.Empty;

        [ObservableProperty]
        private double _oven1MachineTemp;

        [ObservableProperty]
        private double _oven2MachineTemp;

        [ObservableProperty]
        private double _oven3MachineTemp;

        [ObservableProperty]
        private double _oven1CokeTemp;

        [ObservableProperty]
        private double _oven2CokeTemp;

        [ObservableProperty]
        private double _oven3CokeTemp;
    }
}