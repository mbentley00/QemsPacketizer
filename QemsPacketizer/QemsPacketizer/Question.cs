using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QemsPacketizer
{
    public class Question
    {
        public enum QuestionType
        {
            Tossup,
            Bonus
        }

        public QuestionType QType { get; set; }

        public Category Category { get; set; }

        public int Index { get; set; }

        /// <summary>
        /// The id of this question in QEMS2
        /// </summary>
        public string QemsQuestionId { get; set; }

        public int QuestionNumber { get; set; }

        public string TossupText { get; set; }

        public string TossupAnswer { get; set; }

        public string LeadinText { get; set; }

        public string Part1Text { get; set; }

        public string Part1Answer { get; set; }

        public string Part2Text { get; set; }

        public string Part2Answer { get; set; }

        public string Part3Text { get; set; }

        public string Part3Answer { get; set; }

        public string Author { get; set; }

        public List<string> Comments { get; set; }

        public bool IsFinalsQuestion { get; set; }

        public bool IsAllStarGameQuestion { get; set; }

        public bool IsPlayoffsQuestion { get; set; }

        public bool IsExtraQuestion { get; set; }

        public int Length
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.TossupText))
                {
                    return GetLengthWithoutPronunciationGuides(this.TossupText);
                }
                else
                {
                    return GetLengthWithoutPronunciationGuides(this.LeadinText)
                        + GetLengthWithoutPronunciationGuides(this.Part1Text)
                        + GetLengthWithoutPronunciationGuides(this.Part2Text)
                        + GetLengthWithoutPronunciationGuides(this.Part3Text);
                }
            }
        }

        public static int GetLengthWithoutPronunciationGuides(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return 0;
            }

            // Match anything in between parens and get rid of it
            Regex regex = new Regex(@"\(.+\)");
            return regex.Replace(text, "").Length;
        }

        /// <summary>
        /// The packet that was set in QEMS
        /// </summary>
        public string QemsPacketTag { get; set; }

        public int? QemsPacketTagInt
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.QemsPacketTag))
                {
                    return null;
                }
                else
                {
                    if (QemsPacketTag.StartsWith("Round "))
                    {
                        string justRound = QemsPacketTag.Substring(6, 2);
                        return Int32.Parse(justRound);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public Question(Category category, QuestionType qType, int index)
        {
            this.Category = category;
            this.QType = qType;
            this.Index = index;
            this.QemsQuestionId = "";
            this.Comments = new List<string>();
        }

        public void FormatComments(string rawComments)
        {
            this.Comments = new List<string>();
            if (!string.IsNullOrEmpty(rawComments))
            {
                string[] comments = rawComments.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string comment in comments)
                {
                    if (!string.IsNullOrWhiteSpace(comment) && comment.Contains(":"))
                    {
                        this.Comments.Add("@@" + comment);
                    }
                }

                string allComments = string.Join(" ", this.Comments).ToLowerInvariant();
                if (allComments.Contains("finals"))
                {
                    this.IsFinalsQuestion = true;
                }

                if (allComments.Contains("asg") || allComments.Contains("all star") || allComments.Contains("all-star"))
                {
                    this.IsAllStarGameQuestion = true;
                }

                if (allComments.Contains("extra") || allComments.Contains("tiebreaker"))
                {
                    this.IsExtraQuestion = true;
                }

                if (allComments.Contains("playoff") | allComments.Contains("sunday"))
                {
                    this.IsPlayoffsQuestion = true;
                }
            }
        }

        public override string ToString()
        {
            return ToString(true);
        }

        public string ToString(bool escapeQuotes)
        {
            if (escapeQuotes)
            {
                if (!string.IsNullOrEmpty(this.TossupText))
                {
                    return string.Format("\"{0}||{1}||{2}||{3}||{4}||{5}\"", 
                        this.TossupText.Replace("\"", "\"\""),
                        this.TossupAnswer.Replace("\"", "\"\""),
                        this.Author.Replace("\"", "\"\""),
                        string.Join("~~", this.Comments).Replace("\"", "\"\""),
                        this.Category.ToString(true).Replace("\"", "\"\""),
                        this.QemsQuestionId.Replace("\"", "\"\""));
                }
                else if (!string.IsNullOrEmpty(this.Part1Text))
                {
                    return string.Format("\"{0}||{1}||{2}||{3}||{4}||{5}||{6}||{7}||{8}||{9}||{10}\"",
                        this.LeadinText.Replace("\"", "\"\""),
                        this.Part1Text.Replace("\"", "\"\""),
                        this.Part1Answer.Replace("\"", "\"\""),
                        this.Part2Text.Replace("\"", "\"\""),
                        this.Part2Answer.Replace("\"", "\"\""),
                        this.Part3Text.Replace("\"", "\"\""),
                        this.Part3Answer.Replace("\"", "\"\""),
                        this.Author.Replace("\"", "\"\""),
                        string.Join("~~", this.Comments).Replace("\"", "\"\""),
                        this.Category.ToString(true).Replace("\"", "\"\""),
                        this.QemsQuestionId.Replace("\"", "\"\"")
                        );
                }
                else
                {
                    return string.Format("\"{0} - {1} - {2}\"", this.Category, this.QType, this.Index);
                }
            }
            else {
                if (!string.IsNullOrEmpty(this.TossupText))
                {
                    return string.Format("{0}||{1}||{2}||{3}||{4}", this.TossupText, this.TossupAnswer,
                        this.Author, string.Join("~~", this.Comments), this.Category);
                }
                else if (!string.IsNullOrEmpty(this.Part1Text))
                {
                    return string.Format("{0}||{1}||{2}||{3}||{4}||{5}||{6}||{7}||{8}||{9}",
                        this.LeadinText,
                        this.Part1Text,
                        this.Part1Answer,
                        this.Part2Text,
                        this.Part2Answer,
                        this.Part3Text,
                        this.Part3Answer,
                        this.Author,
                        string.Join("~~", this.Comments),
                        this.Category);
                }
                else
                {
                    return string.Format("{0} - {1} - {2}", this.Category, this.QType, this.Index);
                }
            }
        }

        public bool IsRealQuestion()
        {
            if (!string.IsNullOrEmpty(this.TossupText) || !string.IsNullOrEmpty(this.Part1Text))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
