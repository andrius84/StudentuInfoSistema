using Microsoft.EntityFrameworkCore;
using StudentuInformacineSistema.Database.Entities;
using StudentuInformacineSistema.Database.Repositories;
using StudentuInformacineSistema.Services;
using StudentuInformacineSistema.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentuInformacineSistema.Tests
{
    [TestClass]
    public class UnitTestsLectureValidations
    {
        private StudentsContext _context;
        private LectureService _lectureService;

        [TestInitialize]
        public void TestInitialize()
        {
            var options = new DbContextOptionsBuilder<StudentsContext>()
                .UseInMemoryDatabase(databaseName: "TestLectureDatabase")
                .Options;

            _context = new StudentsContext(options);
            _context.Database.EnsureCreated();

            var lectureRepository = new LectureRepository(_context);
            _lectureService = new LectureService(lectureRepository);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public void CreateLecture_ValidLectureInNewDepartment_ShouldReturnTrue()
        {
            //var department = new Department
            //{
            //    DepartmentCode = "MTH567",
            //    DepartmentName = "Mathematics"
            //};
            //_context.Departments.Add(department);
            //_context.SaveChanges();

            var lecture = new Lecture
            {
                LectureName = "DataStructures",
                LectureTime = "10:00-11:30"
            };

            var result = _lectureService.CreateLecture(lecture);

            Assert.IsTrue(result, "Lecture should be successfully added to the department.");
        }

        [TestMethod]
        public void CreateLecture_ExistingLectureInDepartment_ShouldReturnFalse()
        {
            var department = new Department
            {
                DepartmentCode = "CS1234",
                DepartmentName = "Computer Science"
            };
            var lecture = new Lecture
            {
                LectureName = "DataStructures",
                LectureTime = "10:00-11:30"
            };
            _context.Departments.Add(department);
            _context.Lectures.Add(lecture);
            _context.SaveChanges();

            var result = _lectureService.CreateLecture(lecture);

            Assert.IsFalse(result, "Lecture with the same name should not be added again to the same department.");
        }

        [TestMethod]
        public void CreateLecture_EmptyName_ShouldReturnFalse()
        {
            var department = new Department
            {
                DepartmentCode = "MTH567",
                DepartmentName = "Mathematics"
            };
            _context.Departments.Add(department);
            _context.SaveChanges();

            var lecture = new Lecture
            {
                LectureName = "", // Empty name
                LectureTime = "10:00-11:30"
            };

            var result = _lectureService.CreateLecture(lecture);

            Assert.IsFalse(result, "Lecture name is required.");
        }

        [TestMethod]
        public void CreateLecture_ShortName_ShouldReturnFalse()
        {
            var department = new Department
            {
                DepartmentCode = "MTH567",
                DepartmentName = "Mathematics"
            };
            _context.Departments.Add(department);
            _context.SaveChanges();

            var lecture = new Lecture
            {
                LectureName = "Math", // Short name
                LectureTime = "10:00-11:30"
            };

            var result = _lectureService.CreateLecture(lecture);

            Assert.IsFalse(result, "Lecture name must be at least 5 characters long.");
        }

        [TestMethod]
        public void CreateLecture_InvalidTimeFormat_ShouldReturnFalse()
        {
            var department = new Department
            {
                DepartmentCode = "MTH567",
                DepartmentName = "Mathematics"
            };
            _context.Departments.Add(department);
            _context.SaveChanges();

            var lecture = new Lecture
            {
                LectureName = "DataStructures",
                LectureTime = "25:00-26:30" // Invalid time
            };

            var result = _lectureService.CreateLecture(lecture);

            Assert.IsFalse(result, "Lecture time must be between 00:00 and 24:00.");
        }

        [TestMethod]
        public void CreateLecture_EndTimeBeforeStartTime_ShouldReturnFalse()
        {
            var department = new Department
            {
                DepartmentCode = "MTH567",
                DepartmentName = "Mathematics"
            };
            _context.Departments.Add(department);
            _context.SaveChanges();

            var lecture = new Lecture
            {
                LectureName = "DataStructures",
                LectureTime = "14:00-13:00" // End time before start time
            };

            var result = _lectureService.CreateLecture(lecture);

            Assert.IsFalse(result, "End time cannot be earlier than start time.");
        }

        [TestMethod]
        public void CreateLecture_OverlappingLectures_ShouldReturnFalse()
        {
            var department = new Department
            {
                DepartmentCode = "CS1234",
                DepartmentName = "Computer Science"
            };
            _context.Departments.Add(department);
            _context.Lectures.Add(new Lecture { LectureName = "Algorithms", LectureTime = "10:00-11:30" });
            _context.SaveChanges();

            var lecture = new Lecture
            {
                LectureName = "DataStructures",
                LectureTime = "11:00-12:30" // Overlaps with the existing lecture
            };

            var result = _lectureService.CreateLecture(lecture);

            Assert.IsFalse(result, "Lectures cannot overlap in the same department.");
        }

        [TestMethod]
        public void CreateLecture_InvalidWeekday_ShouldReturnFalse()
        {
            //var department = new Department
            //{
            //    DepartmentCode = "MTH567",
            //    DepartmentName = "Mathematics"
            //};
            //_context.Departments.Add(department);
            //_context.SaveChanges();

            var lecture = new Lecture
            {
                LectureName = "DataStructures",
                LectureTime = "10:00-11:30",
                //Weekday = "Sunday" 
            };

            var result = _lectureService.CreateLecture(lecture);

            Assert.IsFalse(result, "Weekday can only be Monday to Friday.");
        }

        [TestMethod]
        public void CreateLecture_NullWeekday_ShouldConsiderDailyLectures()
        {
            //var department = new Department
            //{
            //    DepartmentCode = "MTH567",
            //    DepartmentName = "Mathematics"
            //};
            //_context.Departments.Add(department);
            //_context.SaveChanges();

            var lecture = new Lecture
            {
                LectureName = "DataStructures",
                LectureTime = "10:00-11:30",
                
            };

            var result = _lectureService.CreateLecture(lecture);

            Assert.IsTrue(result, "Lecture should be considered for all weekdays from Monday to Friday.");
        }

        [TestMethod]
        public void AssignStudent_ToLectureNotInDepartment_ShouldReturnFalse()
        {
            var department = new Department
            {
                DepartmentCode = "CS1234",
                DepartmentName = "Computer Science"
            };
            _context.Departments.Add(department);
            _context.SaveChanges();

            var student = new Student
            {
                StudentNumber = 12345678,
                FirstName = "Alice",
                LastName = "Johnson",
                Email = "alice.johnson@example.com"
            };
            _context.Students.Add(student);
            _context.SaveChanges();

            var result = _lectureService.AddLectureToDepartment("Calculus", "MTH567");

            Assert.IsFalse(result, "Student cannot be assigned to a lecture not in their department.");
        }

    }
}


/*
   
   - Jei sukuriama paskaita su pavadinimu "DataStructures" ir priskiriama departamentui su kodu "MTH567", tai gaunama sėkmingas pridėjimas.
   - Jei sukuriama paskaita su pavadinimu "DataStructures" ir priskiriama departamentui su kodu "CS1234", tai gaunama klaida dėl to kad tokia paskaita departamente jau yra.
   - Jei paduodami paskaitos pavadinimas tuščias """ arba null, tai gaunama klaida, nes pavadinimas yra privalomas laukas.
   - Jei paduodami paskaitos pavadinimas "Math" (4 simboliai), tai gaunama klaida, nes pavadinimas turi būti ne trumpesnis kaip 5 simboliai.
   - Jei paduodami paskaitos laikas "25:00-26:30", tai gaunama klaida, nes pradžios ir pabaigos laikas turi būti tarp 00:00 ir 24:00.
   - Jei paduodami paskaitos laikas "14:00-13:00", tai gaunama klaida, nes pabaigos laikas negali būti ankstesnis už pradžios laiką.
   - Jei paduodami dvi paskaitos tame pačiame departamente su laikais "10:00-11:30" ir "11:00-12:30", tai gaunama įspėjimas arba klaida dėl laiko persidengimo.
   - Jei paduodama paskaitos savaitės diena "Sunday", tai gaunama klaida, nes savaitės diena gali būti tik Monday, Tuesday, Wednesday, Thursday, Friday.
   - Jei paskaitos savaitės diena yra null, tuomet paskaitos skaitomos kasdien nuo Monday iki Friday.

   */