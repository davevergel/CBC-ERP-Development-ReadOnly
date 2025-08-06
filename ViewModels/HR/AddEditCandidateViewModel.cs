using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CbcRoastersErp.Models.HR;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Services;
using CBCRoastersErp.Repositories.HR;
using System.Collections.ObjectModel;
using System.Windows;

namespace CbcRoastersErp.ViewModels.HR
{
    public class AddEditCandidateViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action OnCloseRequested;
        public Action CloseAction { get; set; }

        private readonly CandidateRepository _candidateRepository;
        private Candidate _candidate;
        private bool _isEditMode;

        public ObservableCollection<Candidate> Candidates { get; set; }
        public Candidate SelectedCandidate { get; set; }

        public ObservableCollection<JobPosting> JobPosts { get; set; }
        public JobPosting SelectedJobPost { get; set; }
        public Candidate Candidate
        {
            get => _candidate;
            set
            {
                _candidate = value;
                OnPropertyChanged(nameof(Candidate));
            }
        }


        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddEditCandidateViewModel(Candidate item)
        {
            _candidateRepository = new CandidateRepository();
            _isEditMode = item != null && item.CandidateID > 0;
            Candidate = item;

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(_ => OnCloseRequested?.Invoke());

            LoadJobPostings();

            OnPropertyChanged(nameof(Candidate));
        }

        private async void LoadJobPostings()
        {
            try
            {
                var postings = await _candidateRepository.GetAllJobPostingsAsync();
                JobPosts = new ObservableCollection<JobPosting>(postings);

                if (_isEditMode)
                {
                    SelectedJobPost = JobPosts.FirstOrDefault(jp => jp.JobID == Candidate.AppliedJobID);
                }

                OnPropertyChanged(nameof(JobPosts));
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, ex.Message, "Failed to load job postings");
            }
            finally
            {
                OnPropertyChanged(nameof(JobPosting));
            }
        }

        private async void Save(object _)
        {
            try
            {
                if (SelectedJobPost != null)
                    Candidate.AppliedJobID = SelectedJobPost.JobID;

                if (_isEditMode)
                {
                    await _candidateRepository.UpdateAsync(Candidate);
                }
                else
                {
                    await _candidateRepository.AddAsync(Candidate);
                }

                OnCloseRequested?.Invoke();
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "Failed to save candidate");
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}