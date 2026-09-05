using F1Telemetry.Core.Models;
using F1Telemetry.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace F1Telemetry.App.ViewModels
{
    public class TelemetryDashboardViewModel : ViewModelBase
    {
        private int _selectedDriverId;

        public int SelectedDriverId
        {
            get => _selectedDriverId;
            set => SetProperty(ref _selectedDriverId, value);
        }

        private DriverInfo? _selectedDriver;
        public DriverInfo? SelectedDriver
        {
            get => _selectedDriver;
            set => SetProperty(ref _selectedDriver, value);
        }

        public IReadOnlyList<DriverInfo> Drivers { get; } = DriverData.Drivers;

        public TelemetryDashboardViewModel()
        {
            SelectedDriverId = 0;
            SelectedDriver = Drivers[0];
        }
    }
}
