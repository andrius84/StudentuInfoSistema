using Microsoft.EntityFrameworkCore;
using StudentuInformacineSistema.Database.Entities;
using StudentuInformacineSistema.Database.Repositories;
using StudentuInformacineSistema.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentuInformacineSistema.Tests
{
    [TestClass]
    public class UnitTestsDepartamentsValidations
    {
        private StudentsContext _context;
        private DepartmentService _departmentService;

        [TestInitialize]
        public void TestInitialize()
        {
            // Use a fresh in-memory database for each test
            var options = new DbContextOptionsBuilder<StudentsContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Ensure unique database per test
                .Options;

            _context = new StudentsContext(options);
            _context.Database.EnsureCreated(); // Optional but good to ensure

            // Initialize repository and service
            var departmentRepository = new DepartmentRepository(_context);
            _departmentService = new DepartmentService(departmentRepository);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Ensure database is deleted and context is disposed after each test
            _context.Database.EnsureDeleted(); // Deletes the in-memory database
            _context.Dispose();
        }

    [TestMethod]
        public void CreateDepartment_ShortName_ShouldReturnFalse()
        {
            var department = new Department
            {
                DepartmentCode = "CS1234",
                DepartmentName = "CS" // per trumpas pavadinimas
            };

            var result = _departmentService.CreateDepartament(department);

            Assert.IsFalse(result, "Departamento vardas turi būti iš ne mažiau kaip 3 simboliai");
        }

        [TestMethod]
        public void CreateDepartment_InvalidCharactersInName_ShouldReturnFalse()
        {
            var department = new Department
            {
                DepartmentCode = "CS1234",
                DepartmentName = "Computer Science & Engineering" // blogas simbolis
            };

            var result = _departmentService.CreateDepartament(department);

            Assert.IsFalse(result, "Departamento vardą turi sudaryti tik skaičiai ir raidės");
        }

        [TestMethod]
        public void CreateDepartment_ShortCode_ShouldReturnFalse()
        {
            var department = new Department
            {
                DepartmentCode = "CS12", // per trumpas kodas
                DepartmentName = "Computer Science"
            };

            var result = _departmentService.CreateDepartament(department);

            Assert.IsFalse(result, "Departamento kodas turi būti tiksliai 6 simboliai");
        }

        [TestMethod]
        public void CreateDepartment_InvalidCharactersInCode_ShouldReturnFalse()
        {
            var department = new Department
            {
                DepartmentCode = "CS123@", // negalimas simbolis
                DepartmentName = "Computer Science"
            };

            var result = _departmentService.CreateDepartament(department);

            Assert.IsFalse(result, "Departamento vardą turi sudaryti tik skaičiai ir raidės");
        }

        [TestMethod]
        public void CreateDepartment_DuplicateCode_ShouldReturnFalse()
        {
            _context.Departments.Add(new Department
            {
                DepartmentCode = "CS1234",
                DepartmentName = "Computer Science"
            });

            var duplicateDepartment = new Department
            {
                DepartmentCode = "CS1234", // dubliuojantis kodas
                DepartmentName = "Physics"
            };

            var result = _departmentService.CreateDepartament(duplicateDepartment);

            Assert.IsFalse(result, "Departamento kodas turi būti unikalus");
        }
    }
}

/*
 
   -Jei paduodamas departamento pavadinimas "CS" (2 simboliai), tai gaunama klaida, nes pavadinimas turi būti ne trumpesnis kaip 3 simboliai.
   - Jei paduodami departamento pavadinimas "Computer Science & Engineering" (su specialiaisiais simboliais), tai gaunama klaida, nes pavadinime gali būti tik raidės ir skaičiai.
   - Jei paduodami departamento kodas "CS12" (4 simboliai), tai gaunama klaida, nes kodas turi būti tiksliai 6 simbolių ilgio.
   - Jei paduodami departamento kodas "CS123@", tai gaunama klaida, nes kode gali būti tik raidės ir skaičiai.
   - Jei paduodami departamento kodas "CS1234" kuris jau egzistuoja duomenų bazėje, tai gaunama klaida dėl kodo unikalumo pažeidimo.

*/