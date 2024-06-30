﻿using AutoMapper;
using LMS.Application.DTOs;
using LMS.Application.Helpers;
using LMS.Application.Interfaces;
using LMS.Data.Consts;
using LMS.Data.Entities;
using LMS.Data.IGenericRepository_IUOW;

namespace LMS.Application.Services
{
    public class CourseService(IUnitOfWork unitOfWork, IMapper mapper , IUserHelpers userHelpers) : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper=mapper;
        private readonly IUserHelpers _userHelpers=userHelpers;

        public async Task<bool> CreateCourse(CourseDTO courseDto)
        {
            var teacher = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("user not found");
            var course = _mapper.Map<Course>(courseDto);
            course.TeacherId = teacher.Id;
            await _unitOfWork.Courses.AddAsync(course);
            return await _unitOfWork.SaveAsync() > 0;
        }
        public async Task<bool> UpdateCourse(string id, CourseDTO courseDTO)
        {
            _ = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("user not found");
            var course = await _unitOfWork.Courses.FindFirstAsync(c => c.Id == id) ?? throw new Exception("course not found");
            _mapper.Map(courseDTO, course);
            await _unitOfWork.Courses.UpdateAsync(course);
            return await _unitOfWork.SaveAsync() > 0;
        }
        public async Task<bool> DeleteCourse(string id)
        {
            _ = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("user not found");
            var course=await _unitOfWork.Courses.FindFirstAsync(c=>c.Id==id)??throw new Exception("course not found");
            await _unitOfWork.Courses.RemoveAsync(course);
            return await _unitOfWork.SaveAsync() > 0;
        }

        public async Task<bool> EnrollingStudentInCourse(string StudentEmail, string courseCode)
        {
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("user not found");
            var student = await _unitOfWork.Students.FindFirstAsync(s => s.Email == StudentEmail)?? throw new Exception("student not found");
            var course = await _unitOfWork.Courses.FindFirstAsync(c => c.Code == courseCode) ?? throw new Exception("course not found");
            if (currentUser.Id != course.TeacherId)
                throw new Exception("course not found");
            var newStudentCourse =new StudentCourse { CourseId =course.Id,StudentId=student.Id};
            await _unitOfWork.StudentCourses.AddAsync(newStudentCourse);
            return await _unitOfWork.SaveAsync() > 0;

        }

        public async Task<List<CourseResultDTO>> GetAllCourses()
        {
            var courses = await _unitOfWork.Courses.GetAllAsync(orderBy: course => course.Name,
            direction: OrderDirection.Ascending,
            includes:
            [
                course => course.Teacher
            ]);
            var coursesResult = _mapper.Map<IEnumerable<CourseResultDTO>>(courses).ToList();
            return coursesResult;
        }

        public async Task<CourseResultDTO> GetCourse(string id)
        {
            var courses = await _unitOfWork.Courses.FindFirstAsync(c=>c.Id==id,
            includes:
            [
                course => course.Teacher
            ]);
            var coursesResult = _mapper.Map<CourseResultDTO>(courses);
            return coursesResult;
        }

        public async Task<List<CourseResultDTO>> GetCoursesByTeacherId(string id)
        {
            var courses = await _unitOfWork.Courses.FindAsync(c=>c.TeacherId==id,
            orderBy: course => course.Name,
            direction: OrderDirection.Ascending,
            includes:
            [
                c=> c.Teacher
            ]);
            var coursesResult = _mapper.Map<IEnumerable<CourseResultDTO>>(courses).ToList();
            return coursesResult;
        }

        public async Task<int> GetNumberOfCourses()
        {
           return await _unitOfWork.Courses.CountAsync();
        }

        public async Task<int> GetStudentCountInCourse(string courseId)
        {
            var studentCourses = await _unitOfWork.StudentCourses.FindAsync(sc => sc.CourseId == courseId);
            return studentCourses.Count();
        }

        public async Task<List<CourseResultDTO>> SearchForCources(string crateria)
        {
            var courses = await _unitOfWork.Courses.FindAsync(c => c.MaterialName.Contains(crateria)||c.Name.Contains(crateria)||c.Semester.Contains(crateria)||c.Teacher.FirstName.Contains(crateria)||c.Teacher.LastName.Contains(crateria)
            ||crateria.Contains(c.MaterialName) || crateria.Contains(c.Name) || crateria.Contains(c.Semester) || crateria.Contains(c.Teacher.FirstName) || crateria.Contains(c.Teacher.LastName),
            orderBy: course => course.Name,
            direction: OrderDirection.Ascending,
            includes:
            [
                c => c.Teacher
            ]);
            var coursesResult = _mapper.Map<IEnumerable<CourseResultDTO>>(courses).ToList();
            return coursesResult;
        }
    }
}