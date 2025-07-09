using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CbcRoastersErp.Helpers;

namespace CbcRoastersErp.Services
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;
        private readonly Func<Task> _executeAsync;
        private readonly Func<object, Task> _executeParamAsync;
        private ICommand? openAddEditEmployeeCommand;

        public RelayCommand(Func<Task> executeAsync, Func<object, bool> canExecute = null)
        {
            _executeAsync = executeAsync;
            _canExecute = canExecute;
        }

        public RelayCommand(Func<object, Task> executeParamAsync, Func<object, bool> canExecute = null)
        {
            _executeParamAsync = executeParamAsync;
            _canExecute = canExecute;
        }



        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public RelayCommand(Func<Task> executeAsync)
        {
            _execute = async _ => await executeAsync();
        }

        public RelayCommand(ICommand? openAddEditEmployeeCommand)
        {
            this.openAddEditEmployeeCommand = openAddEditEmployeeCommand;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public async void Execute(object parameter)
        {
            try
            {
                if (_execute != null)
                {
                    _execute(parameter);
                }
                else if (_executeAsync != null)
                {
                    await _executeAsync();
                }
                else if (_executeParamAsync != null)
                {
                    await _executeParamAsync(parameter);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ApplicationLogger.Log(ex, nameof(RelayCommand), nameof(Execute), Environment.UserName);
            }
            finally
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }


        public async void asyncExecute(object parameter)
        {
            try
            {
                if (_executeAsync != null)
                    await _executeAsync();
                else if (_executeParamAsync != null)
                    await _executeParamAsync(parameter);
            }
            catch (Exception ex)
            {
                // Optionally handle global command exceptions
                System.Diagnostics.Debug.WriteLine(ex);
                ApplicationLogger.Log(ex, nameof(RelayCommand), nameof(Execute), Environment.UserName);
            }
            finally
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

    }
}
