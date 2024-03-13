using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMDemo
{
    class StudentViewModel
    {
        public ObservableCollection<Student> Students { get; set; }
        public Student selectedStudent { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public string Name { get; set; }
        public StudentViewModel()
        {
            Students = new ObservableCollection<Student>()
            {
                new Student() {ID="1", Name="Nguyen Thanh Hung", Score=0},
                new Student() {ID="2", Name="Tran Thi Danh", Score =0},
                new Student() {ID="3", Name="Lam Van Duy", Score=1}
            };
            selectedStudent = Students[0];
            DeleteCommand = new RelayCommand(OnDelete, CanDelete);
        }
        private void OnDelete(object value)
        {
            Students.Remove(selectedStudent);
        }

        private bool CanDelete(object value)
        {
            return selectedStudent != null;
        }

    }
}
