using Microsoft.EntityFrameworkCore;

namespace backend.Models;

public class QuizContext : DbContext {
    public QuizContext(DbContextOptions<QuizContext> options) : base(options) { }
    public DbSet<Question> Questions { get; set; } = null!;
    public DbSet<ExamResult> Results { get; set; } = null!;
}