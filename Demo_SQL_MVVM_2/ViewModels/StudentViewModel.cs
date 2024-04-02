using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo_SQL_MVVM_2.Models;
using Microsoft.Data.SqlClient;

namespace Demo_SQL_MVVM_2.ViewModels
{
    class StudentViewModel
    {
        public ObservableCollection<Student> Students { get; set; } = new ObservableCollection<Student>();
        public Student selectedStudent { get; set; }

        public StudentViewModel() 
        {
            //Students = new ObservableCollection<Student>()
            //{
            //    new Student() {ID=123, Name="Nguyen Thanh Hung", Score=0},
            //    new Student() {ID=456, Name="Tran Thi Danh", Score =0},
            //    new Student() {ID=789, Name="Lam Van Duy", Score=1}
            //};

            string connetionString;
            SqlConnection cnn;
            connetionString = @"Server=BIXCUIT;
                             Database=DB_Student;
                             Trusted_Connection=yes;
                             TrustServerCertificate=True";
            cnn = new SqlConnection(connetionString);
            cnn.Open();

            SqlCommand command;
            SqlDataReader reader;
            string sql, Output = "";

            sql = "Select * from Student";

            command = new SqlCommand(sql, cnn);

            reader = command.ExecuteReader();

            while (reader.Read())
            {
                Students.Add(new Student()
                {
                    ID = (int)reader.GetValue(0),
                    Name = reader.GetValue(1).ToString(),
                    Score = (int)reader.GetValue(2)
                });
            }
            
            cnn.Close();

            //selectedStudent = Students[0];
        }
    }
}
