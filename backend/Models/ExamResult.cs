namespace backend.Models;

public class ExamResult {
    public long Id { get; set; }
    public string? FullName { get; set; }
    public int? Score { get; set; }
    public string? Answers { get; set; }
}