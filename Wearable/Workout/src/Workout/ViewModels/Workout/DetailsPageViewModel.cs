﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Workout.Models.Workout;
using Workout.Services;
using Workout.Services.Settings;
using Xamarin.Forms;

namespace Workout.ViewModels.Workout
{
    /// <summary>
    /// Workout details view model.
    /// </summary>
    public class DetailsPageViewModel : IDisposable
    {
        #region fields

        /// <summary>
        /// Disposed flag.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Reference of WorkoutModel.
        /// </summary>
        private readonly WorkoutModel _workoutModel;

        #endregion

        #region properties

        /// <summary>
        /// Workout details header data.
        /// </summary>
        public DetailsHeaderData HeaderData
        {
            get;
            set;
        }

        /// <summary>
        /// Workout details items data.
        /// </summary>
        public List<DetailsItemData> ItemsData
        {
            get;
            set;
        }

        /// <summary>
        /// Finishes workout.
        /// </summary>
        public ICommand FinishCommand { get; private set; }

        #endregion

        #region methods

        /// <summary>
        /// Initializes commands.
        /// </summary>
        private void InitCommands()
        {
            FinishCommand = new Command(ExecuteFinish);
        }

        /// <summary>
        /// Handles execution of "FinishCommand".
        /// Finishes workout.
        /// </summary>
        private void ExecuteFinish()
        {
            Finish();
        }

        /// <summary>
        /// Handles "Updated" event of the WorkoutModel object.
        /// Updates value of local time.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="workoutData">Workout data.</param>
        private void OnWorkoutModelUpdated(object sender, WorkoutUpdatedEventArgs workoutData)
        {
            HeaderData.LocalTime = workoutData.Data.LocalTime;
        }

        /// <summary>
        /// Sets up events listeners.
        /// </summary>
        private void AddEventListeners()
        {
            _workoutModel.Updated += OnWorkoutModelUpdated;
        }

        /// <summary>
        /// Removes events listeners.
        /// </summary>
        private void RemoveEventListeners()
        {
            _workoutModel.Updated -= OnWorkoutModelUpdated;
        }

        /// <summary>
        /// Initializes class instance.
        /// </summary>
        public DetailsPageViewModel()
        {
            _workoutModel = WorkoutModel.Instance;

            _workoutModel.FinishWorkout();

            WorkoutData workoutData = _workoutModel.WorkoutData;

            HeaderData = new DetailsHeaderData
            {
                StartTime = workoutData.StartTime,
                LocalTime = workoutData.LocalTime,
                Title = "Workout\ndetails"
            };

            int[] bpmRangeOccurrences = workoutData.BpmRangeOccurrences;

            ItemsData = new List<DetailsItemData>
            {
                new DetailsItemData
                {
                    Name = "time",
                    Value = string.Format("{0:hh\\.mm\\\'ss}", workoutData.ElapsedTime),
                    Icon = "images/details_time_icon.png"
                },
                new DetailsItemData
                {
                    Name = "distance",
                    Value = (workoutData.Distance / 1000).ToString("F2") + " " + SettingsService.Instance.Distance.Unit,
                    Icon = "images/details_distance_icon.png"
                },
                new DetailsItemData
                {
                    Name = "average pace",
                    Value = workoutData.Pace.ToString("F2") + " min/" + SettingsService.Instance.Distance.Unit,
                    Icon = "images/details_average_pace_icon.png"
                },
                new DetailsItemData
                {
                    Name = "intensity",
                    Value = Array.LastIndexOf(bpmRangeOccurrences, bpmRangeOccurrences.Max()).ToString(),
                    Icon = "images/details_intensity_icon.png",
                    IsActionButtonVisible = true
                }
            };

            InitCommands();
            AddEventListeners();
        }

        /// <summary>
        /// Finishes workout.
        /// Clears workout data and navigates to home page.
        /// </summary>
        public void Finish()
        {
            _workoutModel.ClearWorkout();
            PageNavigationService.Instance.GoToHomePage();
        }

        /// <summary>
        /// Releases all resources currently used by this instance.
        /// </summary>
        /// <param name="disposing">
        /// True if managed resources should be disposed, false otherwise.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            RemoveEventListeners();
            _disposed = true;
        }

        /// <summary>
        /// Destroys the current object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Destroys the DetailsPageViewModel object.
        /// </summary>
        ~DetailsPageViewModel()
        {
            Dispose(false);
        }

        #endregion
    }
}
