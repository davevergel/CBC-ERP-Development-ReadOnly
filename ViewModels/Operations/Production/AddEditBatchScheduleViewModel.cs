using CbcRoastersErp.Helpers;
using CbcRoastersErp.Models;
using CbcRoastersErp.Repositories;
using CbcRoastersErp.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace CbcRoastersErp.ViewModels
{
    public class AddEditBatchScheduleViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<FinishedGoods> FinishedGoodsList { get; set; }
        public FinishedGoods SelectedFinishedGood { get; set; }

        private readonly BatchScheduleRepository _repository;
        private bool _isEditMode;

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action OnCloseRequested;
        public BatchSchedule Schedule { get; set; }

        public ICommand SaveCommand => new RelayCommand(Save);
        public ICommand CancelCommand => new RelayCommand(_ => OnCloseRequested?.Invoke());
        public Action CloseAction { get; set; }

        public AddEditBatchScheduleViewModel(BatchSchedule schedule = null)
        {
            _repository = new BatchScheduleRepository();
            _isEditMode = schedule != null && schedule.ScheduleID > 0;

            // Initialize Schedule with provided or default values
            Schedule = schedule ?? new BatchSchedule();

            // Load Finished Goods for dropdown
            FinishedGoodsList = new ObservableCollection<FinishedGoods>(_repository.GetAllFinishedGoods());

            if (_isEditMode)
            {
                // If editing, set the existing schedule
                Schedule = schedule;
            }
            else
            {
                // If creating new, initialize with default values
                Schedule = new BatchSchedule
                {
                    ScheduledDate = DateTime.Today,
                    Status = "Scheduled"
                };
            }

            OnPropertyChanged(nameof(Schedule));
        }

        private void Save(object _)
        {
            try
            {
                if (Schedule.ScheduleID == 0)
                    _repository.AddSchedule(Schedule);
                else
                    _repository.UpdateScheduleStatus(Schedule.ScheduleID, Schedule.Status);

                OnCloseRequested?.Invoke();
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., show a message box)
                ApplicationLogger.Log(ex, "System", "Error");
                MessageBox.Show($"Error saving schedule: {ex.Message}", "Error", MessageBoxButton.OK);
            }
        }

        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
