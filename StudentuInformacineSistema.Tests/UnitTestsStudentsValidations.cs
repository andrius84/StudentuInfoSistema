using Microsoft.EntityFrameworkCore;
using StudentuInformacineSistema.Database.Entities;
using StudentuInformacineSistema.Database.Repositories;
using StudentuInformacineSistema.Services;

namespace StudentuInformacineSistema.Tests
{
    [TestClass]
    public class UnitTestsStudentsValidations
    {
        private StudentsContext _context;
        private StudentService _studentService;

        [TestInitialize]
        public void TestInitialize()
        {
            var options = new DbContextOptionsBuilder<StudentsContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
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
            // Arrange
            var student = new Student
            {
                FirstName = "Jo1n", // Neteisingas simbolis varde
                LastName = "Smith",
                StudentNumber = 12345678,
                Email = "john.smith@example.com",
                DepartmentCode = "KA1256"
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
                Email = "john.smith@example.com",
                DepartmentCode = "KA1256"
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
                Email = "john.smith@example.com",
                DepartmentCode = "KA1256"
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
                Email = "john.smith@example.com",
                DepartmentCode = "KA1256"
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
                Email = "john.smith@example.com",
                DepartmentCode = "KA1256"
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
                StudentNumber = 123456789, // 9 skaitmenys, per ilgas studento numeris
                FirstName = "John",
                LastName = "Smith",
                Email = "john.smith@example.com",
                DepartmentCode = "KA1256",
            };

            // Act
            var result = _studentService.CreateStudent(student);

            // Assert
            Assert.IsFalse(result, "Per ilgas studento numeris");
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
                Email = "john.smith@example.com",
                DepartmentCode = "KA1256"
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
                Email = "john.smith@example.com",
                DepartmentCode = "KA1256"
            };

            // Act
            var result = _studentService.CreateStudent(student);

            // Assert
            Assert.IsFalse(result, "LastName yra privalomas");
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
                Email = "john.smithexample.com", // Trûksta '@' simbolio elektroninio paðto adrese
                DepartmentCode = "KA1256"
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
                Email = "john.smith@", // Trûksta domeno elektroninio paðto adrese
                DepartmentCode = "KA1256"
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
                Email = "john.smith@example", // Trûksta domeno elektroninio paðto adrese
                DepartmentCode = "KA1256"
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
                Email = "john.smith@example.", // Trûksta domeno elektroninio paðto adrese
                DepartmentCode = "KA1256"
            };

            // Act
            var result = _studentService.CreateStudent(student);

            // Assert
            Assert.IsFalse(result, "Trûksta domeno elektroninio paðto adrese");
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

        [TestMethod]
        public void AssignStudent_NonExistentDepartment_ShouldReturnFalse()
        {
            var student = new Student
            {
                StudentNumber = 12345679,
                FirstName = "John",
                LastName = "Smith",
                Email = "alice.johnson@example.com",
                DepartmentCode = "ENG999" // neegzistuojantis departamentas
            };

            // Act
            var result = _studentService.AddDepartmentToStudent(studentNumber: 12345679, departmentCode: "ENG999");
           
            // Assert
            Assert.IsFalse(result, "Studentas negali bûti priskirtas á neegzituojantá departamentà");
        }

        [TestMethod]
        public void TransferStudent_ToAnotherDepartment_ShouldUpdateTrue()
        {
            var student = new Student
            {
                StudentNumber = 12345678,
                FirstName = "Alice",
                LastName = "Johnson",
                Email = "alice.johnson@example.com",
                DepartmentCode = "CS1234"
            };

            // Act
            var result = _studentService.AddDepartmentToStudent(studentNumber: 12345678, departmentCode: "MTH567");
            
            // Assert
            Assert.IsTrue(result, "Studentas perkeltas sëkmingai");
        }
    }
}

/*
  
   - Jei paduodami studento vardas "Jo1n" ir pavardë "Smith", tai gaunama klaida, nes vardas turi bûti sudarytas tik ið raidþiø.
   - Jei paduodami studento vardas "J" ir pavardë "Smith", tai gaunama klaida, nes vardas turi bûti ne trumpesnis kaip 2 simboliai.
   - Jei paduodami studento vardas "JohnathonJohnathonJohnathonJohnathonJohnathon" (51 simbolis) ir pavardë "Smith", tai gaunama klaida, nes vardas turi bûti ne ilgesnis kaip 50 simboliø.
   - Jei paduodami studento vardas "John" ir pavardë "Sm!th", tai gaunama klaida, nes pavardë turi bûti sudaryta tik ið raidþiø.
   - Jei paduodami studento numeris "1234567" (7 skaitmenys), tai gaunama klaida, nes numeris turi bûti tiksliai 8 simboliø ilgio.
   - Jei paduodami studento numeris "123456789" (9 skaitmenys), tai gaunama klaida, nes numeris turi bûti tiksliai 8 simboliø ilgio.
   - Jei paduodami studento numeris "1234ABCD", tai gaunama klaida, nes numeris turi bûti sudarytas tik ið skaièiø.
   - Jei paduodami studento numeris "12345678", kuris jau egzistuoja duomenø bazëje, tai gaunama klaida dël numerio unikalumo paþeidimo.
   - Jei paduodami studento numeris "ABC" (3 simboliai), tai gaunama 2 klaidos, (1)numeris turi bûti tiksliai 8 simboliø ilgio,  (2)numeris turi bûti sudarytas tik ið skaièiø.
   
   - Jei paduodami studento el. paðtas "john.smithexample.com" (trûksta @), tai gaunama klaida, nes el. paðtas turi bûti teisingo formato.
   - Jei paduodami studento el. paðtas "john.smith@" (trûksta domeno), tai gaunama klaida dël netinkamo formato.
   - Jei paduodami studento el. paðtas "@example.com" (trûksta vietovardþio), tai gaunama klaida dël netinkamo formato.
   - Jei paduodami studento el. paðtas "john.smith@example" (trûksta domeno pabaigos), tai gaunama klaida dël netinkamo formato.
   - Jei paduodami studento el. paðtas "john.smith@example." (trûksta domeno pabaigos), tai gaunama klaida dël netinkamo formato.
   - Jei nepaduodamas studento el. paðtas, tai gaunama klaida, nes el. paðtas yra privalomas.
   
   - Jei nepaduodamas studento Departamentas, tai gaunama klaida, nes Departamentas yra privalomas.
   - Jei paduodami studento vardas tuðèias """ arba null, tai gaunama klaida, nes vardas yra privalomas laukas.
   - Jei paduodami du studentai su tuo paèiu el. paðto adresu "alice.johnson@example.com", tai gaunama klaida dël el. paðto unikalumo paþeidimo.
   - Jei paduodami studentas su numeriu "12345679" priskiriamas departamentui su kodu "ENG999", kuris neegzistuoja, tai gaunama klaida dël neegzistuojanèio departamento.
   - Jei paduodami studentas ið departamento "CS1234" priskiriamas paskaitai "Calculus", kuri nepriklauso jo departamentui, tai gaunama klaida.
   - Jei paduodami studentas su numeriu "12345678" perkeliamas á departamentà su kodu "MTH567", tai gaunama sëkmingas perkëlimas, o jo paskaitos atnaujinamos pagal naujà departamentà (jei toks funkcionalumas numatytas).
   - Jei paduodami studentas su numeriu "12345678" perkeliamas á neegzistuojantá departamentà "ENG999", tai gaunama klaida dël neegzistuojanèio departamento.

*/