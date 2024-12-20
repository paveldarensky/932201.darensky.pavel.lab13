using Microsoft.AspNetCore.Mvc;
using LW_13_WebApplication.Models;
using System.Collections.Generic;
using System.Linq;

namespace LW_13_WebApplication.Controllers
{
    public class QuizController : Controller
    {
        public static List<QuizQuestion> _questions = new List<QuizQuestion>
        {
            new QuizQuestion { Question = "1 - 6 =", CorrectAnswer = "-5" },
            new QuizQuestion { Question = "8 + 6 =", CorrectAnswer = "14" },
            new QuizQuestion { Question = "5 - 7 =", CorrectAnswer = "-2" },
            new QuizQuestion { Question = "5 - 2 =", CorrectAnswer = "3" }
        };

        private static QuizState _quizState = new QuizState();

        [HttpGet]
        public IActionResult IndexQuiz()
        {
            _quizState.Reset();
            return View(_quizState);
        }

        [HttpPost]
        public IActionResult IndexQuiz(string answer, string action)
        {
            if (action == "Next")
            {
                if (!int.TryParse(answer, out _))
                {
                    _quizState.ErrorMessage = "Введите числовое значение.";
                    return View(_quizState);
                }

                _quizState.UserAnswers.Add(answer);

                if (_quizState.CurrentQuestionIndex < _questions.Count - 1)
                {
                    _quizState.CurrentQuestionIndex++;
                    _quizState.ErrorMessage = string.Empty;
                    return View(_quizState);
                }

                return RedirectToAction("IndexQuizResult");
            }
            else if (action == "Finish")
            {
                if (!int.TryParse(answer, out _))
                {
                    _quizState.ErrorMessage = "Введите числовое значение.";
                    return View(_quizState);
                }

                _quizState.UserAnswers.Add(answer);
                return RedirectToAction("IndexQuizResult");
            }

            return View(_quizState);
        }

        [HttpGet]
        public IActionResult IndexQuizResult()
        {
            while (_quizState.UserAnswers.Count < _questions.Count)
            {
                _quizState.UserAnswers.Add("");
            }

            var results = new QuizResults
            {
                TotalQuestions = _questions.Count,
                CorrectAnswers = _questions
                    .Select((q, index) => new { q, index })
                    .Count(item => _quizState.UserAnswers[item.index] == item.q.CorrectAnswer),
                QuestionsAndAnswers = _questions
                    .Select((q, index) => new QuestionAnswerResult
                    {
                        Question = q.Question,
                        CorrectAnswer = q.CorrectAnswer,
                        UserAnswer = _quizState.UserAnswers[index] ?? ""
                    }).ToList()
            };

            return View(results);
        }
    }

    public class QuizState
    {
        public int CurrentQuestionIndex { get; set; } = 0;
        public List<string> UserAnswers { get; set; } = new List<string>();
        public string ErrorMessage { get; set; }

        public QuizQuestion CurrentQuestion => QuizController._questions[CurrentQuestionIndex];

        public void Reset()
        {
            CurrentQuestionIndex = 0;
            UserAnswers.Clear();
            ErrorMessage = string.Empty;
        }
    }


    public class QuizQuestion
    {
        public string Question { get; set; }
        public string CorrectAnswer { get; set; }
    }

    public class QuizResults
    {
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public List<QuestionAnswerResult> QuestionsAndAnswers { get; set; }
    }

    public class QuestionAnswerResult
    {
        public string Question { get; set; }
        public string CorrectAnswer { get; set; }
        public string UserAnswer { get; set; }
    }
}
