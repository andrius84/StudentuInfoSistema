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
            // Create InMemory database options
            var options = new DbContextOptionsBuilder<StudentsContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabaseCSV")
                .Options;

            _context = new StudentsContext(options);

            // Seed data into the context using CSVDataService
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

        // Test for Departments
        [TestMethod]
        public void TestDepartmentsLoadedFromCSV()
        {
            // Assert that two departments were created
            Assert.AreEqual(2, _context.Departments.Count());

            // Assert department codes and names
            var csDepartment = _context.Departments.FirstOrDefault(d => d.DepartmentCode == "CS1234");
            var mathDepartment = _context.Departments.FirstOrDefault(d => d.DepartmentCode == "MTH567");

            Assert.IsNotNull(csDepartment);
            Assert.AreEqual("ComputerScience", csDepartment.DepartmentName);

            Assert.IsNotNull(mathDepartment);
            Assert.AreEqual("Mathematics", mathDepartment.DepartmentName);
        }

        // Test for Lectures
        [TestMethod]
        public void TestLecturesLoadedFromCSV()
        {
            // Assert that three lectures were created
            Assert.AreEqual(3, _context.Lectures.Count());

            // Assert lecture names and times
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

        // Test for Students
        [TestMethod]
        public void TestStudentsLoadedFromCSV()
        {
            // Assert that two students were created
            Assert.AreEqual(2, _context.Students.Count());

            // Assert student details
            var john = _context.Students.FirstOrDefault(s => s.FirstName == "John" && s.LastName == "Smith");
            var alice = _context.Students.FirstOrDefault(s => s.FirstName == "Alice" && s.LastName == "Johnson");

            Assert.IsNotNull(john);
            Assert.AreEqual(12345678, john.StudentNumber);

            Assert.IsNotNull(alice);
            Assert.AreEqual(87654321, alice.StudentNumber);
        }

        // Test for Department-Lecture associations
        [TestMethod]
        public void TestDepartmentLectureAssociationsFromCSV()
        {
            var csDepartment = _context.Departments.FirstOrDefault(d => d.DepartmentCode == "CS1234");
            var mathDepartment = _context.Departments.FirstOrDefault(d => d.DepartmentCode == "MTH567");

            // Assert CS1234 is associated with Algorithms and DataStructures
            Assert.IsTrue(csDepartment.Lectures.Any(l => l.LectureName == "Algorithms"));
            Assert.IsTrue(csDepartment.Lectures.Any(l => l.LectureName == "DataStructures"));

            // Assert MTH567 is associated with Calculus
            Assert.IsTrue(mathDepartment.Lectures.Any(l => l.LectureName == "Calculus"));
        }

        // Test for Student-Lecture associations
        [TestMethod]
        public void TestStudentLectureAssociationsFromCSV()
        {
            var john = _context.Students.FirstOrDefault(s => s.StudentNumber == 12345678);
            var alice = _context.Students.FirstOrDefault(s => s.StudentNumber == 87654321);

            // Assert John is registered for Algorithms and DataStructures
            Assert.IsTrue(john.Lectures.Any(l => l.LectureName == "Algorithms"));
            Assert.IsTrue(john.Lectures.Any(l => l.LectureName == "DataStructures"));

            // Assert Alice is registered for Calculus
            Assert.IsTrue(alice.Lectures.Any(l => l.LectureName == "Calculus"));
        }
    }
}
