using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumanResourcesApp.Classes;
using HumanResourcesApp.DBClasses;
using HumanResourcesApp.Models;

namespace HumanResourcesApp.ViewModels
{
    public partial class AttendanceViewModel : ObservableObject
    {
        private readonly HumanResourcesDB _context;

        [ObservableProperty]
        private ObservableCollection<Department> departments;

        [ObservableProperty]
        private ObservableCollection<EmployeeDisplayModel> employees;

        [ObservableProperty]
        private ObservableCollection<AttendanceDisplayModel> attendances;

        [ObservableProperty]
        private AttendanceDisplayModel selectedAttendance;

        [ObservableProperty]
        private AttendanceDisplayModel newAttendance;

        [ObservableProperty]
        private bool isAddingNew;

        [ObservableProperty]
        private string checkInTimeText = "08:00";

        [ObservableProperty]
        private string checkOutTimeText = "17:00";

        private EmployeeDisplayModel selectedEmployee;
        public EmployeeDisplayModel SelectedEmployee
        {
            get => selectedEmployee;
            set
            {
                SetProperty(ref selectedEmployee, value);
                if (value != null)
                {
                    var employee = _context.GetEmployeeById(value.EmployeeId);
                    if (employee != null)
                    {
                        NewAttendance.Employee = employee;
                        NewAttendance.EmployeeId = value.EmployeeId;
                        NewAttendance.EmployeeFullName = $"{value.FirstName} {value.LastName}";
                    }
                }
            }
        }

        private Department selectedDepartment;
        public Department SelectedDepartment
        {
            get => selectedDepartment;
            set
            {
                SetProperty(ref selectedDepartment, value);
                LoadEmployees(value);
            }
        }

        private string status;
        public string Status
        {
            get => status;
            set
            {
                SetProperty(ref status, value);
                if (value != null)
                {
                    NewAttendance.Status = value;
                }
            }
        }

        public AttendanceViewModel()
        {
            // Initialize commands
            _context = new HumanResourcesDB();

            // Initialize collections
            Attendances = new ObservableCollection<AttendanceDisplayModel>();
            NewAttendance = new AttendanceDisplayModel();
            Employees = new ObservableCollection<EmployeeDisplayModel>();
            Departments = new ObservableCollection<Department>();

            SelectedAttendance = new AttendanceDisplayModel();
            SelectedEmployee = new EmployeeDisplayModel();
            SelectedDepartment = new Department();
            Status = string.Empty;

            // Load data
            LoadAttendances();
            LoadDepartments();
        }

        private void LoadDepartments()
        {
            Departments.Clear();
            var departmentList = _context.GetAllDepartments();
            foreach (var department in departmentList)
            {
                Departments.Add(department);
            }
        }
        private void LoadAttendances()
        {
            Attendances.Clear();
            var attenadnceList = _context.GetAllAttendances();
            foreach (var attendance in attenadnceList)
            {
                Attendances.Add(new AttendanceDisplayModel {
                    EmployeeId = attendance.EmployeeId,
                    AttendanceId = attendance.AttendanceId,
                    CheckInTime = attendance.CheckInTime, 
                    CheckOutTime = attendance.CheckOutTime, 
                    CreatedAt = attendance.CreatedAt, 
                    Employee = attendance.Employee, 
                    EmployeeFullName = $"{attendance.Employee.FirstName} {attendance.Employee.LastName}", 
                    Notes = attendance.Notes, 
                    Status = attendance.Status,
                    WorkHours = attendance.WorkHours
                });
            }
        }

        private void LoadEmployees(Department? department = null)
        {
            Employees.Clear();
            if (department != null)
            {
                var employeeList = _context.GetEmployeesByDepartment(department);
                foreach (var emplloyee in employeeList)
                {
                    Employees.Add(new EmployeeDisplayModel
                    {
                        EmployeeId = emplloyee.EmployeeId,
                        FirstName = emplloyee.FirstName,
                        LastName = emplloyee.LastName,
                        Position = emplloyee.Position,
                        Email = emplloyee.Email,
                        Phone = emplloyee.Phone
                    });
                }
            }
            else
            {
                var employeeList = _context.GetAllEmplyees();
                foreach (var emplloyee in employeeList)
                {
                    Employees.Add(new EmployeeDisplayModel
                    {
                        EmployeeId = emplloyee.EmployeeId,
                        FirstName = emplloyee.FirstName,
                        LastName = emplloyee.LastName,
                        Position = emplloyee.Position,
                        Email = emplloyee.Email,
                        Phone = emplloyee.Phone
                    });
                }
            }
        }

