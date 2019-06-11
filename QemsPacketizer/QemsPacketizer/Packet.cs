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
            // Split into quarters

            int[] positions = new int[20];

            List<Question> availableTossups = new List<Question>();
            List<Question> availableBonuses = new List<Question>();

            availableTossups.AddRange(this.Tossups);
            availableBonuses.AddRange(this.Bonuses);

            List<Question> firstQuarterTossups = new List<Question>();
            List<Question> firstQuarterBonuses = new List<Question>();

            List<Question> secondQuarterTossups = new List<Question>();
            List<Question> secondQuarterBonuses = new List<Question>();

            List<Question> thirdQuarterTossups = new List<Question>();
            List<Question> thirdQuarterBonuses = new List<Question>();

            List<Question> fourthQuarterTossups = new List<Question>();
            List<Question> fourthQuarterBonuses = new List<Question>();

            List<Question> firstHalfTossups = new List<Question>();
            List<Question> firstHalfBonuses = new List<Question>();
            List<Question> secondHalfTossups = new List<Question>();
            List<Question> secondHalfBonuses = new List<Question>();

            // Get 1/1 art and 1/1 RMP to guarantee into each half
            firstHalfTossups.Add(GetQuestionInParentCategory(availableTossups, Category.Arts, random));
            firstHalfTossups.Add(GetQuestionInParentCategory(availableTossups, Category.RMP, random));
            secondHalfTossups.Add(GetQuestionInParentCategory(availableTossups, Category.Arts, random));
            secondHalfTossups.Add(GetQuestionInParentCategory(availableTossups, Category.RMP, random));

            firstHalfBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.Arts, random));
            firstHalfBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.RMP, random));
            secondHalfBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.Arts, random));
            secondHalfBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.RMP, random));

            // Get any non-major question for the rest
            firstHalfTossups.Add(GetNonLitHistSciQuestion(availableTossups, random));
            firstHalfTossups.Add(GetNonLitHistSciQuestion(availableTossups, random));
            firstHalfBonuses.Add(GetNonLitHistSciQuestion(availableBonuses, random));
            firstHalfBonuses.Add(GetNonLitHistSciQuestion(availableBonuses, random));

            secondHalfTossups.Add(GetNonLitHistSciQuestion(availableTossups, random));
            secondHalfTossups.Add(GetNonLitHistSciQuestion(availableTossups, random));
            secondHalfBonuses.Add(GetNonLitHistSciQuestion(availableBonuses, random));
            secondHalfBonuses.Add(GetNonLitHistSciQuestion(availableBonuses, random));

            // Populate quarters with 3 major categories and 2 minor
            firstQuarterTossups.Add(GetQuestionInParentCategory(availableTossups, Category.Literature, random));
            firstQuarterTossups.Add(GetQuestionInParentCategory(availableTossups, Category.History, random));
            firstQuarterTossups.Add(GetQuestionInParentCategory(availableTossups, Category.Science, random));
            firstQuarterTossups.Add(GetQuestionFromList(firstHalfTossups, random));
            firstQuarterTossups.Add(GetQuestionFromList(firstHalfTossups, random));

            firstQuarterBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.Literature, random));
            firstQuarterBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.History, random));
            firstQuarterBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.Science, random));
            firstQuarterBonuses.Add(GetQuestionFromList(firstHalfBonuses, random));
            firstQuarterBonuses.Add(GetQuestionFromList(firstHalfBonuses, random));

            firstQuarterBonuses.Shuffle();

            secondQuarterTossups.Add(GetQuestionInParentCategory(availableTossups, Category.Literature, random));
            secondQuarterTossups.Add(GetQuestionInParentCategory(availableTossups, Category.History, random));
            secondQuarterTossups.Add(GetQuestionInParentCategory(availableTossups, Category.Science, random));
            secondQuarterTossups.Add(GetQuestionFromList(firstHalfTossups, random));
            secondQuarterTossups.Add(GetQuestionFromList(firstHalfTossups, random));

            secondQuarterBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.Literature, random));
            secondQuarterBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.History, random));
            secondQuarterBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.Science, random));
            secondQuarterBonuses.Add(GetQuestionFromList(firstHalfBonuses, random));
            secondQuarterBonuses.Add(GetQuestionFromList(firstHalfBonuses, random));

            secondQuarterTossups.Shuffle();
            while (secondQuarterTossups[0].Category.ParentCategoryName == firstQuarterTossups[firstQuarterTossups.Count -1].Category.ParentCategoryName)
            {
                secondQuarterTossups.Shuffle();
            }

            secondQuarterBonuses.Shuffle();
            while (secondQuarterBonuses[0].Category.ParentCategoryName == firstQuarterBonuses[firstQuarterBonuses.Count - 1].Category.ParentCategoryName)
            {
                secondQuarterBonuses.Shuffle();
            }

            thirdQuarterTossups.Add(GetQuestionInParentCategory(availableTossups, Category.Literature, random));
            thirdQuarterTossups.Add(GetQuestionInParentCategory(availableTossups, Category.History, random));
            thirdQuarterTossups.Add(GetQuestionInParentCategory(availableTossups, Category.Science, random));
            thirdQuarterTossups.Add(GetQuestionFromList(secondHalfTossups, random));
            thirdQuarterTossups.Add(GetQuestionFromList(secondHalfTossups, random));

            thirdQuarterBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.Literature, random));
            thirdQuarterBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.History, random));
            thirdQuarterBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.Science, random));
            thirdQuarterBonuses.Add(GetQuestionFromList(secondHalfBonuses, random));
            thirdQuarterBonuses.Add(GetQuestionFromList(secondHalfBonuses, random));

            thirdQuarterTossups.Shuffle();
            while (thirdQuarterTossups[0].Category == secondQuarterTossups[secondQuarterTossups.Count - 1].Category)
            {
                thirdQuarterTossups.Shuffle();
            }

            thirdQuarterBonuses.Shuffle();
            while (thirdQuarterBonuses[0].Category.ParentCategoryName == secondQuarterBonuses[secondQuarterBonuses.Count - 1].Category.ParentCategoryName)
            {
                thirdQuarterBonuses.Shuffle();
            }

            fourthQuarterTossups.Add(GetQuestionInParentCategory(availableTossups, Category.Literature, random));
            fourthQuarterTossups.Add(GetQuestionInParentCategory(availableTossups, Category.History, random));
            fourthQuarterTossups.Add(GetQuestionInParentCategory(availableTossups, Category.Science, random));
            fourthQuarterTossups.Add(GetQuestionFromList(secondHalfTossups, random));
            fourthQuarterTossups.Add(GetQuestionFromList(secondHalfTossups, random));

            fourthQuarterBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.Literature, random));
            fourthQuarterBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.History, random));
            fourthQuarterBonuses.Add(GetQuestionInParentCategory(availableBonuses, Category.Science, random));
            fourthQuarterBonuses.Add(GetQuestionFromList(secondHalfBonuses, random));
            fourthQuarterBonuses.Add(GetQuestionFromList(secondHalfBonuses, random));

            fourthQuarterTossups.Shuffle();
            while (fourthQuarterTossups[0].Category == thirdQuarterTossups[thirdQuarterTossups.Count - 1].Category)
            {
                fourthQuarterTossups.Shuffle();
            }

            fourthQuarterBonuses.Shuffle();
            while (fourthQuarterBonuses[0].Category.ParentCategoryName == thirdQuarterBonuses[thirdQuarterBonuses.Count - 1].Category.ParentCategoryName)
            {
                fourthQuarterBonuses.Shuffle();
            }

            // Tiebreakers
            this.TiebreakerTossups.Shuffle();
            this.TiebreakerBonuses.Shuffle();

            this.Tossups = new List<Question>();
            this.Tossups.AddRange(firstQuarterTossups);
            this.Tossups.AddRange(secondQuarterTossups);
            this.Tossups.AddRange(thirdQuarterTossups);
            this.Tossups.AddRange(fourthQuarterTossups);

            this.Bonuses = new List<Question>();
            this.Bonuses.AddRange(firstQuarterBonuses);
            this.Bonuses.AddRange(secondQuarterBonuses);
            this.Bonuses.AddRange(thirdQuarterBonuses);
            this.Bonuses.AddRange(fourthQuarterBonuses);
        }

        public Question GetQuestionFromList(List<Question> questions, Random random)
        {
            Question foundQuestion = questions[random.Next(questions.Count)];
            questions.Remove(foundQuestion);
            return foundQuestion;
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
                else if (string.Equals(category, question.Category.GrandParentCategoryName))
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

        public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(
          this IEnumerable<TSource> source, int size)
        {
            TSource[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                    bucket = new TSource[size];

                bucket[count++] = item;
                if (count != size)
                    continue;

                yield return bucket;

                bucket = null;
                count = 0;
            }

            if (bucket != null && count > 0)
                yield return bucket.Take(count);
        }
    }
}
