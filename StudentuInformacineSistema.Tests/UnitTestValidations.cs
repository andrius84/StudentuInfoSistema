using Microsoft.EntityFrameworkCore;
using StudentuInformacineSistema.Database.Entities;
using StudentuInformacineSistema.Database.Repositories;
using StudentuInformacineSistema.Services;

namespace StudentuInformacineSistema.Tests
{
    [TestClass]
    public class StudentServiceTests
    {
        private StudentsContext _context;
        private StudentService _studentService;

        [TestInitialize]
        public void TestInitialize()
        {
            var options = new DbContextOptionsBuilder<StudentsContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new StudentsContext(options);
            _context.Database.EnsureCreated();

            var studentRepository = new StudentRepository(_context); // Assuming StudentRepository implements IStudentRepository
            _studentService = new StudentService(studentRepository);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public void CreateStudent_InvalidFirstNameWithNumber_ShouldReturnFalse()
        {
            // Arrange
            var student = new Student
            {
                FirstName = "Jo1n", // Invalid name (contains a number)
                LastName = "Smith",
                StudentNumber = 12345678,
                Email = "john.smith@example.com"
            };

            // Act
            var result = _studentService.CreateStudent(student);

            // Assert
            Assert.IsFalse(result, "Student creation should fail due to invalid first name with a number.");
        }

        [TestMethod]
        public void CreateStudent_TooShortFirstName_ShouldReturnFalse()
        {
            // Arrange
            var student = new Student
            {
                FirstName = "J", // Invalid name (less than 2 characters)
                LastName = "Smith",
                StudentNumber = 12345679,
                Email = "john.smith@example.com"
            };

            // Act
            var result = _studentService.CreateStudent(student);

            // Assert
            Assert.IsFalse(result, "Student creation should fail due to too short first name.");
        }

        [TestMethod]
        public void CreateStudent_TooLongFirstName_ShouldReturnFalse()
        {
            // Arrange
            var student = new Student
            {
                FirstName = "JohnathonJohnathonJohnathonJohnathonJohnathon", // 51 characters
                LastName = "Smith",
                StudentNumber = 12345680,
                Email = "john.smith@example.com"
            };

            // Act
            var result = _studentService.CreateStudent(student);

            // Assert
            Assert.IsFalse(result, "Student creation should fail due to too long first name.");
        }

        [TestMethod]
        public void CreateStudent_InvalidLastNameWithSpecialCharacter_ShouldReturnFalse()
        {
            // Arrange
            var student = new Student
            {
                FirstName = "John",
                LastName = "Sm!th", // Invalid last name (contains a special character)
                StudentNumber = 12345681,
                Email = "john.smith@example.com"
            };

            // Act
            var result = _studentService.CreateStudent(student);

            // Assert
            Assert.IsFalse(result, "Student creation should fail due to invalid last name with a special character.");
        }
    }
}