        private bool TryParseTimeInput(string timeText, out TimeSpan time)
        {
            time = TimeSpan.Zero;

            // Try standard time format (HH:mm)
            if (TimeSpan.TryParse(timeText, out time))
            {
                return true;
            }

            // Try additional formats like "8:30 AM" or "2:45 PM"
            if (DateTime.TryParse(timeText, out DateTime dateTime))
            {
                time = dateTime.TimeOfDay;
                return true;
            }

            return false;
        }

        [RelayCommand]
        private void AddAttendance()
        {
            IsAddingNew = true;
            SelectedEmployee = null;
            SelectedAttendance = null;
            NewAttendance = new AttendanceDisplayModel
            {
                EmployeeFullName = string.Empty,
                Notes = string.Empty,
                Status = string.Empty,
                WorkHours = 0,
                CreatedAt = DateTime.Now
            };
            SelectedDepartment = null;
        }

        private decimal CalculateWorkHours(DateTime checkIn, DateTime checkOut)
        {
            var hours = (decimal)(checkOut - checkIn).TotalHours;
            return Math.Round(hours, 2);
        }   

        [RelayCommand]
        private void SaveNew()
        {
            if (IsAddingNew)
            {
                // Validate employee selection
                if (NewAttendance.Employee == null)
                {
                    MessageBox.Show("Employee field is required.");
                    return;
                }

                // Parse check-in date and time
                if (!TryParseTimeInput(CheckInTimeText, out TimeSpan checkInTime))
                {
                    MessageBox.Show("Invalid check-in time format. Please use HH:mm format (e.g. 08:30).");
                    return;
                }

                // Parse check-out date and time
                if (!TryParseTimeInput(CheckOutTimeText, out TimeSpan checkOutTime))
                {
                    MessageBox.Show("Invalid check-out time format. Please use HH:mm format (e.g. 17:30).");
                    return;
                }

                // Get dates from date pickers and combine with time
                DateTime? checkInDateTime = null;
                DateTime? checkOutDateTime = null;

                try
                {
                    // Assuming you have DatePicker values bound to these properties
                    var checkInDate = NewAttendance.CheckInTime?.Date ?? DateTime.Today;
                    var checkOutDate = NewAttendance.CheckOutTime?.Date ?? DateTime.Today;

                    checkInDateTime = checkInDate.Add(checkInTime);
                    checkOutDateTime = checkOutDate.Add(checkOutTime);

                    // Validate check-out is after check-in
                    if (checkOutDateTime <= checkInDateTime)
                    {
                        MessageBox.Show("Check-out time must be after check-in time.");
                        return;
                    }

                    // Set the parsed date/time values
                    NewAttendance.CheckInTime = checkInDateTime;
                    NewAttendance.CheckOutTime = checkOutDateTime;
                    NewAttendance.WorkHours = CalculateWorkHours((DateTime)checkInDateTime, (DateTime)checkOutDateTime);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error processing date/time values: {ex.Message}");
                    return;
                }

                // Create and save the attendance record
                var attendance = new Attendance
                {
                    EmployeeId = NewAttendance.Employee.EmployeeId,
                    CheckInTime = NewAttendance.CheckInTime,
                    CheckOutTime = NewAttendance.CheckOutTime,
                    WorkHours = NewAttendance.WorkHours,
                    Status = NewAttendance.Status,
                    Notes = NewAttendance.Notes,
                    CreatedAt = NewAttendance.CreatedAt
                };

                if (_context.IsAttendanceTimeValid(attendance))
                {
                    _context.AddAttendance(attendance);
                    LoadAttendances();
                    IsAddingNew = false;
                }
                else
                {
                    MessageBox.Show(
                        "This attendance record conflicts with an existing record for this employee.\n\n" +
                        "Please adjust the check-in or check-out times to avoid overlap with existing attendance periods.",
                        "Scheduling Conflict Detected",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            if (IsAddingNew)
            {
                IsAddingNew = false;
                newAttendance = new AttendanceDisplayModel();
            }
        }

        [RelayCommand]
        private void DeleteAttendance(AttendanceDisplayModel attendance)
        {
            if (attendance == null) return;

            var result = MessageBox.Show($"Are you sure you want to delete attendance info for '{SelectedAttendance.EmployeeFullName}'?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _context.DeleteAttendance(_context.GetAttendanceById(SelectedAttendance.AttendanceId));
                Attendances.Remove(attendance);
            }
        }
    }
}