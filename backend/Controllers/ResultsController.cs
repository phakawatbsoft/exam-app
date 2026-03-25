using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using backend.Models;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly QuizContext _context;

        public ResultsController(QuizContext context)
        {
            _context = context;
        }

        // GET: api/Results
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamResult>>> GetResults()
        {
            return await _context.Results.ToListAsync();
        }

        // GET: api/Results/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamResult>> GetExamResult(long id)
        {
            var examResult = await _context.Results.FindAsync(id);

            if (examResult == null)
            {
                return NotFound();
            }

            return examResult;
        }

        // PUT: api/Results/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExamResult(long id, ExamResult examResult)
        {
            if (id != examResult.Id)
            {
                return BadRequest();
            }

            _context.Entry(examResult).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExamResultExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Results
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ExamResult>> PostExamResult(ExamResult result)
        {
            if (result.Answers == null)
            {
                return BadRequest("Answers cannot be null.");
            }

            // 1. Fetch all questions to get correct answers
            var questions = await _context.Questions.ToListAsync();
            int score = 0;
            Dictionary<string, string>? answersDict = JsonSerializer.Deserialize<Dictionary<string, string>>(result.Answers);

            // 2. Calculate score
            foreach (var question in questions)
            {
                if (answersDict.TryGetValue(question.Id.ToString(), out var userAnswer) &&
                    !string.IsNullOrEmpty(userAnswer) &&
                    userAnswer == question.CorrectAnswer)
                {
                    score++;
                }
            }

            // 3. Create the entity to save
            var examResult = new ExamResult
            {
                FullName = result.FullName,
                Score = score,
                Answers = result.Answers
            };

            _context.Results.Add(examResult);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExamResult), new { id = examResult.Id }, examResult);
        }

        // DELETE: api/Results/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExamResult(long id)
        {
            var examResult = await _context.Results.FindAsync(id);
            if (examResult == null)
            {
                return NotFound();
            }

            _context.Results.Remove(examResult);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExamResultExists(long id)
        {
            return _context.Results.Any(e => e.Id == id);
        }
    }
}
