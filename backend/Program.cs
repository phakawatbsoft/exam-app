using Microsoft.EntityFrameworkCore;
using backend.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<QuizContext>(opt =>
    opt.UseInMemoryDatabase("QuizList"));

builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});    

var app = builder.Build();

// --- สร้างข้อมูลจำลอง (Mock Data) ฝั่ง Backend ---
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<QuizContext>();
    dbContext.Database.EnsureCreated();

    // ถ้ายังไม่มีคำถามในระบบ ให้เพิ่มเข้าไป 3 ข้อ
    if (!dbContext.Questions.Any())
    {
        dbContext.Questions.AddRange(
            new Question { QuestionText = "คำถามที่ 1", Options = "คำตอบที่ 1,คำตอบที่ 2,คำตอบที่ 3,คำตอบที่ 4", CorrectAnswer = "คำตอบที่ 1" },
            new Question { QuestionText = "คำถามที่ 2", Options = "คำตอบที่ 1,คำตอบที่ 2,คำตอบที่ 3,คำตอบที่ 4", CorrectAnswer = "คำตอบที่ 2" },
            new Question { QuestionText = "คำถามที่ 3", Options = "คำตอบที่ 1,คำตอบที่ 2,คำตอบที่ 3,คำตอบที่ 4", CorrectAnswer = "คำตอบที่ 3" }
        );
        dbContext.SaveChanges();
    }
}
// ---------------------------------------------

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
