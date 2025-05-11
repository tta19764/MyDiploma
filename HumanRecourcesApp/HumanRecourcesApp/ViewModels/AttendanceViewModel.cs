using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

        private readonly User _user;

        [ObservableProperty] private ObservableCollection<Department> departments;
        [ObservableProperty] private string checkAction;
        [ObservableProperty] private bool isEmployee;
        [ObservableProperty] private ObservableCollection<EmployeeDisplayModel> employees;
        [ObservableProperty] private ObservableCollection<AttendanceDisplayModel> attendances;
        [ObservableProperty] private AttendanceDisplayModel selectedAttendance;
        [ObservableProperty] private AttendanceDisplayModel newAttendance;
        [ObservableProperty] private bool isAddingNewOrEditing = false;
        [ObservableProperty] private bool isEditing = false;
        [ObservableProperty] private string checkInTimeText = "08:00";
        [ObservableProperty] private string checkOutTimeText = "17:00";
        private EmployeeDisplayModel selectedEmployee = new EmployeeDisplayModel();
        [ObservableProperty] private List<string?> statuses = new List<string?>() { "Checked In", "Checked Out", "On Leave", "Absent", "Late" };

        [ObservableProperty] private bool canManageAttendances = false;
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

        private Department selectedDepartment = new Department();
        public Department SelectedDepartment
        {
            get => selectedDepartment;
            set
            {
                SetProperty(ref selectedDepartment, value);
                LoadEmployees(value);
            }
        }

        private string status = string.Empty;
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

        public AttendanceViewModel(User user)
        {
            // Initialize commands
            _context = new HumanResourcesDB();
            _user = _context.GetUserById(user.UserId) ?? new User();
            IsEmployee = _user.Employee != null;
            if (IsEmployee && _user.Employee != null && _context.GetAllAttendances().Where(a => a.EmployeeId == _user.Employee.EmployeeId && a.CheckInTime.HasValue && a.CheckInTime.Value.Date == DateTime.Now.Date).Count() > _context.GetAllAttendances().Where(a => a.EmployeeId == _user.Employee.EmployeeId && a.CheckOutTime.HasValue && a.CheckOutTime.Value.Date == DateTime.Now.Date).Count())
            {
                CheckAction = "Check Out";
            }
            else if(IsEmployee)
            {
                CheckAction = "Check In";
            }
            else
            {
                CheckAction = string.Empty;
            }

            // Initialize collections
            Attendances = new ObservableCollection<AttendanceDisplayModel>();
            NewAttendance = new AttendanceDisplayModel();
            Employees = new ObservableCollection<EmployeeDisplayModel>();
            Departments = new ObservableCollection<Department>();

            SelectedAttendance = new AttendanceDisplayModel();
            SelectedEmployee = new EmployeeDisplayModel();
            SelectedDepartment = new Department();
            Status = string.Empty;

            CanManageAttendances = _context.HasPermission(_user, "ManageAttendance");

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
        private void Check()
        {
            if(CheckAction == "Check In" && _user.Employee != null)
            {
                var attendance = new Attendance
                {
                    EmployeeId = _user.Employee.EmployeeId,
                    CheckInTime = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    Status = "Checked In"
                };
                _context.CheckIn(_user, attendance);
                LoadAttendances();
                CheckAction = "Check Out";
            }
            else if(CheckAction == "Check Out" && _user.Employee != null)
            {
                var attendance = _context.GetAttendancesByEmployeeId(_user.Employee.EmployeeId)
                    .Where(at => at.CheckInTime.HasValue && at.CheckInTime.Value.Date == DateTime.Now.Date && !at.CheckOutTime.HasValue).FirstOrDefault();
                if (attendance != null)
                {
                    attendance.CheckOutTime = DateTime.Now;
                    attendance.Status = "Checked Out";
                    DateTime checkInTime = attendance.CheckInTime ?? DateTime.Now;
                    attendance.WorkHours = CalculateWorkHours(checkInTime, (DateTime)attendance.CheckOutTime);
                    _context.CheckOut(_user, attendance);
                    LoadAttendances();
                    CheckAction = "Check In";
                }
            }
        }


        [RelayCommand]
        private void AddAttendance()
        {
            try
            {
                IsAddingNewOrEditing = true;

                CheckInTimeText = "08:00";
                CheckOutTimeText = "17:00";

                string employeeName = string.Empty;
                if (_user.Employee != null)
                {
                    var employeeDepartment = Departments.FirstOrDefault(d => d.DepartmentId == _user.Employee.DepartmentId);
                    if (employeeDepartment != null) SelectedDepartment = employeeDepartment;
                    employeeName = $"{_user.Employee.FirstName} {_user.Employee.LastName}";
                    var attendanceEmployee = Employees.FirstOrDefault(e => e.EmployeeId == _user.Employee.EmployeeId);
                    if (attendanceEmployee != null) SelectedEmployee = attendanceEmployee;
                }

                NewAttendance = new AttendanceDisplayModel
                {
                    EmployeeFullName = employeeName,
                    EmployeeId = _user.Employee?.EmployeeId ?? 0,
                    Notes = string.Empty,
                    WorkHours = 0,
                    CreatedAt = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                _context.LogError(_user, "AddAttendance", ex);
            }
        }

        [RelayCommand]
        private void EditAttendance(AttendanceDisplayModel attendance)
        {

            try
            {
                var attendanceToEdit = _context.GetAttendanceById(attendance.AttendanceId);
                if (attendance == null || attendanceToEdit == null) throw new Exception("Attendance record not found.");

                IsAddingNewOrEditing = true;
                IsEditing = true;

                // Load the department for this employee
                if (attendanceToEdit.Employee != null && attendanceToEdit.Employee.DepartmentId > 0)
                {
                    var employeeDepartment = Departments.FirstOrDefault(d => d.DepartmentId == attendanceToEdit.Employee.DepartmentId);
                    if (employeeDepartment != null) SelectedDepartment = employeeDepartment;
                }

                // Set selected employee
                var attendanceEmployee = Employees.FirstOrDefault(e => e.EmployeeId == attendanceToEdit.EmployeeId);
                if (attendanceEmployee != null) SelectedEmployee = attendanceEmployee;

                // Create a copy for editing
                NewAttendance = new AttendanceDisplayModel
                {
                    AttendanceId = attendanceToEdit.AttendanceId,
                    EmployeeId = attendanceToEdit.EmployeeId,
                    EmployeeFullName = $"{attendanceToEdit.Employee?.FirstName} {attendanceToEdit.Employee?.LastName}",
                    CheckInTime = attendanceToEdit.CheckInTime,
                    CheckOutTime = attendanceToEdit.CheckOutTime,
                    Status = Statuses.FirstOrDefault(s => s == attendanceToEdit.Status),
                    Notes = attendanceToEdit.Notes,
                    WorkHours = attendanceToEdit.WorkHours,
                    Employee = attendanceToEdit.Employee ?? new Employee(),
                    CreatedAt = attendanceToEdit.CreatedAt
                };

                // Set time text fields for the form
                if (attendanceToEdit.CheckInTime.HasValue)
                {
                    CheckInTimeText = attendanceToEdit.CheckInTime.Value.ToString("HH:mm");
                }

                if (attendanceToEdit.CheckOutTime.HasValue)
                {
                    CheckOutTimeText = attendanceToEdit.CheckOutTime.Value.ToString("HH:mm");
                }
            }
            catch (Exception ex)
            {
                _context.LogError(_user, "EditAttendance", ex);
            }
        }

        private decimal CalculateWorkHours(DateTime checkIn, DateTime checkOut)
        {
            var hours = (decimal)(checkOut - checkIn).TotalHours;
            return Math.Round(hours, 2);
        }   

        [RelayCommand]
        private void SaveNewAttendance()
        {
            if (IsAddingNewOrEditing)
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
                    _context.LogError(_user, "SaveNewAttendance", ex);
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

                if (IsEditing)
                {
                    attendance.AttendanceId = NewAttendance.AttendanceId;
                }


                    if (_context.IsAttendanceTimeValid(attendance))
                {
                    try
                    {
                        if (IsEditing)
                        {
                            _context.UpdateAttendance(_user, attendance);
                        }
                        else
                        {
                            _context.AddAttendance(_user, attendance);
                        }
                        LoadAttendances();
                        IsAddingNewOrEditing = false;
                        IsEditing = false;
                    }
                    catch (Exception ex)
                    {
                        _context.LogError(_user, "SaveNewAttendance", ex);
                    }
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
            if (IsAddingNewOrEditing)
            {
                IsEditing = false;
                IsAddingNewOrEditing = false;
                NewAttendance = new AttendanceDisplayModel();
            }
        }

        [RelayCommand]
        private void DeleteAttendance(AttendanceDisplayModel attendance)
        {
            try
            {
                var attendanceToDelete = _context.GetAttendanceById(attendance.AttendanceId);
                if (attendanceToDelete == null)
                {
                    throw new Exception("Attendance record not found.");
                }
                else
                {
                    var result = MessageBox.Show($"Are you sure you want to delete attendance info for '{attendance.EmployeeFullName}'?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _context.DeleteAttendance(_user, attendanceToDelete);
                        Attendances.Remove(attendance);
                    }
                }
            }
            catch (Exception ex)
            {
                _context.LogError(_user, "DeleteAttendance", ex);
            }
        }
    }
}