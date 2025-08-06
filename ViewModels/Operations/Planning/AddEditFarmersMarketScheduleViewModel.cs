using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CbcRoastersErp.Models.Planning;
using CbcRoastersErp.Repositories.Operations.Planning;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Services;

namespace CbcRoastersErp.ViewModels.Operations.Planning
{
    public class AddEditFarmersMarketScheduleViewModel : INotifyPropertyChanged
    {
        private readonly FarmersMarketProductionScheduleRepository _scheduleRepository;

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action OnCloseRequested;

        public FarmersMarketProductionSchedule Schedule { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddEditFarmersMarketScheduleViewModel(FarmersMarketProductionSchedule schedule)
        {
            _scheduleRepository = new FarmersMarketProductionScheduleRepository();

            Schedule = schedule ?? new FarmersMarketProductionSchedule
            {
                MarketDate = DateTime.Now,
                CreatedBy = Environment.UserName,
                CreatedAt = DateTime.Now
            };

            SaveCommand = new RelayCommand(async (_) => await SaveScheduleAsync());
            CancelCommand = new RelayCommand(_ => OnCloseRequested?.Invoke());
        }

        private async Task SaveScheduleAsync()
        {
            try
            {
                if (Schedule.Id == 0)
                {
                    int insertedId = await _scheduleRepository.AddScheduleAsync(Schedule);
                    if (insertedId > 0)
                        Schedule.Id = insertedId;
                }
                else
                {
                    await _scheduleRepository.UpdateScheduleAsync(Schedule);
                }

                OnCloseRequested?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving schedule: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

