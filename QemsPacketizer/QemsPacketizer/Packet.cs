using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QemsPacketizer
{
    public class Packet
    {
        public List<Question> Tossups { get; set; }

        public List<Question> Bonuses { get; set; }

        public List<Question> TiebreakerTossups { get; set; }

        public List<Question> TiebreakerBonuses { get; set; }

        public int Round { get; set; }

        /// <summary>
        /// If set, try to use this packet name from QEMS2 to select questions
        /// </summary>
        public string QemsPacketName { get; set; }

        // Maybe a category hierarchy

        public Packet(int round)
        {
            this.Tossups = new List<Question>();
            this.Bonuses = new List<Question>();
            this.TiebreakerTossups = new List<Question>();
            this.TiebreakerBonuses = new List<Question>();
            this.Round = round;
            this.QemsPacketName = "Round " + round;
            if (this.Round < 10)
            {
                this.QemsPacketName = "Round 0" + round;
            }
        }

        public bool IsQuestionLegal(Question question)
        {
            // 

            return false;
        }

        public bool IsTiebreakerLegal(Question question)
        {
            return false;
        }

        public override string ToString()
        {
            return "Packet " + this.Round;
        }

        /// <summary>
        /// Returns a number that represents how good of a packet this is.
        /// Lower numbers are better.  This is affected by things such as the same
        /// categories being in one packet.
        /// </summary>
        /// <returns></returns>
        public int GetPacketScore()
        {
            return 0;
        }

        public void Randomize(Random random)
        {
            // Split into first 10 and last 10

            int[] positions = new int[20];

            List<Question> availableTossups = new List<Question>();
            List<Question> availableBonuses = new List<Question>();

            availableTossups.AddRange(this.Tossups);
            availableBonuses.AddRange(this.Bonuses);

            List<Question> firstHalfTossups = new List<Question>();
            List<Question> firstHalfBonuses = new List<Question>();

            List<Question> secondHalfTossups = new List<Question>();
            List<Question> secondHalfBonuses = new List<Question>();

            firstHalfTossups.Add(GetQuestionInParentCategory(availableTossups, Category.Literature, random));
            firstHalfTossups.Add(GetQuestionInParentCategory(availableTossups, Category.Literature, random));
            firstHalfTossups.Add(GetQuestionInParentCategory(availableTossups, Category.History, random));
            firstHalfTossups.Add(GetQuestionInParentCategory(availableTossups, Category.History, random));
            firstHalfTossups.Add(GetQuestionInParentCategory(availableTossups, Category.Science, random));
            firstHalfTossups.Add(GetQuestionInParentCategory(availableTossups, Category.Science, random));

            // We want 1 RMP and 1 Arts tossup guaranteed.  Get them at once for first half / second half to not run out
            firstHalfTossups.Add(GetQuestionInParentCategory(availableTossups, Category.Arts, random));
            firstHalfTossups.Add(GetQuestionInParentCategory(availableTossups, Category.RMP, random));
            secondHalfTossups.Add(GetQuestionInParentCategory(availableTossups, Category.Arts, random));
            secondHalfTossups.Add(GetQuestionInParentCategory(availableTossups, Category.RMP, random));

            // The other 2 can be from any category
            firstHalfTossups.Add(GetNonLitHistSciQuestion(availableTossups, random));
            firstHalfTossups.Add(GetNonLitHistSciQuestion(availableTossups, random));

            firstHalfBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.Literature, random));
            firstHalfBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.Literature, random));
            firstHalfBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.History, random));
            firstHalfBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.History, random));
            firstHalfBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.Science, random));
            firstHalfBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.Science, random));

            // Get the arts and RMP at once to not run out
            firstHalfBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.Arts, random));
            firstHalfBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.RMP, random));
            secondHalfBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.Arts, random));
            secondHalfBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.RMP, random));

            firstHalfBonuses.Add(GetNonLitHistSciQuestion(availableBonuses, random));
            firstHalfBonuses.Add(GetNonLitHistSciQuestion(availableBonuses, random));

            firstHalfTossups.Shuffle();
            firstHalfBonuses.Shuffle();

            secondHalfTossups.Add(GetQuestionInParentCategory(availableTossups, Category.Literature, random));
            secondHalfTossups.Add(GetQuestionInParentCategory(availableTossups, Category.Literature, random));
            secondHalfTossups.Add(GetQuestionInParentCategory(availableTossups, Category.History, random));
            secondHalfTossups.Add(GetQuestionInParentCategory(availableTossups, Category.History, random));
            secondHalfTossups.Add(GetQuestionInParentCategory(availableTossups, Category.Science, random));
            secondHalfTossups.Add(GetQuestionInParentCategory(availableTossups, Category.Science, random));
            secondHalfTossups.Add(GetNonLitHistSciQuestion(availableTossups, random));
            secondHalfTossups.Add(GetNonLitHistSciQuestion(availableTossups, random));

            secondHalfBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.Literature, random));
            secondHalfBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.Literature, random));
            secondHalfBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.History, random));
            secondHalfBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.History, random));
            secondHalfBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.Science, random));
            secondHalfBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.Science, random));
            secondHalfBonuses.Add(GetNonLitHistSciQuestion(availableBonuses, random));
            secondHalfBonuses.Add(GetNonLitHistSciQuestion(availableBonuses, random));

            secondHalfTossups.Shuffle();
            secondHalfBonuses.Shuffle();

            // If tossup 10 is same category as tossup 11, shuffle to avoid clumps
            while (secondHalfTossups[0].Category.ParentCategoryName == firstHalfTossups[9].Category.ParentCategoryName)
            {
                secondHalfTossups.Shuffle();
            }

            while (secondHalfBonuses[0].Category.ParentCategoryName == firstHalfBonuses[9].Category.ParentCategoryName)
            {
                secondHalfBonuses.Shuffle();
            }

            // Tiebreakers
            this.TiebreakerTossups.Shuffle();
            this.TiebreakerBonuses.Shuffle();

            this.Tossups = new List<Question>();
            this.Tossups.AddRange(firstHalfTossups);
            this.Tossups.AddRange(secondHalfTossups);

            this.Bonuses = new List<Question>();
            this.Bonuses.AddRange(firstHalfBonuses);
            this.Bonuses.AddRange(secondHalfBonuses);
        }

        public Question GetQuestionInParentCategory(List<Question> questions, string category, Random random)
        {
            List<Question> possibleQuestions = new List<Question>();
            foreach (Question question in questions)
            {
                if (string.Equals(category, question.Category.ParentCategoryName))
                {
                    possibleQuestions.Add(question);
                }
            }

            if (possibleQuestions.Count == 0)
            {
                throw new Exception("Couldn't find category:" + category);
            }

            Question foundQuestion = possibleQuestions[random.Next(possibleQuestions.Count)];
            questions.Remove(foundQuestion);
            return foundQuestion;
        }

        public Question GetNonLitHistSciQuestion(List<Question> questions, Random random)
        {
            HashSet<string> categories = new HashSet<string>();
            categories.Add(Category.Literature);
            categories.Add(Category.History);
            categories.Add(Category.Science);

            List<Question> possibleQuestions = new List<Question>();
            foreach (Question question in questions)
            {
                if (string.IsNullOrEmpty(question.Category.ParentCategoryName) || !categories.Contains(question.Category.ParentCategoryName))
                {
                    possibleQuestions.Add(question);
                }
            }

            if (possibleQuestions.Count == 0)
            {
                throw new Exception("Couldn't find non-sci/hist/lit");
            }

            Question foundQuestion = possibleQuestions[random.Next(possibleQuestions.Count)];
            questions.Remove(foundQuestion);
            return foundQuestion;

        }
    }

    public static class Shuffler
    {
        public static Random random = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
