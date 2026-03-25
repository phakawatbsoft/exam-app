namespace backend.Models;

public class Question
{
    public long Id { get; set; }
    public string? QuestionText { get; set; }
    public string? Options { get; set; }
    public string? CorrectAnswer { get; set; }
}