using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .UseInMemoryDatabase(databaseName: "TestDatabaseCSV")
                .Options;

            _context = new StudentsContext(options);

            _context.Departments.AddRange(CSVDataService.GetDepartments());
            _context.Lectures.AddRange(CSVDataService.GetLectures());
            _context.Students.AddRange(CSVDataService.GetStudents());

            _context.SaveChanges();

            _context.Set<object>().AddRange(CSVDataService.GetDepartmentLectures());
            _context.Set<object>().AddRange(CSVDataService.GetStudentLectures());

            _context.SaveChanges();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public void TestDepartmentsLoadedFromCSV()
        {
            Assert.AreEqual(2, _context.Departments.Count());

            var csDepartment = _context.Departments.FirstOrDefault(d => d.DepartmentCode == "CS1234");
            var mathDepartment = _context.Departments.FirstOrDefault(d => d.DepartmentCode == "MTH567");

            Assert.IsNotNull(csDepartment);
            Assert.AreEqual("ComputerScience", csDepartment.DepartmentName);

            Assert.IsNotNull(mathDepartment);
            Assert.AreEqual("Mathematics", mathDepartment.DepartmentName);
        }

        [TestMethod]
        public void TestLecturesLoadedFromCSV()
        {
            Assert.AreEqual(3, _context.Lectures.Count());

            var algorithms = _context.Lectures.FirstOrDefault(l => l.LectureName == "Algorithms");
            var calculus = _context.Lectures.FirstOrDefault(l => l.LectureName == "Calculus");
            var dataStructures = _context.Lectures.FirstOrDefault(l => l.LectureName == "DataStructures");

            Assert.IsNotNull(algorithms);
            Assert.AreEqual("10:00", algorithms.LectureTime);

            Assert.IsNotNull(calculus);
            Assert.AreEqual("12:00", calculus.LectureTime);

            Assert.IsNotNull(dataStructures);
            Assert.AreEqual("14:00", dataStructures.LectureTime);
        }

        [TestMethod]
        public void TestStudentsLoadedFromCSV()
        {
            Assert.AreEqual(2, _context.Students.Count());

            var john = _context.Students.FirstOrDefault(s => s.FirstName == "John" && s.LastName == "Smith");
            var alice = _context.Students.FirstOrDefault(s => s.FirstName == "Alice" && s.LastName == "Johnson");

            Assert.IsNotNull(john);
            Assert.AreEqual(12345678, john.StudentNumber);

            Assert.IsNotNull(alice);
            Assert.AreEqual(87654321, alice.StudentNumber);
        }

        [TestMethod]
        public void TestDepartmentLectureAssociationsFromCSV()
        {
            var csDepartment = _context.Departments.FirstOrDefault(d => d.DepartmentCode == "CS1234");
            var mathDepartment = _context.Departments.FirstOrDefault(d => d.DepartmentCode == "MTH567");

            Assert.IsTrue(csDepartment.Lectures.Any(l => l.LectureName == "Algorithms"));
            Assert.IsTrue(csDepartment.Lectures.Any(l => l.LectureName == "DataStructures"));

            Assert.IsTrue(mathDepartment.Lectures.Any(l => l.LectureName == "Calculus"));
        }

        [TestMethod]
        public void TestStudentLectureAssociationsFromCSV()
        {
            var john = _context.Students.FirstOrDefault(s => s.StudentNumber == 12345678);
            var alice = _context.Students.FirstOrDefault(s => s.StudentNumber == 87654321);

            Assert.IsTrue(john.Lectures.Any(l => l.LectureName == "Algorithms"));
            Assert.IsTrue(john.Lectures.Any(l => l.LectureName == "DataStructures"));

            Assert.IsTrue(alice.Lectures.Any(l => l.LectureName == "Calculus"));
        }
    }
}
