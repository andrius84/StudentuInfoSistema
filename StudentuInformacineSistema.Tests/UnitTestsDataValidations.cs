using Microsoft.VisualStudio.TestTools.UnitTesting;
using StudentuInformacineSistema.Database.Entities;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace StudentuInformacineSistema.Tests
{
    [TestClass]
    public class CSVDataServiceTests
    {
        private StudentsContext _context;

        [TestInitialize]
        public void TestInitialize()
        {
            var options = new DbContextOptionsBuilder<StudentsContext>()
                .UseInMemoryDatabase(databaseName: "TestDataDatabase")
                .Options;

            _context = new StudentsContext(options);

            _context.Departments.AddRange(
                new Department { DepartmentCode = "CS1234", DepartmentName = "ComputerScience" },
                new Department { DepartmentCode = "MTH567", DepartmentName = "Mathematics" }
            );

            _context.Lectures.AddRange(
                new Lecture { LectureName = "Algorithms", LectureTime = "10:00-11:30" },
                new Lecture { LectureName = "Calculus", LectureTime = "12:00-13:30" },
                new Lecture { LectureName = "DataStructures", LectureTime = "14:00-15:30" }
            );

            _context.Students.AddRange(
                new Student
                {
                    StudentNumber = 12345678,
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "john.smith@example.com",
                    DepartmentCode = "CS1234"
                },
                new Student
                {
                    StudentNumber = 87654321,
                    FirstName = "Alice",
                    LastName = "Johnson",
                    Email = "alice.johnson@example.com",
                    DepartmentCode = "MTH567"
                }
            );

            _context.SaveChanges();

            var departmentLectures = new List<object>
            {
                new { DepartmentsDepartmentCode = "CS1234", LecturesLectureName = "Algorithms" },
                new { DepartmentsDepartmentCode = "CS1234", LecturesLectureName = "DataStructures" },
                new { DepartmentsDepartmentCode = "MTH567", LecturesLectureName = "Calculus" }
            };

            var studentLectures = new List<object>
            {
                new { StudentsStudentNumber = 12345678, LecturesLectureName = "Algorithms" },
                new { StudentsStudentNumber = 12345678, LecturesLectureName = "DataStructures" },
                new { StudentsStudentNumber = 87654321, LecturesLectureName = "Calculus" }
            };

            _context.SaveChanges();
        }

        [TestMethod]
        public void TestDepartmentsInsertedCorrectly()
        {
            var departments = _context.Departments.ToList();
            Assert.AreEqual(2, departments.Count);
            Assert.IsTrue(departments.Any(d => d.DepartmentCode == "CS1234" && d.DepartmentName == "ComputerScience"));
            Assert.IsTrue(departments.Any(d => d.DepartmentCode == "MTH567" && d.DepartmentName == "Mathematics"));
        }

        [TestMethod]
        public void TestLecturesInsertedCorrectly()
        {
            var lectures = _context.Lectures.ToList();
            Assert.AreEqual(3, lectures.Count);
            Assert.IsTrue(lectures.Any(l => l.LectureName == "Algorithms" && l.LectureTime == "10:00-11:30"));
            Assert.IsTrue(lectures.Any(l => l.LectureName == "Calculus" && l.LectureTime == "12:00-13:30"));
            Assert.IsTrue(lectures.Any(l => l.LectureName == "DataStructures" && l.LectureTime == "14:00-15:30"));
        }

        [TestMethod]
        public void TestStudentsInsertedCorrectly()
        {
            var students = _context.Students.ToList();
            Assert.AreEqual(2, students.Count);
            Assert.IsTrue(students.Any(s => s.StudentNumber == 12345678 && s.FirstName == "John" && s.LastName == "Smith"));
            Assert.IsTrue(students.Any(s => s.StudentNumber == 87654321 && s.FirstName == "Alice" && s.LastName == "Johnson"));
        }
    }
}





/*
Kai įrašomi departamentų duomenys iš departments.csv
- Sukurti du departamentai su kodais CS1234 ir MTH567
- Departamentų pavadinimai turi būti ComputerScience ir Mathematics
Kai įrašomi paskaitų duomenys iš lectures.csv, duomenys turi būti
- Sukurtos trys paskaitos: Algorithms, Calculus, DataStructures.
- Paskaitų laikai turi būti 10:00, 12:00, 14:00
Kai įrašomi studentų duomenys iš students.csv, duomenys turi būti:
- Sukurti du studentai: John Smith ir Alice Johnson.
Kai įrašomi departamentų ir paskaitų asociacijų duomenys iš department_lectures.csv, duomenys turi būti:
- Departamentas CS1234 turi būti susietas su paskaitomis Algorithms ir DataStructures.
- Departamentas MTH567 turi būti susietas su paskaitomis Calculus.
Kai įrašomi studentų ir paskaitų asociacijų duomenys iš student_lectures.csv, duomenys turi būti:
- Studentas 12345678 (John Smith) turi būti užregistruotas į paskaitas Algorithms ir DataStructures.
- Studentas 87654321 (Alice Johnson) turi būti užregistruotas į paskaitą Calculus.
*/