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

            var studentRepository = new StudentRepository(_context); 
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
            var student = new Student
            {
                FirstName = "Jo1n", // Neteisingas simbolis varde
                LastName = "Smith",
                StudentNumber = 12345678,
                Email = "john.smith@example.com"
            };

            // Act
            var result = _studentService.CreateStudent(student);

            // Assert
            Assert.IsFalse(result, "Neteisingas FirstName.");
        }

        [TestMethod]
        public void CreateStudent_TooShortFirstName_ShouldReturnFalse()
        {
            // Arrange
            var student = new Student
            {
                FirstName = "J", // Per trumpas vardas
                LastName = "Smith",
                StudentNumber = 12345679,
                Email = "john.smith@example.com"
            };

            // Act
            var result = _studentService.CreateStudent(student);

            // Assert
            Assert.IsFalse(result, "Neteisingas FistName.");
        }

        [TestMethod]
        public void CreateStudent_TooLongFirstName_ShouldReturnFalse()
        {
            // Arrange
            var student = new Student
            {
                FirstName = "JohnathonJohnathonJohnathonJohnathonJohnathodfgagfdfn", // Per ilgas vardas
                LastName = "Smith",
                StudentNumber = 12345680,
                Email = "john.smith@example.com"
            };

            // Act
            var result = _studentService.CreateStudent(student);

            // Assert
            Assert.IsFalse(result, "Neteisingas FistName.");
        }

        [TestMethod]
        public void CreateStudent_InvalidLastNameWithSpecialCharacter_ShouldReturnFalse()
        {
            // Arrange
            var student = new Student
            {
                FirstName = "John",
                LastName = "Sm!th", // Neteisingas simbolis pavardëje
                StudentNumber = 12345681,
                Email = "john.smith@example.com"
            };

            // Act
            var result = _studentService.CreateStudent(student);

            // Assert
            Assert.IsFalse(result, "Neteisingas LastName.");
        }
        [TestMethod]
        public void CreateStudent_StudentNumberTooShort_ShouldReturnFalse()
        {
            // Arrange
            var student = new Student
            {
                FirstName = "John",
                LastName = "Smith",
                StudentNumber = 1234567, // 7 skaitmenys, per trumpas studento numeris
                Email = "john.smith@example.com"
            };

            // Act
            var result = _studentService.CreateStudent(student);

            // Assert
            Assert.IsFalse(result, "Per trumpas studento numeris");
        }
        [TestMethod]
        public void CreateStudent_StudentNumberTooLong_ShouldReturnFalse()
        {
            // Arrange
            var student = new Student
            {
                FirstName = "John",
                LastName = "Smith",
                StudentNumber = 123456789, // 9 skaitmenys, per ilgas studento numeris
                Email = "john.smith@example.com"
            };

            // Act
            var result = _studentService.CreateStudent(student);

            // Assert
            Assert.IsFalse(result, "Per ilgas studento numeris");
        }

        [TestMethod]
        public void CreateStudent_StudentNumberWithLetters_ShouldReturnFalse()
        {
            // Arrange
            var student = new Student
            {
                FirstName = "John",
                LastName = "Smith",
                StudentNumber = int.Parse("1234ABCD"), // Studento numeris su raidëmis
                Email = "john.smith@example.com"
            };

            // Act
            var result = _studentService.CreateStudent(student);

            // Assert
            Assert.IsFalse(result, "Studento numeris su raidëmis");
        }

        [TestMethod]
        public void CreateStudent_InvalidEmailWithoutAtSymbol_ShouldReturnFalse()
        {
            // Arrange
            var student = new Student
            {
                FirstName = "John",
                LastName = "Smith",
                StudentNumber = 12345678,
                Email = "john.smithexample.com" // Trûksta '@' simbolio elektroninio paðto adrese
            };

            // Act
            var result = _studentService.CreateStudent(student);

            // Assert
            Assert.IsFalse(result, "Trûksta '@' simbolio elektroninio paðto adrese");
        }

        [TestMethod]
        public void CreateStudent_InvalidEmailWithoutDomain_ShouldReturnFalse()
        {
            // Arrange
            var student = new Student
            {
                FirstName = "John",
                LastName = "Smith",
                StudentNumber = 12345678,
                Email = "john.smith@" // Trûksta domeno elektroninio paðto adrese
            };

            // Act
            var result = _studentService.CreateStudent(student);

            // Assert
            Assert.IsFalse(result, "Trûksta domeno elektroninio paðto adrese");
        }

        [TestMethod]
        public void CreateStudent_InvalidEmailWithoutTld_ShouldReturnFalse()
        {
            // Arrange
            var student = new Student
            {
                FirstName = "John",
                LastName = "Smith",
                StudentNumber = 12345678,
                Email = "john.smith@example" // Trûksta domeno elektroninio paðto adrese
            };

            // Act
            var result = _studentService.CreateStudent(student);

            // Assert
            Assert.IsFalse(result, "Trûksta domeno elektroninio paðto adrese");
        }

        [TestMethod]
        public void CreateStudent_InvalidEmailWithIncompleteTld_ShouldReturnFalse()
        {
            // Arrange
            var student = new Student
            {
                FirstName = "John",
                LastName = "Smith",
                StudentNumber = 12345678,
                Email = "john.smith@example." // Trûksta domeno elektroninio paðto adrese
            };

            // Act
            var result = _studentService.CreateStudent(student);

            // Assert
            Assert.IsFalse(result, "Trûksta domeno elektroninio paðto adrese");
        }

        [TestMethod]
        public void CreateStudent_EmptyFirstName_ShouldReturnFalse()
        {
            // Arrange
            var student = new Student
            {
                FirstName = "", // Tuðèias vardas
                LastName = "Smith",
                StudentNumber = 12345678,
                Email = "john.smith@example.com"
            };

            // Act
            var result = _studentService.CreateStudent(student);

            // Assert
            Assert.IsFalse(result, "FirstName yra privalomas");
        }

        [TestMethod]
        public void CreateStudent_EmptyLastName_ShouldReturnFalse()
        {
            // Arrange
            var student = new Student
            {
                FirstName = "John",
                LastName = "", //Tuðèia pavardë
                StudentNumber = 12345678,
                Email = "john.smith@example.com"
            };

            // Act
            var result = _studentService.CreateStudent(student);

            // Assert
            Assert.IsFalse(result, "LastName yra privalomas");
        }

        [TestMethod]
        public void CreateStudent_MissingDepartment_ShouldReturnFalse()
        {
            // Arrange
            var student = new Student
            {
                FirstName = "John",
                LastName = "Smith",
                StudentNumber = 12345678,
                Email = "john.smith@example.com",
                DepartmentCode = "" // Nenurodytas studento fakulteto kodas
            };

            // Act
            var result = _studentService.CreateStudent(student);

            // Assert
            Assert.IsFalse(result, "Departamentas yra privalomas");
        }
        [TestMethod]
        public void CreateStudent_DuplicateEmail_ShouldReturnTrue()
        {
            // Arrange
            var existingStudent = new Student
            {
                StudentNumber = 22222222,
                FirstName = "Alice",
                LastName = "Johnson",
                Email = "alice.johnson@example.com",
                DepartmentCode = "MTH567"
            };

            _context.Students.Add(existingStudent);
            _context.SaveChanges();

            var newStudent = new Student
            {
                StudentNumber = 11111111,
                FirstName = "John",
                LastName = "Smith",
                Email = "alice.johnson@example.com", // Pasikartojantis elektroninio paðto adresas
                DepartmentCode = "CS1234"
            };

            // Act
            var result = _studentService.CreateStudent(newStudent);

            // Assert
            Assert.IsTrue(result, "Studentas su tokiu el. paðtu jau egzistuoja");
        }
    }
}