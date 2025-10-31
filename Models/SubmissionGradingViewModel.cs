namespace OnlineClassManagement.Models
{
    // ViewModel này chứa MỌI THỨ cần thiết cho trang chấm điểm
    public class SubmissionGradingViewModel
    {
        public Assignment AssignmentDetails { get; set; }
        public List<StudentSubmissionInfo> StudentSubmissions { get; set; }
    }

    // ViewModel này đại diện cho 1 sinh viên và bài nộp của họ
    public class StudentSubmissionInfo
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }

        // Thông tin bài nộp (có thể là NULL nếu chưa nộp)
        public Submission? Submission { get; set; } 
    }
}