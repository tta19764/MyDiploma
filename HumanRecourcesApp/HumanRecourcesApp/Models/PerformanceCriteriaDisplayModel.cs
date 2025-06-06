﻿using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResourcesApp.Models
{
    public class PerformanceCriteriaDisplayModel : ObservableObject
    {
        public int CriteriaId { get; set; }
        public string CriteriaName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal WeightPercentage { get; set; }

        private decimal _score;
        public decimal Score
        {
            get => _score;
            set => SetProperty(ref _score, value);
        }

        private string _comments = string.Empty;
        public string Comments
        {
            get => _comments;
            set => SetProperty(ref _comments, value);
        }
    }
}
