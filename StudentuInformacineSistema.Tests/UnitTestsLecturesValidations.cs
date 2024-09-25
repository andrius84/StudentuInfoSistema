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
    public class UnitTestsLecturesValidations
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
        public void AddLectureToDepartment_ValidLectureInNewDepartment_ShouldReturnTrue()
        {
            var departmentCode = "MTH567";
            var lectureName = "DataStructures";

            var result = _lectureService.AddLectureToDepartment(lectureName, departmentCode); 

            Assert.IsTrue(result, "Lecture should be successfully added to the department.");
        }

        [TestMethod]
        public void CreateLecture_ExistingLectureInDepartment_ShouldReturnFalse()
        {
            var lecture = new Lecture
            {
                LectureName = "DataStructures",
                LectureTime = "10:00-11:30"
            };

            var result = _lectureService.CreateLecture(lecture);

            Assert.IsFalse(result, "Lecture with the same name should not be added again to the same department.");
        }

        [TestMethod]
        public void CreateLecture_EmptyName_ShouldReturnFalse()
        {
            var lecture = new Lecture
            {
                LectureName = "", // Tuščias pavadinimas
                LectureTime = "10:00-11:30"
            };

            var result = _lectureService.CreateLecture(lecture);

            Assert.IsFalse(result, "Paskaitos vardas privalomas");
        }

        [TestMethod]
        public void CreateLecture_ShortName_ShouldReturnFalse()
        {
            var lecture = new Lecture
            {
                LectureName = "Math", // Per trumpas vardas
                LectureTime = "10:00-11:30"
            };

            var result = _lectureService.CreateLecture(lecture);

            Assert.IsFalse(result, "Paskaitos vardas turi būti mažiausiai 5 simboliai");
        }

        [TestMethod]
        public void CreateLecture_InvalidTimeFormat_ShouldReturnFalse()
        {
            var lecture = new Lecture
            {
                LectureName = "DataStructures",
                LectureTime = "25:00-26:30" // neteisingas laiko formatas
            };

            var result = _lectureService.CreateLecture(lecture);

            Assert.IsFalse(result, "Paskaitos laikas turi būti tarp 00:00 ir 24:00.");
        }

        [TestMethod]
        public void CreateLecture_EndTimeBeforeStartTime_ShouldReturnFalse()
        {
            var lecture = new Lecture
            {
                LectureName = "DataStructures",
                LectureTime = "14:00-13:00" // Pabaigos laikas negali būti ankstesnis už pradžios laiką
            };

            var result = _lectureService.CreateLecture(lecture);

            Assert.IsFalse(result, "Pabaigos laikas negali būti ankstesnis už pradžios laiką");
        }

        [TestMethod]
        public void CreateLecture_OverlappingLectures_ShouldReturnFalse()
        {
            var lecture = new Lecture
            {
                LectureName = "DataStructures",
                LectureTime = "11:00-12:30" // Persidengiantis laikas
            };

            var result = _lectureService.CreateLecture(lecture);

            Assert.IsFalse(result, "Paskaitos laikas negali persidengti tame pačiame departamente");
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