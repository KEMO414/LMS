﻿namespace LMS.Data.Entities
{
    public class Question
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CorrectAnswer { get; set; }
        public string ExamId { get; set; }
        public Exam Exam { get; set; }
    }
}
