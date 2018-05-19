namespace QemsPacketizer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using CsvHelper;

    public class QuestionSet
    {
        /// <summary>
        /// When looking at questions that were manually assigned a packet, how far to look back
        /// </summary>
        public const int MaxPacketDistance = 4;

        public List<Packet> Packets { get; set; }

        /// <summary>
        /// All tossups not yet in a packet
        /// </summary>
        public List<Question> UnassignedTossups { get; set; }

        /// <summary>
        /// All bonuses not yet in a packet
        /// </summary>
        public List<Question> UnassignedBonuses { get; set; }

        /// <summary>
        /// Tossups that we couldn't put into a packet because there were too many in the set
        /// </summary>
        public List<Question> ExtraTossups { get; set; }

        /// <summary>
        /// Bonuses that we couldn't put into a packet because there were too many in the set
        /// </summary>
        public List<Question> ExtraBonuses { get; set; }

        /// <summary>
        /// Map from category name to category object of all categories in the set
        /// </summary>
        public Dictionary<string, Category> Categories { get; set; }

        /// <summary>
        /// Random seed to use
        /// </summary>
        private Random random = new Random();

        /// <summary>
        /// Different types of question sets
        /// </summary>
        public enum SetType
        {
            NSC,
            Nasat,
            VHSL,
            Generic
        }

        public void CreateNASATSet(int numberOfPackets)
        {
            this.Packets = new List<Packet>();
            for (int i = 1; i <= numberOfPackets; i++)
            {
                this.Packets.Add(new Packet(i));
            }

            // We're going to need to know the minimum number of questions in each packet
            List<Category> catList = new List<Category>();

            // Lit - 4
            catList.Add(new Category(NasatCategory.Literature, 4, 4, 4, 4, 8, 8));
            catList.Add(new Category(NasatCategory.AmericanLiterature, 1, 2, 1, 2, 2, 3, NasatCategory.Literature));
            catList.Add(new Category(NasatCategory.BritishLiterature, 1, 2, 1, 2, 2, 3, NasatCategory.Literature));
            catList.Add(new Category(NasatCategory.EuropeanLiterature, 1, 1, 1, 1, 2, 2, NasatCategory.Literature));
            catList.Add(new Category(NasatCategory.WorldLiterature, 0, 1, 0, 1, 1, 2, NasatCategory.Literature));

            // Hist - 4
            catList.Add(new Category(NasatCategory.History, 4, 4, 4, 4, 8, 8));
            catList.Add(new Category(NasatCategory.AmericanHistory, 1, 1, 1, 1, 2, 2, NasatCategory.History));
            catList.Add(new Category(NasatCategory.EuropeanHistory, 2, 2, 2, 2, 4, 4, NasatCategory.History));
            catList.Add(new Category(NasatCategory.WorldHistory, 1, 1, 1, 1, 2, 2, NasatCategory.History));

            // Sci - 4
            catList.Add(new Category(NasatCategory.Science, 4, 4, 4, 4, 8, 8));
            catList.Add(new Category(NasatCategory.Biology, 1, 2, 1, 2, 2, 4, NasatCategory.Science));
            catList.Add(new Category(NasatCategory.Physics, 1, 1, 1, 1, 2, 2, NasatCategory.Science));
            catList.Add(new Category(NasatCategory.Chemistry, 0, 1, 0, 1, 1, 2, NasatCategory.Science));
            catList.Add(new Category(NasatCategory.Math, 0, 1, 0, 1, 1, 1, NasatCategory.Science));
            catList.Add(new Category(NasatCategory.ComputerScience, 0, 1, 0, 1, 0, 1, NasatCategory.Science)); // 8/8
            catList.Add(new Category(NasatCategory.EarthScience, 0, 1, 0, 1, 0, 1, NasatCategory.Science)); // 7/6
            catList.Add(new Category(NasatCategory.Astronomy, 0, 1, 0, 1, 0, 1, NasatCategory.Science)); // 7/6
            catList.Add(new Category(NasatCategory.HistoryOfScience, 0, 1, 0, 1, 0, 1, NasatCategory.Science));

            // Arts - 3
            catList.Add(new Category(NasatCategory.Arts, 3, 3, 3, 3, 6, 6));
            catList.Add(new Category(NasatCategory.Painting, 1, 1, 1, 1, 2, 2, NasatCategory.Arts));
            catList.Add(new Category(NasatCategory.Music, 1, 1, 1, 1, 2, 2, NasatCategory.Arts));
            catList.Add(new Category(NasatCategory.Photography, 0, 1, 0, 1, 0, 1, NasatCategory.Arts));
            catList.Add(new Category(NasatCategory.Sculpture, 0, 1, 0, 1, 0, 1, NasatCategory.Arts));
            catList.Add(new Category(NasatCategory.Opera, 0, 1, 0, 1, 0, 1, NasatCategory.Arts));
            catList.Add(new Category(NasatCategory.Ballet, 0, 1, 0, 1, 0, 1, NasatCategory.Arts));
            catList.Add(new Category(NasatCategory.Jazz, 0, 1, 0, 1, 0, 1, NasatCategory.Arts));
            catList.Add(new Category(NasatCategory.Film, 0, 1, 0, 1, 0, 1, NasatCategory.Arts));
            catList.Add(new Category(NasatCategory.Architecture, 0, 1, 0, 1, 0, 1, NasatCategory.Arts));
            catList.Add(new Category(NasatCategory.MiscArts, 0, 1, 0, 1, 0, 1, NasatCategory.Arts));

            // Religion/Myth - 2
            catList.Add(new Category(NasatCategory.RMP, 3, 3, 3, 3, 6, 6)); // TODO: This may not be right
            catList.Add(new Category(NasatCategory.GrecoRomanMyth, 0, 1, 0, 1, 0, 2, NasatCategory.Myth, NasatCategory.RMP));
            catList.Add(new Category(NasatCategory.OtherMyth, 0, 1, 0, 1, 0, 2, NasatCategory.Myth, NasatCategory.RMP));
            catList.Add(new Category(NasatCategory.Myth, 0, 1, 0, 1, 0, 2, NasatCategory.RMP));
            catList.Add(new Category(NasatCategory.Christianity, 0, 1, 0, 1, 0, 2, NasatCategory.Religion, NasatCategory.RMP));
            catList.Add(new Category(NasatCategory.OtherReligion, 0, 1, 0, 1, 0, 2, NasatCategory.Religion, NasatCategory.RMP));
            catList.Add(new Category(NasatCategory.Religion, 0, 1, 0, 1, 0, 2, NasatCategory.RMP));

            // Phil/SS/GK - 2
            catList.Add(new Category(NasatCategory.Philosophy, 0, 1, 0, 1, 1, 2, NasatCategory.RMP));

            catList.Add(new Category(NasatCategory.SocialScience, 0, 1, 0, 1, 1, 2, NasatCategory.SocialScience));
            catList.Add(new Category(NasatCategory.Psychology, 0, 1, 0, 1, 0, 1, NasatCategory.SocialScience));
            catList.Add(new Category(NasatCategory.Economics, 0, 1, 0, 1, 0, 1, NasatCategory.SocialScience));
            catList.Add(new Category(NasatCategory.Anthropology, 0, 1, 0, 1, 0, 1, NasatCategory.SocialScience));
            catList.Add(new Category(NasatCategory.Sociology, 0, 1, 0, 1, 0, 1, NasatCategory.SocialScience));
            catList.Add(new Category(NasatCategory.Linguistics, 0, 1, 0, 1, 0, 1, NasatCategory.SocialScience));
            catList.Add(new Category(NasatCategory.OtherSS, 0, 1, 0, 1, 0, 1, NasatCategory.SocialScience));

            // geo/ce - 1
            catList.Add(new Category(NasatCategory.Geography, 0, 1, 0, 1, 0, 1));
            catList.Add(new Category(NasatCategory.USGeography, 0, 1, 0, 1, 0, 1, NasatCategory.Geography));
            catList.Add(new Category(NasatCategory.EuropeanGeography, 0, 1, 0, 1, 0, 1, NasatCategory.Geography));
            catList.Add(new Category(NasatCategory.WorldGeography, 0, 1, 0, 1, 0, 1, NasatCategory.Geography));

            catList.Add(new Category(NasatCategory.CurrentEvents, 0, 1, 0, 1, 0, 1));
            catList.Add(new Category(NasatCategory.USCurrentEvents, 0, 1, 0, 1, 0, 1, NasatCategory.CurrentEvents));
            catList.Add(new Category(NasatCategory.ForeignCurrentEvents, 0, 1, 0, 1, 0, 1, NasatCategory.CurrentEvents));

            this.Categories = new Dictionary<string, Category>();
            foreach (Category category in catList)
            {
                this.Categories.Add(category.Name, category);
            }

            this.UnassignedTossups = new List<Question>();
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.AmericanLiterature, 29, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.BritishLiterature, 29, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.EuropeanLiterature, 20, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.WorldLiterature, 8, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.AmericanHistory, 30, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.EuropeanHistory, 45, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.WorldHistory, 14, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.Biology, 21, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.Chemistry, 13, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.Physics, 20, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.Math, 19, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.ComputerScience, 2, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.EarthScience, 4, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.Astronomy, 4, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.HistoryOfScience, 2, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.Christianity, 12, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.OtherReligion, 11, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.GrecoRomanMyth, 12, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.OtherMyth, 12, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.Philosophy, 13, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.Painting, 20, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.Music, 20, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.Photography, 1, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.Sculpture, 2, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.Opera, 4, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.Ballet, 2, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.Jazz, 2, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.MiscArts, 1, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.Film, 5, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.Architecture, 3, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.Psychology, 6, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.Economics, 6, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.Anthropology, 2, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.Sociology, 2, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.Linguistics, 2, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.OtherSS, 2, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.USGeography, 3, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.EuropeanGeography, 2, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.WorldGeography, 8, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.USCurrentEvents, 4, Question.QuestionType.Tossup));
            this.UnassignedTossups.AddRange(this.GetQuestions(NasatCategory.ForeignCurrentEvents, 3, Question.QuestionType.Tossup));

            Console.WriteLine(this.UnassignedTossups.Count);
            Console.WriteLine();

            this.UnassignedBonuses = new List<Question>();
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.AmericanLiterature, 29, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.BritishLiterature, 29, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.EuropeanLiterature, 20, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.WorldLiterature, 8, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.AmericanHistory, 30, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.EuropeanHistory, 45, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.WorldHistory, 14, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Biology, 21, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Chemistry, 13, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Physics, 20, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Math, 19, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.ComputerScience, 2, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.EarthScience, 4, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Astronomy, 4, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.HistoryOfScience, 2, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Christianity, 12, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.OtherReligion, 11, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.GrecoRomanMyth, 12, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.OtherMyth, 12, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Philosophy, 13, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Painting, 20, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Music, 20, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Photography, 1, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Sculpture, 2, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Opera, 4, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Ballet, 2, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Jazz, 2, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.MiscArts, 1, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Film, 5, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Architecture, 3, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Psychology, 6, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Economics, 6, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Anthropology, 2, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Sociology, 2, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Linguistics, 2, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.OtherSS, 2, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.USGeography, 3, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.EuropeanGeography, 2, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.WorldGeography, 8, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.USCurrentEvents, 4, Question.QuestionType.Bonus));
            this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.ForeignCurrentEvents, 3, Question.QuestionType.Bonus));



            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.AmericanLiterature, 28, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.BritishLiterature, 29, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.EuropeanLiterature, 20, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.WorldLiterature, 7, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.AmericanHistory, 31, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.EuropeanHistory, 42, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.WorldHistory, 13, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Biology, 22, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Chemistry, 11, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Physics, 21, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Math, 11, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.ComputerScience, 3, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.EarthScience, 7, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Astronomy, 7, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.HistoryOfScience, 3, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Christianity, 12, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.OtherReligion, 11, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.GrecoRomanMyth, 12, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.OtherMyth, 10, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Philosophy, 13, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Painting, 21, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Music, 21, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Photography, 2, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Sculpture, 2, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Opera, 5, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Ballet, 2, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Jazz, 2, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.MiscArts, 1, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Film, 4, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Architecture, 4, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Psychology, 6, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Economics, 6, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Anthropology, 2, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Sociology, 2, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.Linguistics, 2, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.OtherSS, 2, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.USGeography, 4, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.EuropeanGeography, 2, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.WorldGeography, 15, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.USCurrentEvents, 5, Question.QuestionType.Bonus));
            //this.UnassignedBonuses.AddRange(this.GetQuestions(NasatCategory.ForeignCurrentEvents, 5, Question.QuestionType.Bonus));
        }

        public void LoadTotalQuestionCounts(string file)
        {
            this.UnassignedTossups = new List<Question>();
            this.UnassignedBonuses = new List<Question>();
            this.Categories = new Dictionary<string, Category>();

            bool readingCategories = false;
            foreach (string line in File.ReadAllLines(file))
            {
                if (!readingCategories)
                {
                    if (line.StartsWith(@"""Category"",""Subcategory""") || (line.Contains("Category") && line.Contains("Subcategory")))
                    {
                        readingCategories = true;
                    }
                }
                else
                {
                    List<string> columns = ReadCSVLine(line);
                    if (columns.Count >= 4)
                    {
                        string category = columns[0];
                        string subCategory = columns[1];
                        string formattedName = columns[0] + " - " + columns[1];
                        int tossupCount = Int32.Parse(columns[2]);
                        int bonusCount = Int32.Parse(columns[3]);

                        if (!this.Categories.ContainsKey(category))
                        {
                            // Add the base category
                            // TODO: At some point set the actual values for these numbers--although I'm not positive we use them
                            Category c = new Category(category, 0, 10, 0, 10, 0, 20);
                            this.Categories.Add(category, c);
                        }

                        // TODO: At some point set the actual values for these numbers--although I'm not positive we use them
                        Category sc = new Category(formattedName, 0, 10, 0, 10, 0, 20, category);
                        this.Categories.Add(formattedName, sc);
                        this.UnassignedTossups.AddRange(this.GetQuestions(formattedName, tossupCount, Question.QuestionType.Tossup));
                        this.UnassignedBonuses.AddRange(this.GetQuestions(formattedName, bonusCount, Question.QuestionType.Bonus));
                    }
                }
            }
        }

        public void CreateGenericSet(string file, int numberOfPackets)
        {
            this.Packets = new List<Packet>();
            for (int i = 1; i <= numberOfPackets; i++)
            {
                this.Packets.Add(new Packet(i));
            }

            LoadTotalQuestionCounts(file);
        }

        public QuestionSet(SetType setType, string file, int numberOfPackets)
        {
            if (setType == SetType.NSC || setType == SetType.VHSL || setType == SetType.Generic)
            {
                this.CreateGenericSet(file, numberOfPackets);
            }
            else if (setType == SetType.Nasat)
            {
                this.CreateNASATSet(numberOfPackets);
            }
        }

        public void CreatePackets(SetType setType)
        {
            if (setType == SetType.NSC)
            {
                this.CreateNSCPackets();
            }
            else if (setType == SetType.Nasat)
            {
                this.CreateNasatPackets();
            }
            else if (setType == SetType.VHSL)
            {
                this.CreateVHSLPackets();
            }
        }

        public void CreateVHSLPackets()
        {
            // Regular tossups
            foreach (Packet packet in this.Packets)
            {
                List<Question> firstPeriodTossups = new List<Question>();
                List<Question> bonuses = new List<Question>();
                List<Question> secondPeriodTossups = new List<Question>();
                List<Question> anyPeriodTossups = new List<Question>();

                /*
                 * Literature: 5/3 (3 in one tossup period and 2 in the other)
                History: 6/3 (3 in each tossup period)
                Science: 7/3 (4 in one tossup period and 3 in the other)
                RMP: 3/1 (2 in one tossup period and 1 in the other)
                Arts: 3/1 (2 in one tossup period and 1 in the other)
                Geography: 2/1 (1 in each tossup period) 
                Trash: 2/1  (1 in each tossup period)
                Current Events: 1/1
                Social Science: 1/0
                Math calculation: 0/4
                Foreign language: 0/1
                Literary terms & vocabulary: 0/1
                */

                /* The second period is the "directed round" containing 20 bonuses – see the sample packet for how this works. 
                 * Note that the 20-second calculation questions must always appear as an A and B pair of the same number, 
                 * and the 30-second calculation questions must always appear as an A and B pair of the same number that comes 
                 * at least 2 question numbers after the 20-second calculation pair. Also, regulation needs to have 1 calculation 
                 * question each from the four subjects (algebra, geometry, trigonometry, and statistics/probability) – I set the 
                 * subdistribution so the right number of questions will come out, but the program should have an extra check on 
                 * this so it doesn't put, e.g., a 20-second algebra and a 30-second algebra in the same packet.
                 */

                // Determine if this one gets 3 or 2 lit tossups

                firstPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.Literature }));
                firstPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.Literature }));
                secondPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.Literature }));
                secondPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.Literature }));
                anyPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.Literature }));

                firstPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.History }));
                firstPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.History }));
                firstPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.History }));
                secondPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.History }));
                secondPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.History }));
                secondPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.History }));

                firstPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.Science }));
                firstPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.Science }));
                firstPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.Science }));
                secondPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.Science }));
                secondPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.Science }));
                secondPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.Science }));
                anyPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.Science }));

                firstPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.RMP }));
                secondPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.RMP }));
                anyPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.RMP }));

                firstPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.Arts }));
                secondPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.Arts }));
                anyPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.Arts }));

                firstPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.Geography }));
                secondPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.Geography }));

                firstPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.Trash }));
                secondPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.Trash }));

                anyPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.CurrentEvents }));
                anyPeriodTossups.Add(GetUnassignedTossup(new string[] { VhslCategory.SocialScience }));

                bonuses.Add(GetUnassignedBonus(new string[] { VhslCategory.Literature }));
                bonuses.Add(GetUnassignedBonus(new string[] { VhslCategory.Literature }));
                bonuses.Add(GetUnassignedBonus(new string[] { VhslCategory.Literature }));

                bonuses.Add(GetUnassignedBonus(new string[] { VhslCategory.History }));
                bonuses.Add(GetUnassignedBonus(new string[] { VhslCategory.History }));
                bonuses.Add(GetUnassignedBonus(new string[] { VhslCategory.History }));

                bonuses.Add(GetUnassignedBonus(new string[] { VhslCategory.Science }));
                bonuses.Add(GetUnassignedBonus(new string[] { VhslCategory.Science }));
                bonuses.Add(GetUnassignedBonus(new string[] { VhslCategory.Science }));

                bonuses.Add(GetUnassignedBonus(new string[] { VhslCategory.RMP }));
                bonuses.Add(GetUnassignedBonus(new string[] { VhslCategory.Arts }));
                bonuses.Add(GetUnassignedBonus(new string[] { VhslCategory.Geography }));
                bonuses.Add(GetUnassignedBonus(new string[] { VhslCategory.Trash }));
                bonuses.Add(GetUnassignedBonus(new string[] { VhslCategory.CurrentEvents }));

                bonuses.AddRange(this.GetVhslMathBonuses());

                bonuses.Add(GetUnassignedBonus(new string[] { VhslCategory.ForeignLanguage }));
                bonuses.Add(GetUnassignedBonus(new string[] { VhslCategory.Vocab }));

                anyPeriodTossups.Shuffle();
                for (int i = 0; i < 6; i++)
                {
                    if (i < 3)
                    {
                        firstPeriodTossups.Add(anyPeriodTossups[i]);
                    }
                    else
                    {
                        secondPeriodTossups.Add(anyPeriodTossups[i]);
                    }
                }

                firstPeriodTossups.Shuffle();
                secondPeriodTossups.Shuffle();
                bonuses.Shuffle();
                packet.Tossups = new List<Question>();
                packet.Tossups.AddRange(firstPeriodTossups);
                packet.Tossups.AddRange(secondPeriodTossups);
                packet.Bonuses = bonuses;
            }

            // Tiebreakers
            foreach (Packet packet in this.Packets)
            {
                /* 
                 * Literature: 1/.25
                    History: 1/.25
                    Science:  1/.25
                    RMP: .67/.08
                    Arts: .67/.09
                    Geography:  .66/.08
                    Trash: 0/0
                    Current Events: 0/0
                    Social Science: 0/0
                    Math calculation: 0/1
                    Foreign language: 0/0
                    Literary terms & vocabulary: 0/0
                    */

                List<Question> tossups = new List<Question>();
                List<Question> bonuses = new List<Question>();

                tossups.Add(GetUnassignedTossup(new string[] { VhslCategory.Literature }));
                tossups.Add(GetUnassignedTossup(new string[] { VhslCategory.History }));
                tossups.Add(GetUnassignedTossup(new string[] { VhslCategory.Science }));

                // 2 from RMP, Arts, Geography
                string[] tbCats = new string[] { VhslCategory.RMP, VhslCategory.Arts, VhslCategory.Geography };
                Question tbTossup = this.GetUnassignedTossup(tbCats);
                tossups.Add(tbTossup);
                tossups.Add(this.GetUnassignedTossup(tbCats, new string[] { tbTossup.Category.Name }));

                bonuses.Add(GetUnassignedBonus(new string[] { VhslCategory.MathCalc20, VhslCategory.MathCalc30 }));
                bonuses.Add(GetUnassignedBonus(new string[] { VhslCategory.Literature, VhslCategory.History, VhslCategory.Science, VhslCategory.RMP, VhslCategory.Arts,
                    VhslCategory.Geography }));

                tossups.Shuffle();

                // Math calc should always be first so don't shuffle bonuses

                packet.TiebreakerTossups = tossups;
                packet.TiebreakerBonuses = bonuses;
            }

            this.Packets.Shuffle();
            this.SetVhslMathBonusOrder();

            // Don't randomize questions in packets since we've already done that

            // Temporary measure to make sure that the first two packets have questions
            EnsureFirstPacketsHaveRealQuestions(2);
            StripTimeFromVhslTiebreakerBonus();
        }

        /// <summary>
        /// Get rid of text indicating that VHSL tiebreaker math bonuses should be timed
        /// </summary>
        private void StripTimeFromVhslTiebreakerBonus()
        {
            foreach (Packet packet in this.Packets)
            {
                Question tbBonus = packet.TiebreakerBonuses[0];
                if (!string.IsNullOrWhiteSpace(tbBonus.Part1Text))
                {
                    tbBonus.Part1Text = tbBonus.Part1Text.Replace("This is a 30-second calculation question.", string.Empty).Replace("This is a 20-second calculation question.", string.Empty);
                    tbBonus.Part1Text = tbBonus.Part1Text.Replace("This is a 30 second calculation question.", string.Empty).Replace("This is a 20 second calculation question.", string.Empty).TrimStart();
                }
            }
        }

        /// <summary>
        /// Place VHSL math bonuses in the specified order
        /// </summary>
        private void SetVhslMathBonusOrder()
        {
            foreach (Packet packet in this.Packets)
            {
                // 20 second must always be 3
                // 30 second must always be 8

                int first20Index = -1;
                int second20Index = -1;
                int first30Index = -1;
                int second30Index = -1;

                for (int i = 0; i < packet.Bonuses.Count; i++)
                {
                    Question bonus = packet.Bonuses[i];
                    if (VhslCategory.MathCalc20.Equals(bonus.Category.ParentCategoryName))
                    {
                        if (first20Index == -1)
                        {
                            first20Index = i;
                        }
                        else
                        {
                            second20Index = i;
                        }
                    }
                }

                SwapBonus(packet, first20Index, 4); // Put at 3rd question
                SwapBonus(packet, second20Index, 5); // Also put at 3rd question

                for (int i = 0; i < packet.Bonuses.Count; i++)
                {
                    Question bonus = packet.Bonuses[i];
                    if (VhslCategory.MathCalc30.Equals(bonus.Category.ParentCategoryName))
                    {
                        if (first30Index == -1)
                        {
                            first30Index = i;
                        }
                        else
                        {
                            second30Index = i;
                        }
                    }
                }

                SwapBonus(packet, first30Index, 14); // Put at 8th question
                SwapBonus(packet, second30Index, 15); // Also put at 8th question
            }
        }

        /// <summary>
        /// Switch two bonuses
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private void SwapBonus(Packet packet, int a, int b)
        {
            Question tempQuestion = packet.Bonuses[b];
            packet.Bonuses[b] = packet.Bonuses[a];
            packet.Bonuses[a] = tempQuestion;
        }

        /// <summary>
        /// When making a set, prioritize placing the actual written questions in the first packets
        /// </summary>
        /// <param name="packetCount">Number of packets</param>
        private void EnsureFirstPacketsHaveRealQuestions(int packetCount)
        {
            for (int p = 0; p < packetCount; p++)
            {
                for (int i = 0; i < this.Packets[p].Tossups.Count; i++)
                {
                    if (!this.Packets[p].Tossups[i].IsRealQuestion())
                    {
                        FindNextRealQuestion(p, i, false, Question.QuestionType.Tossup, packetCount);
                    }
                }

                for (int i = 0; i < this.Packets[p].Bonuses.Count; i++)
                {
                    if (!this.Packets[p].Bonuses[i].IsRealQuestion())
                    {
                        FindNextRealQuestion(p, i, false, Question.QuestionType.Bonus, packetCount);
                    }
                }

                for (int i = 0; i < this.Packets[p].TiebreakerTossups.Count; i++)
                {
                    if (!this.Packets[p].TiebreakerTossups[i].IsRealQuestion())
                    {
                        FindNextRealQuestion(p, i, true, Question.QuestionType.Tossup, packetCount);
                    }
                }

                for (int i = 0; i < this.Packets[p].TiebreakerBonuses.Count; i++)
                {
                    if (!this.Packets[p].TiebreakerBonuses[i].IsRealQuestion())
                    {
                        FindNextRealQuestion(p, i, true, Question.QuestionType.Bonus, packetCount);
                    }
                }
            }
        }

        /// <summary>
        /// Looks for a question that has actually been written and isn't a palceholder
        /// </summary>
        /// <param name="originalPacketIndex"></param>
        /// <param name="originalQuestionIndex"></param>
        /// <param name="isTiebreaker"></param>
        /// <param name="questionType"></param>
        /// <param name="firstPacketToUse"></param>
        private void FindNextRealQuestion(int originalPacketIndex, int originalQuestionIndex, bool isTiebreaker, Question.QuestionType questionType, int firstPacketToUse)
        {
            Question originalQuestion = null;
            if (!isTiebreaker)
            {
                if (questionType == Question.QuestionType.Tossup)
                {
                    originalQuestion = this.Packets[originalPacketIndex].Tossups[originalQuestionIndex];
                }
                else
                {
                    originalQuestion = this.Packets[originalPacketIndex].Bonuses[originalQuestionIndex];
                }
            }
            else
            {
                if (questionType == Question.QuestionType.Tossup)
                {
                    originalQuestion = this.Packets[originalPacketIndex].TiebreakerTossups[originalQuestionIndex];
                }
                else
                {
                    originalQuestion = this.Packets[originalPacketIndex].TiebreakerBonuses[originalQuestionIndex];
                }
            }

            for (int p = firstPacketToUse; p < this.Packets.Count; p++)
            {
                List<Question> questionList = new List<Question>();
                if (questionType == Question.QuestionType.Tossup)
                {
                    for (int i = 0; i < this.Packets[p].Tossups.Count; i++)
                    {
                        Question curQuestion = this.Packets[p].Tossups[i];
                        if (curQuestion.Category.Name == originalQuestion.Category.Name && curQuestion.IsRealQuestion())
                        {
                            this.Packets[p].Tossups[i] = originalQuestion;

                            if (!isTiebreaker)
                            {
                                this.Packets[originalPacketIndex].Tossups[originalQuestionIndex] = curQuestion;
                            }
                            else
                            {
                                this.Packets[originalPacketIndex].TiebreakerTossups[originalQuestionIndex] = curQuestion;
                            }

                            return;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < this.Packets[p].Bonuses.Count; i++)
                    {
                        Question curQuestion = this.Packets[p].Bonuses[i];
                        if (curQuestion.Category.Name == originalQuestion.Category.Name && curQuestion.IsRealQuestion())
                        {
                            this.Packets[p].Bonuses[i] = originalQuestion;

                            if (!isTiebreaker)
                            {
                                this.Packets[originalPacketIndex].Bonuses[originalQuestionIndex] = curQuestion;
                            }
                            else
                            {
                                this.Packets[originalPacketIndex].TiebreakerBonuses[originalQuestionIndex] = curQuestion;
                            }

                            return;
                        }
                    }
                }
            }

            Console.WriteLine("Couldn't swap: " + originalQuestion.Category.Name);
        }

        private List<Question> GetVhslMathBonuses()
        {
            List<List<string>> mathSets = new List<List<string>>();
            mathSets.Add(new List<string> { VhslCategory.Algebra20, VhslCategory.Geometry20, VhslCategory.Trigonometry30, VhslCategory.Statistics30 });
            mathSets.Add(new List<string> { VhslCategory.Algebra20, VhslCategory.Trigonometry20, VhslCategory.Geometry30, VhslCategory.Statistics30 });
            mathSets.Add(new List<string> { VhslCategory.Algebra20, VhslCategory.Statistics20, VhslCategory.Geometry30, VhslCategory.Trigonometry30 });
            mathSets.Add(new List<string> { VhslCategory.Trigonometry20, VhslCategory.Geometry20, VhslCategory.Algebra30, VhslCategory.Statistics30 });
            mathSets.Add(new List<string> { VhslCategory.Trigonometry20, VhslCategory.Statistics20, VhslCategory.Algebra30, VhslCategory.Geometry30 });
            mathSets.Add(new List<string> { VhslCategory.Statistics20, VhslCategory.Geometry20, VhslCategory.Algebra30, VhslCategory.Trigonometry30 });

            Dictionary<string, int> categoryCount = new Dictionary<string, int>();
            foreach (Question bonus in this.UnassignedBonuses)
            {
                if (!categoryCount.ContainsKey(bonus.Category.Name))
                {
                    categoryCount.Add(bonus.Category.Name, 1);
                }
                else
                {
                    categoryCount[bonus.Category.Name]++;
                }
            }

            for (int i = mathSets.Count - 1; i >= 0; i--)
            {
                bool isValid = true;
                foreach (string category in mathSets[i])
                {
                    if (!categoryCount.ContainsKey(category) || categoryCount[category] == 0)
                    {
                        isValid = false;
                        break;
                    }
                }

                if (!isValid)
                {
                    mathSets.RemoveAt(i);
                }
            }

            if (mathSets.Count == 0)
            {
                throw new InvalidOperationException("Can't find math sets!");
            }

            int index = this.random.Next(mathSets.Count);
            List<Question> questions = new List<Question>();
            foreach (string category in mathSets[index])
            {
                questions.Add(this.GetUnassignedBonus(category));
            }

            return questions;
        }

        public void CreateNasatPackets()
        {
            // Create regular
            foreach (Packet packet in this.Packets)
            {
                List<Question> tossups = new List<Question>();
                List<Question> bonuses = new List<Question>();

                // Find 1 am lit
                tossups.Add(GetUnassignedTossup(new string[] { NasatCategory.AmericanLiterature }, new string[] { NasatCategory.EuropeanLiterature, NasatCategory.BritishLiterature, NasatCategory.WorldLiterature }, packetToMatch: packet.QemsPacketName));
                bonuses.Add(GetUnassignedBonus(new string[] { NasatCategory.AmericanLiterature }, new string[] { NasatCategory.EuropeanLiterature, NasatCategory.BritishLiterature, NasatCategory.WorldLiterature }, packetToMatch: packet.QemsPacketName));

                // Find 1 brit lit
                tossups.Add(GetUnassignedTossup(new string[] { NasatCategory.BritishLiterature }, new string[] { NasatCategory.EuropeanLiterature, NasatCategory.AmericanLiterature, NasatCategory.WorldLiterature }, packetToMatch: packet.QemsPacketName));
                bonuses.Add(GetUnassignedBonus(new string[] { NasatCategory.BritishLiterature }, new string[] { NasatCategory.EuropeanLiterature, NasatCategory.AmericanLiterature, NasatCategory.WorldLiterature }, packetToMatch: packet.QemsPacketName));

                // Find 1 eur or world lit
                string[] eurWorldLitCats = new string[] { NasatCategory.EuropeanLiterature, NasatCategory.WorldLiterature };
                tossups.Add(GetUnassignedTossup(eurWorldLitCats, new string[] { NasatCategory.AmericanLiterature, NasatCategory.BritishLiterature }, packetToMatch: packet.QemsPacketName));
                bonuses.Add(GetUnassignedBonus(eurWorldLitCats, new string[] { NasatCategory.AmericanLiterature, NasatCategory.BritishLiterature }, packetToMatch: packet.QemsPacketName));

                // Find 1 of any lit
                tossups.Add(GetUnassignedTossup(NasatCategory.Literature, packetToMatch: packet.QemsPacketName));
                bonuses.Add(GetUnassignedBonus(NasatCategory.Literature, packetToMatch: packet.QemsPacketName));

                // Find 1 am hist
                tossups.Add(GetUnassignedTossup(new string[] { NasatCategory.AmericanHistory }, new string[] { NasatCategory.EuropeanHistory, NasatCategory.WorldHistory }, packetToMatch: packet.QemsPacketName));
                bonuses.Add(GetUnassignedBonus(new string[] { NasatCategory.AmericanHistory }, new string[] { NasatCategory.EuropeanHistory, NasatCategory.WorldHistory }, packetToMatch: packet.QemsPacketName));

                // Find 2 eur hist
                tossups.Add(GetUnassignedTossup(NasatCategory.EuropeanHistory, packetToMatch: packet.QemsPacketName));
                bonuses.Add(GetUnassignedBonus(NasatCategory.EuropeanHistory, packetToMatch: packet.QemsPacketName));
                tossups.Add(GetUnassignedTossup(NasatCategory.EuropeanHistory, packetToMatch: packet.QemsPacketName));
                bonuses.Add(GetUnassignedBonus(NasatCategory.EuropeanHistory, packetToMatch: packet.QemsPacketName));

                // Find 1 world hist or am history
                string[] histCats = new string[] { NasatCategory.AmericanHistory, NasatCategory.EuropeanHistory, NasatCategory.WorldHistory };
                List<string> excludeHistCats = new List<string> { NasatCategory.EuropeanHistory };
                Question otherHistTossup = this.GetUnassignedTossup(histCats, excludeHistCats.ToArray(), packetToMatch: packet.QemsPacketName);
                tossups.Add(otherHistTossup);
                excludeHistCats.Add(otherHistTossup.Category.Name);
                bonuses.Add(this.GetUnassignedBonus(histCats, excludeHistCats.ToArray(), packetToMatch: packet.QemsPacketName));

                // Find 1 bio
                tossups.Add(GetUnassignedTossup(NasatCategory.Biology, packetToMatch: packet.QemsPacketName));
                bonuses.Add(GetUnassignedBonus(NasatCategory.Biology, packetToMatch: packet.QemsPacketName));

                // Find 1 physics
                tossups.Add(GetUnassignedTossup(NasatCategory.Physics, packetToMatch: packet.QemsPacketName));
                bonuses.Add(GetUnassignedBonus(NasatCategory.Physics, packetToMatch: packet.QemsPacketName));

                // Find 1 math/chem                
                Question mathChemTossup = this.GetUnassignedTossup(new string[] { NasatCategory.Math, NasatCategory.Chemistry }, packetToMatch: packet.QemsPacketName);
                tossups.Add(mathChemTossup);
                bonuses.Add(this.GetUnassignedBonus(new string[] { NasatCategory.Math, NasatCategory.Chemistry }, new string[] { mathChemTossup.Category.Name }, packetToMatch: packet.QemsPacketName));

                // Find 1 other sci
                string[] otherSci = new string[] { NasatCategory.HistoryOfScience, NasatCategory.ComputerScience, NasatCategory.Astronomy, NasatCategory.EarthScience };
                Question otherSciTossup = this.GetUnassignedTossup(otherSci, packetToMatch: packet.QemsPacketName);
                tossups.Add(otherSciTossup);
                bonuses.Add(this.GetUnassignedBonus(otherSci, new string[] { otherSciTossup.Category.Name }, packetToMatch: packet.QemsPacketName));

                // Pick 1 painting
                tossups.Add(GetUnassignedTossup(new string[] { NasatCategory.Painting }, packetToMatch: packet.QemsPacketName));
                bonuses.Add(GetUnassignedBonus(new string[] { NasatCategory.Painting }, packetToMatch: packet.QemsPacketName));

                // Pick 1 music
                tossups.Add(GetUnassignedTossup(new string[] { NasatCategory.Music }, packetToMatch: packet.QemsPacketName));
                bonuses.Add(GetUnassignedBonus(new string[] { NasatCategory.Music }, packetToMatch: packet.QemsPacketName));

                // Pick 1/1 other arts
                string[] miscArtsCats = new string[] { NasatCategory.MiscArts, NasatCategory.Sculpture, NasatCategory.Architecture, NasatCategory.Photography,
                    NasatCategory.Film, NasatCategory.Opera, NasatCategory.Jazz, NasatCategory.Ballet};
                Question miscArtsTossup = GetUnassignedTossup(miscArtsCats, packetToMatch: packet.QemsPacketName);
                tossups.Add(miscArtsTossup);
                bonuses.Add(GetUnassignedBonus(miscArtsCats, new string[] { miscArtsTossup.Category.ParentCategoryName }, packetToMatch: packet.QemsPacketName));


                // Find a total of 3/3 RMP

                // Always pick 1 myth
                Question mythTossup = GetUnassignedTossup(new string[] { NasatCategory.OtherMyth, NasatCategory.GrecoRomanMyth }, packetToMatch: packet.QemsPacketName);
                tossups.Add(mythTossup);
                bonuses.Add(GetUnassignedBonus(new string[] { NasatCategory.OtherMyth, NasatCategory.GrecoRomanMyth }, new string[] { mythTossup.Category.Name }, packetToMatch: packet.QemsPacketName));

                // Alawys pick 1 religion
                Question religionTossup = GetUnassignedTossup(new string[] { NasatCategory.Christianity, NasatCategory.OtherReligion }, packetToMatch: packet.QemsPacketName);
                tossups.Add(religionTossup);
                bonuses.Add(GetUnassignedBonus(new string[] { NasatCategory.Christianity, NasatCategory.OtherReligion }, new string[] { religionTossup.Category.Name }, packetToMatch: packet.QemsPacketName));

                // Choose from philosophy and the remaining religion
                Question otherRmpTossup = GetUnassignedTossup(new string[] { NasatCategory.Myth, NasatCategory.Religion, NasatCategory.Philosophy });
                tossups.Add(otherRmpTossup);

                //Question otherRmpBonus = GetUnassignedTossup(new string[] { NasatCategory.Myth, NasatCategory.Religion, NasatCategory.Philosophy });
                //bonuses.Add(otherRmpBonus);

                bonuses.Add(GetUnassignedBonus(new string[] { NasatCategory.Myth, NasatCategory.Religion, NasatCategory.Philosophy }, new string[] { otherRmpTossup.Category.ParentCategoryName }, packetToMatch: packet.QemsPacketName));

                // Pick 1/1 Geo or CE
                Question geoOrCeTossup = GetUnassignedTossup(new string[] { NasatCategory.Geography, NasatCategory.CurrentEvents }, packetToMatch: packet.QemsPacketName);
                tossups.Add(geoOrCeTossup);
                bonuses.Add(GetUnassignedBonus(new string[] { NasatCategory.Geography, NasatCategory.CurrentEvents }, new string[] { geoOrCeTossup.Category.ParentCategoryName }, packetToMatch: packet.QemsPacketName));

                // Pick 1/1 SS
                Question ssTossup = GetUnassignedTossup(new string[] { NasatCategory.SocialScience }, packetToMatch: packet.QemsPacketName);
                tossups.Add(ssTossup);
                bonuses.Add(GetUnassignedBonus(new string[] { NasatCategory.SocialScience }, new string[] { ssTossup.Category.Name }, packetToMatch: packet.QemsPacketName));

                packet.Tossups = tossups;
                packet.Bonuses = bonuses;
            }

            // Create tiebreakers
            // We should actually do a second pass of everything else since it's legal to include lit/hist/world
            foreach (Packet packet in this.Packets)
            {
                List<Question> tbTossups = new List<Question>();
                List<Question> tbBonuses = new List<Question>();

                List<string> excludeCategories = new List<string>();
                List<string> persistExcludeCategories = new List<string>();
                for (int i = 0; i < 1; i++)
                {
                    Question tbTossup = this.GetUnassignedTossupNotInTheseCategories(excludeCategories.ToArray());
                    tbTossups.Add(tbTossup);
                    excludeCategories.Add(tbTossup.Category.ParentCategoryName);
                    persistExcludeCategories.Add(tbTossup.Category.Name);
                }

                for (int i = 0; i < 1; i++)
                {
                    Question tbBonus = this.GetUnassignedBonusNotInTheseCategories(persistExcludeCategories.ToArray());
                    tbBonuses.Add(tbBonus);
                    persistExcludeCategories.Add(tbBonus.Category.ParentCategoryName);
                }

                packet.TiebreakerTossups.AddRange(tbTossups);
                packet.TiebreakerBonuses.AddRange(tbBonuses);
            }

            this.Packets.Shuffle();
            foreach (Packet packet in this.Packets)
            {
                packet.Randomize(random);
            }
        }

        public void CreateNSCPackets()
        {
            // These are limits to how many questions we can choose for cases when we cross-pollinate categories
            // The idea is that we don't select so many British lit questions that world lit gets put into TB
            int biochemBioTULimit = 4;
            int biochemBioBoLimit = 4;
            int biochemBioTU = 0;
            int biochemBioBo = 0;

            int worldLitOtherTULimit = 6;
            int worldLitOtherBoLimit = 7;
            int worldLitOtherTU = 0;
            int worldLitOtherBo = 0;

            // Create packets for all regular rounds
            foreach (Packet packet in this.Packets)
            {
                List<Question> tossups = new List<Question>();
                List<Question> bonuses = new List<Question>();

                // Find 1 am lit
                tossups.Add(GetUnassignedTossup(Category.AmericanLiterature, packetToMatch: packet.QemsPacketName));
                bonuses.Add(GetUnassignedBonus(Category.AmericanLiterature, packetToMatch: packet.QemsPacketName));

                // Find 1 brit lit
                tossups.Add(GetUnassignedTossup(Category.BritishLiterature, packetToMatch: packet.QemsPacketName));
                bonuses.Add(GetUnassignedBonus(Category.BritishLiterature, packetToMatch: packet.QemsPacketName));

                // Find 1 eur lit
                tossups.Add(GetUnassignedTossup(Category.EuropeanLiterature, packetToMatch: packet.QemsPacketName));
                bonuses.Add(GetUnassignedBonus(Category.EuropeanLiterature, packetToMatch: packet.QemsPacketName));

                // Find 1 other lit - weighted to world
                // TODO: We really need to create a separate max pool of am/brit lit so we don't over dip
                Category worldLit = new Category(Category.WorldLiterature, 0.75);
                Category americanLit = new Category(Category.AmericanLiterature, 0.12);
                Category britishList = new Category(Category.BritishLiterature, 0.12);
                List<Category> otherLitCats = new List<Category>();
                otherLitCats.Add(worldLit);
                otherLitCats.Add(americanLit);
                otherLitCats.Add(britishList);

                // We've run out of other stuff to select, always add a world lit tossup
                if (worldLitOtherTU >= worldLitOtherTULimit)
                {
                    tossups.Add(GetUnassignedTossup(Category.WorldLiterature, packetToMatch: packet.QemsPacketName));
                    if (worldLitOtherBo >= worldLitOtherBoLimit)
                    {
                        bonuses.Add(GetUnassignedBonus(Category.WorldLiterature, packetToMatch: packet.QemsPacketName));
                    }
                    else
                    {
                        Question otherLitBonus = GetUnassignedBonus(otherLitCats, packetToMatch: packet.QemsPacketName);
                        bonuses.Add(otherLitBonus);

                        if (otherLitBonus.Category.Name != Category.WorldLiterature)
                        {
                            worldLitOtherBo++;
                        }
                    }
                }
                else
                {
                    Question otherLitTossup = GetUnassignedTossup(otherLitCats, packetToMatch: packet.QemsPacketName);
                    tossups.Add(otherLitTossup);
                    if (otherLitTossup.Category.Name != Category.WorldLiterature)
                    {
                        worldLitOtherTU++;
                    }

                    if (otherLitTossup.Category.Name != Category.WorldLiterature || worldLitOtherBo >= worldLitOtherBoLimit)
                    {
                        // Always add a world lit bonus in this case
                        Question otherLitBonus = null;
                        try
                        {
                            otherLitBonus = GetUnassignedBonus(Category.WorldLiterature, packetToMatch: packet.QemsPacketName);
                        }
                        catch (Exception)
                        {
                        }

                        if (otherLitBonus == null)
                        {
                            otherLitBonus = GetUnassignedBonus(otherLitCats, packetToMatch: packet.QemsPacketName);
                            if (otherLitBonus == null)
                            {
                                throw new Exception("Other Lit");
                            }
                        }

                        bonuses.Add(otherLitBonus);
                    }
                    else
                    {
                        // Pick whether to add a world lit bonus or not
                        Question otherLitBonus = GetUnassignedBonus(otherLitCats, packetToMatch: packet.QemsPacketName);
                        bonuses.Add(otherLitBonus);

                        if (otherLitBonus.Category.Name != Category.WorldLiterature)
                        {
                            worldLitOtherBo++;
                        }
                    }
                }

                // Find 1 am hist
                tossups.Add(GetUnassignedTossup(Category.AmericanHistory, packetToMatch: packet.QemsPacketName));
                bonuses.Add(GetUnassignedBonus(Category.AmericanHistory, packetToMatch: packet.QemsPacketName));

                // Find 2 eur hist
                tossups.Add(GetUnassignedTossup(Category.EuropeanHistory, packetToMatch: packet.QemsPacketName));
                bonuses.Add(GetUnassignedBonus(Category.EuropeanHistory, packetToMatch: packet.QemsPacketName));
                tossups.Add(GetUnassignedTossup(Category.EuropeanHistory, packetToMatch: packet.QemsPacketName));
                bonuses.Add(GetUnassignedBonus(Category.EuropeanHistory, packetToMatch: packet.QemsPacketName));

                // Find 1 world hist
                tossups.Add(GetUnassignedTossup(Category.WorldHistory, packetToMatch: packet.QemsPacketName));
                bonuses.Add(GetUnassignedBonus(Category.WorldHistory, packetToMatch: packet.QemsPacketName));

                // Find 1 bio
                tossups.Add(GetUnassignedTossup(Category.Biology, packetToMatch: packet.QemsPacketName));
                bonuses.Add(GetUnassignedBonus(Category.Biology, packetToMatch: packet.QemsPacketName));

                // Find 1 physics
                tossups.Add(GetUnassignedTossup(Category.Physics, packetToMatch: packet.QemsPacketName));
                bonuses.Add(GetUnassignedBonus(Category.Physics, packetToMatch: packet.QemsPacketName));

                // Find 1 bio/chem
                Category biology = new Category(Category.Biology, 0.25);
                Category chemistry = new Category(Category.Chemistry, 0.75);
                List<Category> biochemCats = new List<Category>();
                biochemCats.Add(biology);
                biochemCats.Add(chemistry);
                if (biochemBioTU >= biochemBioTULimit)
                {
                    tossups.Add(GetUnassignedTossup(Category.Chemistry, packetToMatch: packet.QemsPacketName));
                    if (biochemBioBo >= biochemBioBoLimit)
                    {
                        bonuses.Add(GetUnassignedBonus(Category.Chemistry, packetToMatch: packet.QemsPacketName));
                    }
                    else
                    {
                        Question bioChemBonus = null;
                        try
                        {
                            bioChemBonus = GetUnassignedBonus(biochemCats, packetToMatch: packet.QemsPacketName);
                        }
                        catch (Exception)
                        {
                        }

                        if (bioChemBonus == null)
                        {
                            throw new Exception("biochem");
                        }

                        bonuses.Add(bioChemBonus);
                        if (bioChemBonus.Category.Name != Category.Chemistry)
                        {
                            biochemBioBo++;
                        }
                    }
                }
                else
                {
                    Question bioChemTossup = GetUnassignedTossup(biochemCats, packetToMatch: packet.QemsPacketName);
                    tossups.Add(bioChemTossup);
                    if (bioChemTossup.Category.Name != Category.Chemistry)
                    {
                        biochemBioTU++;
                    }

                    if (bioChemTossup.Category.Name != Category.Chemistry || biochemBioBo >= biochemBioBoLimit)
                    {
                        // Always add a chem bonus in this case
                        Question bioChemBonus = null;
                        try
                        {
                            bioChemBonus = GetUnassignedBonus(Category.Chemistry, packetToMatch: packet.QemsPacketName);
                        }
                        catch (Exception)
                        {
                        }

                        if (bioChemBonus == null)
                        {
                            bioChemBonus = GetUnassignedBonus(biochemCats, packetToMatch: packet.QemsPacketName);
                            if (bioChemBonus == null)
                            {
                                throw new Exception("Biochem");
                            }
                        }

                        bonuses.Add(bioChemBonus);
                    }
                    else
                    {
                        Question bioChemBonus = GetUnassignedBonus(biochemCats, packetToMatch: packet.QemsPacketName);
                        bonuses.Add(bioChemBonus);
                        if (bioChemBonus.Category.Name != Category.Chemistry)
                        {
                            biochemBioBo++;
                        }
                    }
                }

                // Find 1 other sci
                // Maybe we remove the other sci we found from the possible bonus
                Category math = new Category(Category.Math, 0.5);
                Category compSci = new Category(Category.ComputerScience, 0.18);
                Category earthSci = new Category(Category.EarthScience, 0.16);
                Category astro = new Category(Category.Astronomy, 0.16);
                List<Category> otherSciCats = new List<Category>();
                otherSciCats.Add(math);
                otherSciCats.Add(compSci);
                otherSciCats.Add(earthSci);
                otherSciCats.Add(astro);
                Question otherSciTossup = GetUnassignedTossup(otherSciCats, packetToMatch: packet.QemsPacketName);
                tossups.Add(otherSciTossup);
                Question otherSciBonus = null;
                if (otherSciTossup.Category.Name != Category.Math)
                {
                    try
                    {
                        otherSciBonus = GetUnassignedBonus(Category.Math, packetToMatch: packet.QemsPacketName);
                    }
                    catch (Exception)
                    {
                    }

                    if (otherSciBonus == null)
                    {
                        otherSciBonus = GetUnassignedBonus(otherSciCats, packetToMatch: packet.QemsPacketName);
                    }
                }
                else
                {
                    otherSciCats.Remove(math);

                    try
                    {
                        otherSciBonus = GetUnassignedBonus(otherSciCats, packetToMatch: packet.QemsPacketName);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (otherSciBonus == null)
                {
                    otherSciCats = new List<Category>();
                    otherSciCats.Add(math);
                    otherSciCats.Add(compSci);
                    otherSciCats.Add(earthSci);
                    otherSciCats.Add(astro);
                    otherSciBonus = GetUnassignedBonus(otherSciCats, packetToMatch: packet.QemsPacketName);
                    if (otherSciBonus == null)
                    {
                        throw new Exception("Other sci");
                    }
                }

                bonuses.Add(otherSciBonus);

                // Find 1 painting
                tossups.Add(GetUnassignedTossup(Category.Painting, packetToMatch: packet.QemsPacketName));
                bonuses.Add(GetUnassignedBonus(Category.Painting, packetToMatch: packet.QemsPacketName));

                // Find 1 music
                tossups.Add(GetUnassignedTossup(Category.Music, packetToMatch: packet.QemsPacketName));
                bonuses.Add(GetUnassignedBonus(Category.Music, packetToMatch: packet.QemsPacketName));

                // Find 1 other arts
                Category photography = new Category(Category.Photography, 0.04);
                Category sculpture = new Category(Category.Sculpture, 0.2);
                Category opera = new Category(Category.Opera, 0.2);
                Category ballet = new Category(Category.Ballet, 0.08);
                Category jazz = new Category(Category.Jazz, 0.08);
                Category miscArts = new Category(Category.MiscArts, 0.12);
                Category film = new Category(Category.Film, 0.12);
                Category architecture = new Category(Category.Architecture, 0.16);
                List<Category> otherArtsCats = new List<Category>();
                otherArtsCats.Add(photography);
                otherArtsCats.Add(sculpture);
                otherArtsCats.Add(opera);
                otherArtsCats.Add(ballet);
                otherArtsCats.Add(jazz);
                otherArtsCats.Add(miscArts);
                otherArtsCats.Add(film);
                otherArtsCats.Add(architecture);
                Question otherArtsTossup = GetUnassignedTossup(otherArtsCats, packetToMatch: packet.QemsPacketName);
                tossups.Add(otherArtsTossup);

                HashSet<string> visualArts = new HashSet<string>();
                visualArts.Add(Category.Photography);
                visualArts.Add(Category.Sculpture);
                visualArts.Add(Category.Film);
                visualArts.Add(Category.Architecture);
                Question otherArtsBonus = null;
                if (visualArts.Contains(otherArtsTossup.Category.Name))
                {
                    // Remove all visual arts
                    otherArtsCats.Remove(photography);
                    otherArtsCats.Remove(sculpture);
                    otherArtsCats.Remove(film);
                    otherArtsCats.Remove(architecture);

                    try
                    {
                        otherArtsBonus = GetUnassignedBonus(otherArtsCats, packetToMatch: packet.QemsPacketName);
                    }
                    catch (Exception)
                    {
                    }

                    if (otherArtsBonus == null)
                    {
                        // Try again with all categories
                        otherArtsCats = new List<Category>();
                        otherArtsCats.Add(photography);
                        otherArtsCats.Add(sculpture);
                        otherArtsCats.Add(opera);
                        otherArtsCats.Add(ballet);
                        otherArtsCats.Add(jazz);
                        otherArtsCats.Add(miscArts);
                        otherArtsCats.Add(film);
                        otherArtsCats.Add(architecture);
                        otherArtsBonus = GetUnassignedBonus(otherArtsCats, packetToMatch: packet.QemsPacketName);
                        if (otherArtsBonus == null)
                        {
                            throw new Exception("Other arts");
                        }
                    }
                }
                else
                {
                    // Remove all audio arts
                    otherArtsCats.Remove(opera);
                    otherArtsCats.Remove(ballet);
                    otherArtsCats.Remove(jazz);
                    otherArtsCats.Remove(miscArts);
                    try
                    {
                        otherArtsBonus = GetUnassignedBonus(otherArtsCats, packetToMatch: packet.QemsPacketName);
                    }
                    catch (Exception)
                    {
                    }

                    if (otherArtsBonus == null)
                    {
                        // Try again with all categories
                        otherArtsCats = new List<Category>();
                        otherArtsCats.Add(photography);
                        otherArtsCats.Add(sculpture);
                        otherArtsCats.Add(opera);
                        otherArtsCats.Add(ballet);
                        otherArtsCats.Add(jazz);
                        otherArtsCats.Add(miscArts);
                        otherArtsCats.Add(film);
                        otherArtsCats.Add(architecture);
                        otherArtsBonus = GetUnassignedBonus(otherArtsCats, packetToMatch: packet.QemsPacketName);
                        if (otherArtsBonus == null)
                        {
                            throw new Exception("Other arts");
                        }
                    }
                }

                bonuses.Add(otherArtsBonus);

                // Find 1 myth
                Category grecoMyth = new Category(Category.GrecoRomanMyth, 0.64);
                Category otherMyth = new Category(Category.OtherMyth, 0.36);
                List<Category> mythCategories = new List<Category>();
                mythCategories.Add(grecoMyth);
                mythCategories.Add(otherMyth);
                tossups.Add(GetUnassignedTossup(mythCategories, packetToMatch: packet.QemsPacketName)); // TODO: Maybe we need to do something differently here
                bonuses.Add(GetUnassignedBonus(mythCategories, packetToMatch: packet.QemsPacketName));

                // Find 1 religion
                Category christianity = new Category(Category.Christianity, 0.36);
                Category otherReligion = new Category(Category.OtherReligion, 0.64);
                List<Category> religionCategories = new List<Category>();
                religionCategories.Add(christianity);
                religionCategories.Add(otherReligion);
                tossups.Add(GetUnassignedTossup(religionCategories, packetToMatch: packet.QemsPacketName));
                bonuses.Add(GetUnassignedBonus(religionCategories, packetToMatch: packet.QemsPacketName));

                List<Question> otherQuestions = GetSSPhilMixed(packet.Round, random, packetToMatch: packet.QemsPacketName);
                foreach (Question question in otherQuestions)
                {
                    if (question.QType == Question.QuestionType.Tossup)
                    {
                        tossups.Add(question);
                        this.UnassignedTossups.Remove(question);
                    }
                    else
                    {
                        bonuses.Add(question);
                        this.UnassignedBonuses.Remove(question);
                    }
                }

                // find 1 geo/ce
                Category usGeography = new Category(Category.USGeography, 0.11);
                Category europeGeography = new Category(Category.EuropeanGeography, 0.11);
                Category worldGeography = new Category(Category.WorldGeography, 0.30);
                Category usCE = new Category(Category.USCurrentEvents, 0.15);
                Category foreignCE = new Category(Category.ForeignCurrentEvents, 0.33);

                string[] ceGeoCategories = new string[] { Category.Geography, Category.CurrentEvents };
                Question ceGeoTossup = GetUnassignedTossup(ceGeoCategories, packetToMatch: packet.QemsPacketName);
                tossups.Add(ceGeoTossup);
                if (ceGeoTossup.Category.ParentCategoryName == Category.CurrentEvents)
                {
                    try
                    {
                        bonuses.Add(GetUnassignedBonus(Category.Geography, packetToMatch: packet.QemsPacketName));
                    }
                    catch (Exception)
                    {
                        bonuses.Add(GetUnassignedBonus(Category.CurrentEvents, packetToMatch: packet.QemsPacketName));
                    }
                }
                else
                {
                    try
                    {
                        bonuses.Add(GetUnassignedBonus(Category.CurrentEvents, packetToMatch: packet.QemsPacketName));
                    }
                    catch (Exception)
                    {
                        bonuses.Add(GetUnassignedBonus(Category.Geography, packetToMatch: packet.QemsPacketName));
                    }
                }

                packet.Tossups = tossups;
                packet.Bonuses = bonuses;
            }

            // Create tiebreakers
            // We should actually do a second pass of everything else since it's legal to include lit/hist/world
            foreach (Packet packet in this.Packets)
            {
                // Just get 1/1 random questions to add
                var tossup = this.GetUnassignedTossupNotInTheseCategories(new string[] { "anything" });
                packet.TiebreakerTossups.Add(tossup);

                var bonus = this.GetUnassignedBonusNotInTheseCategories(new string[] { "anything" });
                packet.TiebreakerBonuses.Add(bonus);
            }

            // Randomize the questions in each packet
            foreach (Packet packet in this.Packets)
            {
                packet.Randomize(random);
            }

            // Create an output of assigned packets to actual packets
            using (StreamWriter writer = new StreamWriter(@"assigned_packets.tsv"))
            {
                writer.WriteLine("Assigned Packet\tActual Packet");
                foreach (var packet in this.Packets)
                {
                    List<Question> packetQuestions = new List<Question>();
                    packetQuestions.AddRange(packet.Tossups);
                    packetQuestions.AddRange(packet.TiebreakerBonuses);
                    packetQuestions.AddRange(packet.TiebreakerTossups);
                    packetQuestions.AddRange(packet.Bonuses);

                    foreach (var question in packetQuestions)
                    {
                        if (question.QemsPacketTagInt.HasValue)
                        {
                            writer.WriteLine($"{question.QemsPacketTagInt.Value}\t{packet.Round}\t{question.Category}");
                        }
                    }
                }
            }

            Console.WriteLine();

            // Output any leftover files to an extra packet
            if (this.UnassignedBonuses.Count > 0 || this.UnassignedTossups.Count > 0)
            {
                Packet extraPacket = new Packet(this.Packets.Count + 1);
                foreach (var tossup in this.UnassignedTossups)
                {
                    extraPacket.Tossups.Add(tossup);
                }
            
                foreach (var bonus in this.UnassignedBonuses)
                {
                    extraPacket.Bonuses.Add(bonus);
                }
            
                this.Packets.Add(extraPacket);
            }
        }

        private List<Question> GetSSPhilMixed(int packetNumber, Random random, string packetToMatch = null)
        {
            List<int> pairs = new List<int>();
            pairs.Add(1);
            pairs.Add(2);
            pairs.Add(3);
            pairs.Add(4);

            // Get the tossup and bonus counts for these categories
            int philTUCount = 0;
            int ssTUCount = 0;
            int mixedTUCount = 0;
            foreach (Question question in this.UnassignedTossups)
            {
                if (question.Category.Name == Category.Philosophy)
                {
                    philTUCount++;
                }
                else if (question.Category.ParentCategoryName == Category.SocialScience)
                {
                    ssTUCount++;
                }
                else if (question.Category.ParentCategoryName == Category.MixedOther)
                {
                    mixedTUCount++;
                }
            }

            int philBonusCount = 0;
            int ssBonusCount = 0;
            int mixedBonusCount = 0;
            foreach (Question question in this.UnassignedBonuses)
            {
                if (question.Category.Name == Category.Philosophy)
                {
                    philBonusCount++;
                }
                else if (question.Category.ParentCategoryName == Category.SocialScience)
                {
                    ssBonusCount++;
                }
                else if (question.Category.ParentCategoryName == Category.MixedOther)
                {
                    mixedBonusCount++;
                }
            }

            List<Question> questions = new List<Question>();
            bool firstIteration = true;
            while (pairs.Count > 0)
            {
                int index;
                if (firstIteration)
                {
                    index = (packetNumber % 4) + 1;
                    firstIteration = false;
                }
                else
                {
                    index = pairs[random.Next(pairs.Count)];
                }

                switch (index)
                {
                    case 1:
                        // mixed/phil, phil/ss
                        if (mixedTUCount >= 1 && philTUCount >= 1 && philBonusCount >= 1 && ssBonusCount >= 1)
                        {
                            questions.Add(GetUnassignedTossup(Category.Philosophy, packetToMatch: packetToMatch));
                            questions.Add(GetUnassignedTossup(Category.MixedOther, packetToMatch: packetToMatch));
                            questions.Add(GetUnassignedBonus(Category.Philosophy, packetToMatch: packetToMatch));
                            questions.Add(GetUnassignedBonus(Category.SocialScience, packetToMatch: packetToMatch));
                            return questions;
                        }
                        else
                        {
                            pairs.Remove(index);
                        }

                        break;
                    case 2:
                        // mixed/ss, ss/phil
                        if (mixedTUCount >= 1 && ssTUCount >= 1 && philBonusCount >= 1 && ssBonusCount >= 1)
                        {
                            questions.Add(GetUnassignedTossup(Category.MixedOther, packetToMatch: packetToMatch));
                            questions.Add(GetUnassignedBonus(Category.Philosophy, packetToMatch: packetToMatch));
                            questions.AddRange(GetSocialScienceTUBonus());
                            return questions;
                        }
                        else
                        {
                            pairs.Remove(index);
                        }

                        break;
                    case 3:
                        // phil/mixed, ss/phil
                        if (philTUCount >= 1 && ssTUCount >= 1 && mixedBonusCount >= 1 && philBonusCount >= 1)
                        {
                            questions.Add(GetUnassignedTossup(Category.Philosophy, packetToMatch: packetToMatch));
                            questions.Add(GetUnassignedTossup(Category.SocialScience, packetToMatch: packetToMatch));
                            questions.Add(GetUnassignedBonus(Category.Philosophy, packetToMatch: packetToMatch));
                            questions.Add(GetUnassignedBonus(Category.MixedOther, packetToMatch: packetToMatch));
                            return questions;
                        }
                        else
                        {
                            pairs.Remove(index);
                        }

                        break;
                    case 4:
                        // ss/mixed, phil/ss
                        if (ssTUCount >= 1 && philTUCount >= 1 && mixedBonusCount >= 1 && ssBonusCount >= 1)
                        {
                            questions.Add(GetUnassignedTossup(Category.Philosophy, packetToMatch: packetToMatch));
                            questions.Add(GetUnassignedBonus(Category.MixedOther, packetToMatch: packetToMatch));
                            questions.AddRange(GetSocialScienceTUBonus(packetToMatch: packetToMatch));
                            return questions;
                        }
                        else
                        {
                            pairs.Remove(index);
                        }

                        break;
                }
            }

            // There has to be 1 packet with an uneven number of questions, so handle that case
            try
            {
                string[] allCats = new string[] { Category.MixedOther, Category.Philosophy, Category.SocialScience };
                questions.Add(GetUnassignedTossup(allCats, packetToMatch: packetToMatch));
                questions.Add(GetUnassignedTossup(allCats, packetToMatch: packetToMatch));
                questions.Add(GetUnassignedBonus(allCats, packetToMatch: packetToMatch));
                questions.Add(GetUnassignedBonus(allCats, packetToMatch: packetToMatch));
                return questions;
            }
            catch
            {
                throw new Exception("Could not find ss/phil/other");
            }
        }

        public List<Question> GetSocialScienceTUBonus(string packetToMatch = null)
        {
            List<Question> questions = new List<Question>();
            Question tossup = GetUnassignedTossup(Category.SocialScience, packetToMatch: packetToMatch);
            Question bonus = GetUnassignedBonus(Category.SocialScience, tossup.Category.Name, packetToMatch: packetToMatch);

            questions.Add(tossup);
            questions.Add(bonus);

            return questions;
        }

        /// <summary>
        /// Loads from a CSV that lists which questions go where
        /// </summary>
        /// <param name="inputFile"></param>
        public void LoadTemplate(string inputFile, SetType setType)
        {
            bool firstLine = true;
            int packet = 0;
            string tossupMatch = " - Tossup - ";
            string bonusMatch = " - Bonus - ";

            int totalTossupCount = 0;
            int regulationTossupCount = 0;
            int totalBonusCount = 0;
            int regulationBonusCount = 0;

            if (setType == SetType.NSC)
            {
                totalTossupCount = 21;
                regulationTossupCount = 20;
                totalBonusCount = 21;
                regulationBonusCount = 20;
            }
            else if (setType == SetType.Nasat)
            {
                totalTossupCount = 21;
                regulationTossupCount = 20;
                totalBonusCount = 21;
                regulationBonusCount = 20;
            }
            else if (setType == SetType.VHSL)
            {
                totalTossupCount = 35;
                regulationTossupCount = 30;
                totalBonusCount = 26;
                regulationBonusCount = 24;
            }

            foreach (string line in File.ReadAllLines(inputFile))
            {
                if (firstLine)
                {
                    firstLine = false;
                }
                else
                {
                    Packet curPacket = this.Packets[packet];
                    List<string> cols = ReadCSVLine(line);

                    for (int i = 0; i < totalTossupCount; i++)
                    {
                        // Find the tossup of this name
                        string question = cols[i];
                        string category = question.Substring(0, question.IndexOf(tossupMatch));
                        string sIndex = question.Substring(question.IndexOf(tossupMatch) + tossupMatch.Length);
                        int index = Int32.Parse(sIndex);
                        Question tossup = FindQuestionAtIndex(this.UnassignedTossups, category, index);

                        if (i < regulationTossupCount)
                        {
                            curPacket.Tossups.Add(tossup);
                        }
                        else
                        {
                            curPacket.TiebreakerTossups.Add(tossup);
                        }
                    }

                    for (int i = totalTossupCount; i < cols.Count; i++)
                    {
                        // Find the bonus of this name
                        string question = cols[i];
                        string category = question.Substring(0, question.IndexOf(bonusMatch));
                        string sIndex = question.Substring(question.IndexOf(bonusMatch) + bonusMatch.Length);
                        int index = Int32.Parse(sIndex);
                        Question bonus = FindQuestionAtIndex(this.UnassignedBonuses, category, index);

                        if (i < totalTossupCount + regulationBonusCount)
                        {
                            curPacket.Bonuses.Add(bonus);
                        }
                        else
                        {
                            curPacket.TiebreakerBonuses.Add(bonus);
                        }
                    }

                    packet++;
                }
            }
        }

        public Question FindQuestionAtIndex(List<Question> questions, string category, int index)
        {
            int count = 1;
            foreach (Question question in questions)
            {
                if (question.Category.Name.Equals(category))
                {
                    if (count == index)
                    {
                        return question;
                    }

                    count++;
                }
            }

            // Shouldn't ever happen
            return null;
        }

        public void LoadRealQuestions(string inputFile, SetType setType)
        {
            Dictionary<string, int> tossupCategoryIndex = new Dictionary<string, int>();
            Dictionary<string, int> bonusCategoryIndex = new Dictionary<string, int>();

            this.ExtraTossups = new List<Question>();
            this.ExtraBonuses = new List<Question>();

            bool readBonusHeader = false;

            string tossupFilename = inputFile.Replace(".csv", "_tossups.csv");
            string bonusFileName = inputFile.Replace(".csv", "_bonuses.csv");

            using (StreamWriter tossupFile = new StreamWriter(tossupFilename))
            {
                using (StreamWriter bonusFile = new StreamWriter(bonusFileName))
                {
                    foreach (string line in File.ReadAllLines(inputFile))
                    {
                        if (!readBonusHeader)
                        {
                            if (line.Contains("Bonus Leadin"))
                            {
                                readBonusHeader = true;
                                bonusFile.WriteLine(line);
                                continue;
                            }

                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                tossupFile.WriteLine(line);
                            }
                        }
                        else
                        {
                            if (line.StartsWith(@"""Category"",""Subcategory""") || line.Contains("Category,Subcategory"))
                            {
                                // We've gone past reading bonuses
                                break;
                            }

                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                bonusFile.WriteLine(line);
                            }
                        }
                    }
                }
            }

            // Now read the CSV for these

            using (StreamReader tossupReader = new StreamReader(tossupFilename))
            {
                CsvReader reader = new CsvReader(tossupReader);
                while (reader.Read())
                {
                    string tossupText = reader.GetField<string>(0);
                    string tossupAnswer = reader.GetField<string>(1);
                    string category = reader.GetField<string>(2);
                    string author = reader.GetField<string>(3);
                    string packet = reader.GetField<string>(5);
                    string comments = reader.GetField<string>(7);
                    //string questionId = reader.GetField<string>(8);

                    category = ReplaceNasatCategory(category);

                    if (!tossupCategoryIndex.ContainsKey(category))
                    {
                        tossupCategoryIndex.Add(category, 1);
                    }

                    int index = tossupCategoryIndex[category];

                    // Find the question that corresponds to this
                    bool foundMatch = false;
                    foreach (Question question in this.UnassignedTossups)
                    {
                        if (question.Category.Name.Equals(category) && question.Index == index)
                        {
                            question.TossupText = tossupText;
                            question.TossupAnswer = tossupAnswer;
                            question.Author = author;
                            question.QemsPacketTag = packet;
                            //question.QemsQuestionId = questionId;
                            question.FormatComments(comments);
                            foundMatch = true;
                            break;
                        }
                    }

                    if (!foundMatch)
                    {
                        Question question = new Question(this.Categories[category], Question.QuestionType.Bonus, 0);
                        question.TossupText = tossupText;
                        question.TossupAnswer = tossupAnswer;
                        question.Author = author;
                        question.QemsPacketTag = packet;
                        question.FormatComments(comments);
                        this.ExtraTossups.Add(question);

                        Console.WriteLine(category);
                        // throw new Exception("Couldn't find match for category: " + category);
                    }

                    tossupCategoryIndex[category]++;
                }
            }

            using (StreamReader bonusReader = new StreamReader(bonusFileName))
            {
                CsvReader reader = new CsvReader(bonusReader);
                while (reader.Read())
                {
                    string leadin = reader.GetField<string>(0);
                    string part1Text = reader.GetField<string>(1);
                    string part1Answer = reader.GetField<string>(2);
                    string part2Text = reader.GetField<string>(3);
                    string part2Answer = reader.GetField<string>(4);
                    string part3Text = reader.GetField<string>(5);
                    string part3Answer = reader.GetField<string>(6);

                    string category = reader.GetField<string>(7);
                    string author = reader.GetField<string>(8);
                    string packet = reader.GetField<string>(10);
                    string comments = reader.GetField<string>(12);
                    //string questionId = reader.GetField<string>(13);

                    category = ReplaceNasatCategory(category);

                    if (!bonusCategoryIndex.ContainsKey(category))
                    {
                        bonusCategoryIndex.Add(category, 1);
                    }

                    int index = bonusCategoryIndex[category];

                    // Find the question that corresponds to this
                    bool foundMatch = false;
                    foreach (Question question in this.UnassignedBonuses)
                    {
                        if (question.Category.Name.Equals(category) && question.Index == index)
                        {
                            question.LeadinText = leadin;
                            question.Part1Text = part1Text;
                            question.Part1Answer = part1Answer;
                            question.Part2Text = part2Text;
                            question.Part2Answer = part2Answer;
                            question.Part3Text = part3Text;
                            question.Part3Answer = part3Answer;
                            question.Author = author;
                            question.QemsPacketTag = packet;
                            //question.QemsQuestionId = questionId;
                            question.FormatComments(comments);
                            foundMatch = true;
                            break;
                        }
                    }

                    // If you can't find a match, it's probably because there's an extra question
                    if (!foundMatch)
                    {
                        Question question = new Question(this.Categories[category], Question.QuestionType.Bonus, 0);
                        question.LeadinText = leadin;
                        question.Part1Text = part1Text;
                        question.Part1Answer = part1Answer;
                        question.Part2Text = part2Text;
                        question.Part2Answer = part2Answer;
                        question.Part3Text = part3Text;
                        question.Part3Answer = part3Answer;
                        question.Author = author;
                        question.FormatComments(comments);
                        question.QemsPacketTag = packet;
                        this.ExtraBonuses.Add(question);

                        Console.WriteLine(category);
                        // throw new Exception("Couldn't find match for category: " + category);
                    }

                    bonusCategoryIndex[category]++;
                }
            }
        }

        private string ReplaceNasatCategory(string category)
        {
            category = category.Replace("Literature - Literature", "Literature -");
            category = category.Replace("History - History", "History -");
            category = category.Replace("Science - Science", "Science -");
            category = category.Replace("RMP - RMP", "RMP -");
            category = category.Replace("Arts - Arts", "Arts -");
            category = category.Replace("Current Events - Current Events", "Current Events -");
            category = category.Replace("Geography - Geography", "Geography -");
            category = category.Replace("Social Science - Social Science", "Social Science -");
            category = category.Replace(" Non-Shakespeare", "");

            category = category.Replace("Shakespeare", "British");

            category = category.Replace("British Non-Shakespeare", "British");
            category = category.Replace(" (1865-1945)", "");
            category = category.Replace(" (1945-present)", "");
            category = category.Replace(" (pre-1865)", "");
            category = category.Replace(" 1400-1914", "");
            category = category.Replace(" 1914-present", "");
            category = category.Replace(" to 1400", "");

            return category;
        }

        public void OutputCategoryFiles(string setName, string font, bool includeWriterNames, bool includeCategories, bool includeComments, List<string> commentFilters, string outputDirectory, bool separateSubCategoies)
        {
            Directory.CreateDirectory(outputDirectory);

            foreach (KeyValuePair<string, Category> category in this.Categories)
            {
                List<string> tossups = new List<string>();
                List<string> bonuses = new List<string>();
                StringBuilder builder = new StringBuilder();
                builder.Append(@"{\rtf1\ansi\ansicpg1252\deff0\deflang1033\margl720\margr720\margt720\margb720");
                string formattedFileName = category.Key.Replace("\\", "-").Replace("/", "-");
                string outputFile = Path.Combine(outputDirectory, formattedFileName + ".rtf");

                if (!separateSubCategoies)
                {
                    if (string.IsNullOrEmpty(category.Value.ParentCategoryName))
                    {
                        // Get all of the questions in this category
                        foreach (Question tossup in this.UnassignedTossups)
                        {
                            if (tossup.Category.ParentCategoryName == category.Key)
                            {
                                tossups.Add(Utilities.EscapeRTF(tossup.ToString(false)));
                            }
                        }

                        foreach (Question bonus in this.UnassignedBonuses)
                        {
                            if (bonus.Category.ParentCategoryName == category.Key)
                            {
                                bonuses.Add(Utilities.EscapeRTF(bonus.ToString(false)));
                            }
                        }

                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(category.Value.ParentCategoryName))
                    {
                        foreach (Question tossup in this.UnassignedTossups)
                        {
                            if (tossup.Category.Name == category.Key)
                            {
                                tossups.Add(Utilities.EscapeRTF(tossup.ToString(false)));
                            }
                        }

                        foreach (Question bonus in this.UnassignedBonuses)
                        {
                            if (bonus.Category.Name == category.Key)
                            {
                                bonuses.Add(Utilities.EscapeRTF(bonus.ToString(false)));
                            }
                        }
                    }
                }

                if (tossups.Count > 0 || bonuses.Count > 0)
                {
                    builder.Append(@"{\colortbl;\red0\green0\blue0;\red0\green0\blue255;}");
                    builder.Append(string.Format(@"{{\footer\pard\qr {0} - {1} - Page \chpgn  of {{\field{{\*\fldinst  NUMPAGES }}}}\par}} \keep \keepn", setName, category.Key));
                    builder.Append(string.Format(@"\keep \keepn {{\fonttbl{{\f99998\fnil\fcharset0 {0};}}{{\f99999\fbidi \fswiss\fcharset0\fprq2{{\*\panose 00000000000000000000}}Source Sans Pro SemiBold;}}}}\viewkind4\uc1\par\f0\fs24\qc\b", font));
                    string tossupHeader = string.Format(@"{0} - Tossups", category.Key);
                    builder.Append(@"\line " + tossupHeader);
                    builder.Append(@"\par\sb0\sa0\par\sb0\sa0\keep\keepn\ql\b0");

                    builder.Append(Utilities.GetTossupText(tossups, 0, tossups.Count, 1, includeWriterNames, includeCategories, includeComments, commentFilters, false));

                    builder.Append(@"\page ");
                    builder.Append(string.Format(@"\keep \keepn {{\fonttbl{{\f99998\fnil\fcharset0 {0};}}{{\f99999\fbidi \fswiss\fcharset0\fprq2{{\*\panose 00000000000000000000}}Source Sans Pro SemiBold;}}}}\viewkind4\uc1\par\sb0\sa0\f0\fs24\qc\b", font));
                    string bonusHeader = string.Format(@"{0} - Bonuses", setName);
                    builder.Append(@"\line " + bonusHeader);
                    builder.Append(@"\par\sb0\sa0\par\sb0\sa0\keep\keepn\ql\b0");

                    builder.Append(Utilities.GetAcfBonusText(bonuses, 0, bonuses.Count, 1, includeWriterNames, includeCategories, includeComments, commentFilters, false));

                    builder.Append("}");
                    File.WriteAllText(outputFile, builder.ToString());
                }
            }

            Utilities.ConvertToDocx(outputDirectory);
        }

        public static void Foo()
        {

        }

        public static List<string> ReadCSVLine(string line)
        {
            if (line.Contains("Kingdom of Kush"))
            {
                Console.WriteLine(line);
            }
            
            string[] cols = line.Split(',');
            List<string> formattedCols = new List<string>();
            bool isEscapeText = false;
            StringBuilder builder = new StringBuilder();
            foreach (string col in cols)
            {
                if (isEscapeText)
                {
                    if (col.EndsWith("\""))
                    {
                        // Count how many quotes there are at the end
                        int quoteCount = 0;
                        for (int i = col.Length - 1; i >= 0; i--)
                        {
                            if (col.Substring(i, 1) == "\"")
                            {
                                quoteCount++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        // An odd number of quotes means that this is the end of the escaped text
                        if (quoteCount % 2 == 1)
                        {
                            isEscapeText = false;
                            builder.Append(col.Substring(0, col.Length - 1));
                            formattedCols.Add(builder.ToString());
                        }
                        else
                        {
                            builder.Append(col + ",");
                        }
                    }
                    else
                    {
                        builder.Append(col + ",");
                    }
                }
                else
                {
                    if (col.StartsWith("\""))
                    {
                        // Count how many quotes there are at the beginning
                        int startQuoteCount = 0;
                        for (int i = 0; i < col.Length; i++)
                        {
                            if (col.Substring(i, 1) == "\"")
                            {
                                startQuoteCount++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        int endQuoteCount = 0;
                        for (int i = col.Length - 1; i >= 0; i--)
                        {
                            if (col.Substring(i, 1) == "\"")
                            {
                                endQuoteCount++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        // An odd number of quotes means that this is the beginning of the escaped text
                        // But if there's an odd number at the end, we have the whole thing already
                        if (startQuoteCount % 2 == 1 && endQuoteCount % 2 != 1)
                        {
                            builder = new StringBuilder();
                            isEscapeText = true;
                            builder.Append(col.Substring(1) + ",");
                        }
                        else
                        {
                            // In this case, make sure to trim the start and end quotes
                            if (endQuoteCount == 1)
                            {
                                string trimmedLine = col.Substring(1);
                                trimmedLine = trimmedLine.Substring(0, trimmedLine.Length - 1);
                                formattedCols.Add(trimmedLine);
                            }
                            else
                            {
                                formattedCols.Add(col);
                            }
                        }
                    }
                    else
                    {
                        formattedCols.Add(col);
                    }
                }
            }

            for (int i = 0; i < formattedCols.Count; i++)
            {
                // There's a bug where if everything is in quotes we can still have triple quotes
                // That's the reason for this double replace
                formattedCols[i] = formattedCols[i].Replace("\"\"\"", "\"").Replace("\"\"", "\"");
            }

            return formattedCols;
        }

        /// <summary>
        /// Writes a CSV with what category each question was in the same order as the template file
        /// </summary>
        /// <param name="outputDir"></param>
        /// <param name="setType"></param>
        /// <returns></returns>
        public string WriteCategoriesToCsv(string outputDir, SetType setType)
        {
            string outputFile = Path.Combine(outputDir, "Categories_" + DateTime.Now.ToFileTimeUtc() + ".csv");
            Directory.CreateDirectory(outputDir);
            int tossupCount = 21;
            int bonusCount = 21;
            if (setType == SetType.Nasat)
            {
                tossupCount = 21;
                bonusCount = 21;
            }

            using (StreamWriter writer = new StreamWriter(outputFile))
            {
                for (int i = 1; i <= tossupCount; i++)
                {
                    writer.Write("Tossup {0},", i);
                }

                for (int i = 1; i <= bonusCount; i++)
                {
                    writer.Write("Bonus {0},", i);
                }

                writer.WriteLine();

                foreach (Packet packet in this.Packets)
                {
                    List<string> questions = new List<string>();
                    foreach (Question tossup in packet.Tossups)
                    {
                        questions.Add(tossup.Category.ToString() + " - " + tossup.QemsQuestionId);
                    }

                    foreach (Question tossup in packet.TiebreakerTossups)
                    {
                        questions.Add(tossup.Category.ToString() + " - " + tossup.QemsQuestionId);
                    }

                    foreach (Question tossup in packet.Bonuses)
                    {
                        questions.Add(tossup.Category.ToString() + " - " + tossup.QemsQuestionId);
                    }

                    foreach (Question tossup in packet.TiebreakerBonuses)
                    {
                        questions.Add(tossup.Category.ToString() + " - " + tossup.QemsQuestionId);
                    }


                    writer.WriteLine(string.Join(",", questions));
                }
            }

            return outputFile;
        }

        public string WriteOutput(string outputDir, SetType setType)
        {
            string outputFile = Path.Combine(outputDir, DateTime.Now.ToFileTimeUtc() + ".csv");
            Directory.CreateDirectory(outputDir);
            int tossupCount = 21;
            int bonusCount = 21;
            if (setType == SetType.Nasat)
            {
                tossupCount = 21;
                bonusCount = 21;
            }

            using (StreamWriter writer = new StreamWriter(outputFile))
            {
                for (int i = 1; i <= tossupCount; i++)
                {
                    writer.Write("Tossup {0},", i);
                }

                for (int i = 1; i <= bonusCount; i++)
                {
                    writer.Write("Bonus {0},", i);
                }

                writer.WriteLine();

                foreach (Packet packet in this.Packets)
                {
                    writer.Write(string.Join(",", packet.Tossups));
                    writer.Write(",");
                    writer.Write(string.Join(",", packet.TiebreakerTossups));
                    writer.Write(",");
                    writer.Write(string.Join(",", packet.Bonuses));
                    writer.Write(",");
                    writer.Write(string.Join(",", packet.TiebreakerBonuses));
                    writer.WriteLine();
                }

                if (this.ExtraTossups.Count > 0 || this.ExtraBonuses.Count > 0)
                {
                    var category = new Category("Placeholder", 0);
                    for (int i = (this.ExtraTossups.Count + 1); i <= tossupCount; i++)
                    {
                        Question placeHolder = new Question(category, Question.QuestionType.Tossup, i);
                        placeHolder.TossupText = "Placeholder";
                        placeHolder.TossupAnswer = "Placeholder";
                        placeHolder.Author = "Placeholder";
                        this.ExtraTossups.Add(placeHolder);
                    }

                    for (int i = (this.ExtraBonuses.Count + 1); i <= bonusCount; i++)
                    {
                        Question placeHolder = new Question(category, Question.QuestionType.Bonus, i);
                        placeHolder.LeadinText = "Placeholder";
                        placeHolder.Part1Text = "Placeholder";
                        placeHolder.Part1Answer = "Placeholder";
                        placeHolder.Part2Text = "Placeholder";
                        placeHolder.Part2Answer = "Placeholder";
                        placeHolder.Part3Text = "Placeholder";
                        placeHolder.Part3Answer = "Placeholder";
                        placeHolder.Author = "Placeholder";
                        this.ExtraBonuses.Add(placeHolder);
                    }

                    // Split the extra tossups and bonuses into separate packets if needed
                    var tossupBatches = this.ExtraTossups.Batch(tossupCount);
                    var bonusBatches = this.ExtraBonuses.Batch(bonusCount);
                    List<IEnumerable<Question>> tossupBatchList = new List<IEnumerable<Question>>();
                    List<IEnumerable<Question>> bonusBatchList = new List<IEnumerable<Question>>();

                    foreach (var batch in tossupBatches)
                    {
                        tossupBatchList.Add(batch);
                    }

                    foreach (var batch in bonusBatches)
                    {
                        bonusBatchList.Add(batch);
                    }

                    for (int i = 0; i < Math.Max(tossupBatchList.Count, bonusBatchList.Count); i++)
                    {
                        List<string> tossupsToWrite = new List<string>();
                        for (int j = 0; j < tossupCount; j++)
                        {
                            tossupsToWrite.Add(string.Empty);
                        }

                        List<string> bonusesToWrite = new List<string>();
                        for (int j = 0; j < bonusCount; j++)
                        {
                            bonusesToWrite.Add(string.Empty);
                        }

                        int index = 0;
                        if (i < tossupBatchList.Count)
                        {
                            foreach (var tossup in tossupBatchList[i])
                            {
                                tossupsToWrite[index] = tossup.ToString();
                                index++;
                            }
                        }

                        index = 0;
                        if (i < bonusBatchList.Count)
                        {
                            foreach (var bonus in bonusBatchList[i])
                            {
                                bonusesToWrite[index] = bonus.ToString();
                                index++;
                            }
                        }

                        writer.Write(string.Join(",", tossupsToWrite));
                        writer.Write(",");
                        writer.Write(string.Join(",", bonusesToWrite));
                        writer.WriteLine();
                    }
                }
            }

            return outputFile;
        }

        public List<Question> GetBestRandomQuestionSpread(Category[] categories, int tossups, int bonuses)
        {
            HashSet<string> categorySet = new HashSet<string>();
            foreach (Category category in categories)
            {
                categorySet.Add(category.Name);
            }

            // Try 100 times to get a random set of questions in these categories
            // Score them based on what's allowed

            List<Question> bestQuestions = new List<Question>();
            int bestScore = Int32.MaxValue;

            for (int iteration = 0; iteration < 100; iteration++)
            {
                Dictionary<string, int> categoryCount = new Dictionary<string, int>();
                Dictionary<string, int> tossupCategoryCount = new Dictionary<string, int>();
                Dictionary<string, int> bonusCategoryCount = new Dictionary<string, int>();
                List<Question> questions = new List<Question>();
                List<Question> unassignedTossupsCopy = new List<Question>();
                foreach (Question tossup in this.UnassignedTossups)
                {
                    if (categorySet.Contains(tossup.Category.Name))
                    {
                        unassignedTossupsCopy.Add(tossup);
                    }
                }

                for (int i = 0; i < tossups; i++)
                {
                    if (unassignedTossupsCopy.Count == 0)
                    {
                        throw new Exception("Not enough tossups in getting random spread: " + categories);
                    }

                    Question question = unassignedTossupsCopy[this.random.Next(unassignedTossupsCopy.Count)];
                    unassignedTossupsCopy.Remove(question);
                    questions.Add(question);
                    if (!categoryCount.ContainsKey(question.Category.Name))
                    {
                        categoryCount.Add(question.Category.Name, 0);
                    }

                    if (!tossupCategoryCount.ContainsKey(question.Category.Name))
                    {
                        tossupCategoryCount.Add(question.Category.Name, 0);
                    }

                    categoryCount[question.Category.Name]++;
                    tossupCategoryCount[question.Category.Name]++;

                    if (!string.IsNullOrEmpty(question.Category.ParentCategoryName))
                    {
                        string parentCategory = question.Category.ParentCategoryName;
                        if (!categoryCount.ContainsKey(parentCategory))
                        {
                            categoryCount.Add(parentCategory, 0);
                        }

                        if (!tossupCategoryCount.ContainsKey(parentCategory))
                        {
                            tossupCategoryCount.Add(parentCategory, 0);
                        }

                        categoryCount[parentCategory]++;
                        tossupCategoryCount[parentCategory]++;
                    }
                }

                List<Question> unassignedBonusesCopy = new List<Question>();
                foreach (Question bonus in this.UnassignedBonuses)
                {
                    if (categorySet.Contains(bonus.Category.Name))
                    {
                        unassignedBonusesCopy.Add(bonus);
                    }
                }

                for (int i = 0; i < bonuses; i++)
                {
                    if (unassignedBonusesCopy.Count == 0)
                    {
                        throw new Exception("Not enough bonuses in getting random spread: " + categories);
                    }

                    Question question = unassignedBonusesCopy[this.random.Next(unassignedBonusesCopy.Count)];
                    unassignedBonusesCopy.Remove(question);
                    questions.Add(question);
                    if (!categoryCount.ContainsKey(question.Category.Name))
                    {
                        categoryCount.Add(question.Category.Name, 0);
                    }

                    if (!bonusCategoryCount.ContainsKey(question.Category.Name))
                    {
                        bonusCategoryCount.Add(question.Category.Name, 0);
                    }

                    categoryCount[question.Category.Name]++;
                    bonusCategoryCount[question.Category.Name]++;

                    if (!string.IsNullOrEmpty(question.Category.ParentCategoryName))
                    {
                        string parentCategory = question.Category.ParentCategoryName;
                        if (!categoryCount.ContainsKey(parentCategory))
                        {
                            categoryCount.Add(parentCategory, 0);
                        }

                        if (!bonusCategoryCount.ContainsKey(parentCategory))
                        {
                            bonusCategoryCount.Add(parentCategory, 0);
                        }

                        categoryCount[parentCategory]++;
                        bonusCategoryCount[parentCategory]++;
                    }
                }

                int score = 0;
                foreach (KeyValuePair<string, int> kvp in tossupCategoryCount)
                {
                    score += this.GetDiffScore(kvp.Value, this.Categories[kvp.Key].MaximumTossups);
                    score += this.GetDiffScore(this.Categories[kvp.Key].MinimumTossups, kvp.Value);
                }

                foreach (KeyValuePair<string, int> kvp in bonusCategoryCount)
                {
                    score += this.GetDiffScore(kvp.Value, this.Categories[kvp.Key].MaximumBonuses);
                    score += this.GetDiffScore(this.Categories[kvp.Key].MinimumBonuses, kvp.Value);
                }

                foreach (KeyValuePair<string, int> kvp in categoryCount)
                {
                    score += this.GetDiffScore(kvp.Value, this.Categories[kvp.Key].MaximumTotalQuestions);
                    score += this.GetDiffScore(this.Categories[kvp.Key].MinimumTotalQuestions, kvp.Value);
                }

                if (score <= bestScore)
                {
                    bestScore = score;
                    bestQuestions = questions;
                }
            }

            return bestQuestions;
        }

        public int GetDiffScore(int limit, int actualValue)
        {
            int diff = limit - actualValue;
            return Math.Max(0, diff);
        }

        public List<Question> GetQuestions(string category, int count, Question.QuestionType qtype)
        {
            List<Question> questions = new List<Question>();
            Category cat = this.Categories[category];
            for (int i = 1; i <= count; i++)
            {
                questions.Add(new Question(cat, qtype, i));
            }

            return questions;
        }

        /// <summary>
        /// Randomly finds any unassigned tossup in this one particular category.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public Question GetUnassignedTossup(string category, string excludeCategory = null, bool isParentSearch = false, string packetToMatch = null)
        {
            List<Question> questionsInCategory = new List<Question>();
            foreach (Question question in this.UnassignedTossups)
            {
                if (question.Category.Name == category || question.Category.ParentCategoryName == category)
                {
                    if (string.IsNullOrEmpty(excludeCategory) || (question.Category.Name != excludeCategory && question.Category.ParentCategoryName != excludeCategory))
                    {
                        questionsInCategory.Add(question);
                    }
                }
            }

            if (questionsInCategory.Count == 0)
            {
                if (excludeCategory != null)
                {
                    // When this happens, it generally means that you're trying to find, say, a SS questions that isn't
                    // econ, but sometimes there's only econ left.  So just deal with it.
                    return GetUnassignedTossup(category);
                }
                else if (!isParentSearch)
                {
                    List<string> parentCategories = new List<string>();
                    parentCategories.Add(this.Categories[category].ParentCategoryName);
                    return GetUnassignedTossup(parentCategories.ToArray(), null, false);
                }
                else
                {
                    throw new Exception("Unassigned tossup: " + category);
                }
            }

            // If there was a packet to match against, see if any eligible questions match it
            if (packetToMatch != null)
            {
                foreach (var question in questionsInCategory)
                {
                    if (question.QemsPacketTag == packetToMatch)
                    {
                        this.UnassignedTossups.Remove(question);
                        return question;
                    }
                }

                // Nothing matched--see if there is anything +- 1 packets away
                int roundToMatch = GetPacketFromPacketToMatch(packetToMatch);
                foreach (var question in questionsInCategory)
                {
                    if (question.QemsPacketTagInt.HasValue)
                    {
                        if (roundToMatch - question.QemsPacketTagInt.Value >= 0 && roundToMatch - question.QemsPacketTagInt.Value <= MaxPacketDistance)
                        {
                            this.UnassignedTossups.Remove(question);
                            return question;
                        }
                    }
                }
            }

            return GetRandomQuestionTryNotToGetOneWithAssignedPacket(random, questionsInCategory, this.UnassignedTossups, GetPacketFromPacketToMatch(packetToMatch));
        }

        public int GetPacketFromPacketToMatch(string packetToMatch)
        {
            if (string.IsNullOrEmpty(packetToMatch))
            {
                return 0;
            }

            string justRound = packetToMatch.Substring(6);
            return Int32.Parse(justRound);
        }

        public Question GetRandomQuestionTryNotToGetOneWithAssignedPacket(Random random, List<Question> eligibleQuestions, List<Question> setToRemoveFrom, int round)
        {
            Question question = null;

            // Get all the questions without an assignment
            List<Question> unassignedQuestions = new List<Question>();
            foreach (var eligibleQuestion in eligibleQuestions)
            {
                if (string.IsNullOrWhiteSpace(eligibleQuestion.QemsPacketTag))
                {
                    unassignedQuestions.Add(eligibleQuestion);
                }
            }

            if (unassignedQuestions.Count > 0)
            {
                question = unassignedQuestions[random.Next(unassignedQuestions.Count)];
            }
            else
            {
                question = eligibleQuestions[random.Next(eligibleQuestions.Count)];
            }

            setToRemoveFrom.Remove(question);
            return question;
        }

        /// <summary>
        /// Randomly finds any unassigned tossup in the specified categories, based on
        /// the ratios in the passed categories.
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public Question GetUnassignedTossup(List<Category> categories, string packetToMatch = null)
        {
            if (categories.Count == 0)
            {
                throw new Exception("Unassigned tossup: " + categories);
                // return null;
            }

            List<Category> clonedCategories = new List<Category>();
            clonedCategories.AddRange(categories);

            // Use the category ratios to pick what type of question you get
            List<string> weightedCategories = new List<string>();
            foreach (Category cat in clonedCategories)
            {
                int count = (int)(100 * (cat.Ratio - (int)cat.Ratio));
                for (int i = 0; i < count; i++)
                {
                    weightedCategories.Add(cat.Name);
                }
            }

            string pickedCategory = weightedCategories[random.Next(weightedCategories.Count)];

            Question randomTossup = null;
            try
            {
                randomTossup = GetUnassignedTossup(pickedCategory, packetToMatch: packetToMatch);
            }
            catch (Exception)
            {
            }

            if (randomTossup == null)
            {
                for (int i = clonedCategories.Count - 1; i >= 0; i--)
                {
                    if (clonedCategories[i].Name == pickedCategory)
                    {
                        clonedCategories.RemoveAt(i);
                    }
                }

                return GetUnassignedTossup(clonedCategories, packetToMatch: packetToMatch);
            }
            else
            {
                return randomTossup;
            }
        }

        public Question GetUnassignedTossupNotInTheseCategories(string[] excludeCategories, string packetToMatch = null)
        {
            HashSet<string> categorySet = new HashSet<string>();

            HashSet<string> excludeSet = new HashSet<string>();
            if (excludeCategories != null)
            {
                foreach (string category in excludeCategories)
                {
                    excludeSet.Add(category);
                }
            }

            List<Question> eligibleQuestions = new List<Question>();
            foreach (Question question in this.UnassignedTossups)
            {
                if (!excludeSet.Contains(question.Category.Name) && !excludeSet.Contains(question.Category.ParentCategoryName))
                {
                    eligibleQuestions.Add(question);
                }
            }

            if (eligibleQuestions.Count == 0)
            {
                if (excludeCategories != null)
                {
                    return GetUnassignedTossupNotInTheseCategories(null, packetToMatch: packetToMatch);
                }

                throw new Exception("Unassigned tossup in all categories");
            }

            if (packetToMatch != null)
            {
                foreach (var question in eligibleQuestions)
                {
                    if (question.QemsPacketTag == packetToMatch)
                    {
                        this.UnassignedTossups.Remove(question);
                        return question;
                    }
                }

                // Nothing matched--see if there is anything +- 1 packets away
                int roundToMatch = GetPacketFromPacketToMatch(packetToMatch);
                foreach (var question in eligibleQuestions)
                {
                    if (question.QemsPacketTagInt.HasValue)
                    {
                        if (roundToMatch - question.QemsPacketTagInt.Value >= 0 && roundToMatch - question.QemsPacketTagInt.Value <= MaxPacketDistance)
                        {
                            this.UnassignedTossups.Remove(question);
                            return question;
                        }
                    }
                }
            }

            return GetRandomQuestionTryNotToGetOneWithAssignedPacket(random, eligibleQuestions, this.UnassignedTossups, GetPacketFromPacketToMatch(packetToMatch));
        }

        /// <summary>
        /// Randomly finds any unassigned tossup in the specificed categories.
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public Question GetUnassignedTossup(string[] categories, string[] excludeCategories = null, bool isParentSearch = false, string packetToMatch = null)
        {
            HashSet<string> categorySet = new HashSet<string>();
            foreach (string category in categories)
            {
                categorySet.Add(category);
            }

            HashSet<string> excludeSet = new HashSet<string>();
            if (excludeCategories != null)
            {
                foreach (string category in excludeCategories)
                {
                    excludeSet.Add(category);
                }
            }

            List<Question> eligibleQuestions = new List<Question>();
            foreach (Question question in this.UnassignedTossups)
            {
                if (categorySet.Contains(question.Category.Name) || categorySet.Contains(question.Category.ParentCategoryName))
                {
                    if (excludeCategories == null || (!excludeSet.Contains(question.Category.Name) && !excludeSet.Contains(question.Category.ParentCategoryName)))
                    {
                        eligibleQuestions.Add(question);
                    }
                }
            }

            if (eligibleQuestions.Count == 0)
            {
                if (excludeCategories != null)
                {
                    return GetUnassignedTossup(categories, packetToMatch: packetToMatch);
                }
                else if (!isParentSearch)
                {
                    List<string> parentCategories = new List<string>();
                    foreach (string category in categories)
                    {
                        parentCategories.Add(this.Categories[category].ParentCategoryName);
                    }

                    return GetUnassignedTossup(parentCategories.ToArray(), null, false, packetToMatch: packetToMatch);
                }

                throw new Exception("Unassigned tossup: " + categories);
            }

            Question foundQuestion = eligibleQuestions[random.Next(eligibleQuestions.Count)];
            this.UnassignedTossups.Remove(foundQuestion);
            return foundQuestion;
        }

        /// <summary>
        /// Gets an unassigned tossup from any remaining category.  Tries to find the
        /// best question that doesn't imbalance the packet.
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="iteration"></param>
        /// <returns></returns>
        public Question GetUnassignedTossup(Packet packet, int iteration, string packetToMatch = null)
        {
            // Potentially check for mismatches and try again a few times
            Question foundQuestion = this.UnassignedTossups[random.Next(this.UnassignedTossups.Count)];
            if (iteration == 10)
            {
                this.UnassignedTossups.Remove(foundQuestion);
                return foundQuestion;
            }

            int matchCategoryCount = 0;
            foreach (Question question in packet.Tossups)
            {
                if (question.Category.Name == foundQuestion.Category.Name)
                {
                    matchCategoryCount++;
                }
            }

            foreach (Question question in packet.Bonuses)
            {
                if (question.Category.Name == foundQuestion.Category.Name)
                {
                    matchCategoryCount++;
                }
            }

            if (matchCategoryCount == 0)
            {
                // No other matches, go ahead and add it
                this.UnassignedTossups.Remove(foundQuestion);
                return foundQuestion;
            }
            else if (matchCategoryCount >= 5)
            {
                // Already 5 of this type of question, definitely don't add it
                return GetUnassignedTossup(packet, iteration + 1, packetToMatch: packetToMatch);
            }
            else if (matchCategoryCount >= 3)
            {
                // euro hist
                if (foundQuestion.Category.Name == Category.EuropeanHistory)
                {
                    this.UnassignedTossups.Remove(foundQuestion);
                    return foundQuestion;
                }
                else
                {
                    return GetUnassignedTossup(packet, iteration + 1, packetToMatch: packetToMatch);
                }
            }
            else if (matchCategoryCount == 2)
            {
                // In some cases, having 2 questions already is fine
                switch (foundQuestion.Category.Name)
                {
                    case Category.AmericanLiterature:
                    case Category.BritishLiterature:
                    case Category.EuropeanLiterature:
                    case Category.AmericanHistory:
                    case Category.EuropeanHistory:
                    case Category.WorldHistory:
                    case Category.Biology:
                    case Category.Physics:
                        this.UnassignedTossups.Remove(foundQuestion);
                        return foundQuestion;
                    default:
                        return GetUnassignedTossup(packet, iteration + 1, packetToMatch: packetToMatch);
                }
            }
            else
            {
                // In some cases, having 1 questions already is fine
                switch (foundQuestion.Category.Name)
                {
                    case Category.AmericanLiterature:
                    case Category.BritishLiterature:
                    case Category.EuropeanLiterature:
                    case Category.AmericanHistory:
                    case Category.EuropeanHistory:
                    case Category.WorldHistory:
                    case Category.Biology:
                    case Category.Physics:
                    case Category.WorldLiterature:
                    case Category.Chemistry:
                    case Category.Painting:
                    case Category.Music:
                    case Category.Philosophy:
                    case Category.MixedOther:
                        this.UnassignedTossups.Remove(foundQuestion);
                        return foundQuestion;
                    default:
                        return GetUnassignedTossup(packet, iteration + 1, packetToMatch: packetToMatch);
                }
            }
        }

        public Question GetUnassignedBonus(string category, string excludeCategory = null, bool isParentSearch = false, string packetToMatch = null)
        {
            List<Question> questionsInCategory = new List<Question>();
            foreach (Question question in this.UnassignedBonuses)
            {
                if (question.Category.Name == category || question.Category.ParentCategoryName == category)
                {
                    if (excludeCategory == null || (excludeCategory != question.Category.Name && excludeCategory != question.Category.ParentCategoryName))
                    {
                        questionsInCategory.Add(question);
                    }
                }
            }

            if (questionsInCategory.Count == 0)
            {
                if (!string.IsNullOrEmpty(excludeCategory))
                {
                    // When this happens, you're often trying to find a ss bonus that isn't econ.  But you may
                    // only have econ left.  Just return that econ question in this case.
                    return GetUnassignedBonus(category);
                }
                else if (!isParentSearch)
                {
                    List<string> parentCategories = new List<string>();
                    parentCategories.Add(this.Categories[category].ParentCategoryName);
                    return GetUnassignedBonus(parentCategories.ToArray(), null, false);
                }
                else
                {
                    throw new Exception("Unassigned bonus: " + category);
                }
            }

            // See if there are any eligible questions that match the desired category
            if (packetToMatch != null)
            {
                foreach (var question in questionsInCategory)
                {
                    if (question.QemsPacketTag == packetToMatch)
                    {
                        this.UnassignedBonuses.Remove(question);
                        return question;
                    }
                }

                // Nothing matched--see if there is anything +- 1 packets away
                int roundToMatch = GetPacketFromPacketToMatch(packetToMatch);
                foreach (var question in questionsInCategory)
                {
                    if (question.QemsPacketTagInt.HasValue)
                    {
                        if (roundToMatch - question.QemsPacketTagInt.Value >= 0 && roundToMatch - question.QemsPacketTagInt.Value <= MaxPacketDistance)
                        {
                            this.UnassignedBonuses.Remove(question);
                            return question;
                        }
                    }
                }
            }

            return GetRandomQuestionTryNotToGetOneWithAssignedPacket(random, questionsInCategory, this.UnassignedBonuses, GetPacketFromPacketToMatch(packetToMatch));
        }

        public Question GetUnassignedBonus(List<Category> categories, string packetToMatch = null)
        {
            if (categories.Count == 0)
            {
                throw new Exception("Unassgined bonus: " + categories);
            }

            List<Category> clonedCategories = new List<Category>();
            clonedCategories.AddRange(categories);

            // Use the category ratios to pick what type of question you get
            List<string> weightedCategories = new List<string>();
            foreach (Category cat in clonedCategories)
            {
                int count = (int)(100 * (cat.Ratio - (int)cat.Ratio));
                for (int i = 0; i < count; i++)
                {
                    weightedCategories.Add(cat.Name);
                }
            }

            string pickedCategory = weightedCategories[random.Next(weightedCategories.Count)];
            Question randomBonus = null;

            try
            {
                randomBonus = GetUnassignedBonus(pickedCategory, packetToMatch: packetToMatch);
            }
            catch (Exception)
            {
            }

            if (randomBonus == null)
            {
                for (int i = clonedCategories.Count - 1; i >= 0; i--)
                {
                    if (clonedCategories[i].Name == pickedCategory)
                    {
                        clonedCategories.RemoveAt(i);
                    }
                }

                return GetUnassignedBonus(clonedCategories, packetToMatch: packetToMatch);
            }
            else
            {
                return randomBonus;
            }
        }

        public Question GetUnassignedBonusNotInTheseCategories(string[] excludeCategories, string packetToMatch = null)
        {
            HashSet<string> excludeCategorySet = new HashSet<string>();
            if (excludeCategories != null)
            {
                foreach (string category in excludeCategories)
                {
                    excludeCategorySet.Add(category);
                }
            }

            List<Question> eligibleQuestions = new List<Question>();
            foreach (Question question in this.UnassignedBonuses)
            {
                if (!excludeCategorySet.Contains(question.Category.Name) && !excludeCategorySet.Contains(question.Category.ParentCategoryName))
                {
                    eligibleQuestions.Add(question);
                }
            }

            if (eligibleQuestions.Count == 0)
            {
                if (excludeCategories != null)
                {
                    return GetUnassignedBonusNotInTheseCategories(null, packetToMatch: packetToMatch);
                }

                // TODO: Add this exception back. This is due to a bug in calculating how many CE bonuses we need vs. science bonuses in the NSC
                // For now just grab any question
                //Question randomQuestion = this.UnassignedBonuses[random.Next(this.UnassignedBonuses.Count)];
                //this.UnassignedBonuses.Remove(randomQuestion);
                //return randomQuestion;

                throw new Exception("Unassigned bonus: ");
            }

            if (packetToMatch != null)
            {
                foreach (var question in eligibleQuestions)
                {
                    if (question.QemsPacketTag == packetToMatch)
                    {
                        this.UnassignedBonuses.Remove(question);
                        return question;
                    }
                }

                // Nothing matched--see if there is anything +- 1 packets away
                int roundToMatch = GetPacketFromPacketToMatch(packetToMatch);
                foreach (var question in eligibleQuestions)
                {
                    if (question.QemsPacketTagInt.HasValue)
                    {
                        if (roundToMatch - question.QemsPacketTagInt.Value >= 0 && roundToMatch - question.QemsPacketTagInt.Value <= MaxPacketDistance)
                        {
                            this.UnassignedBonuses.Remove(question);
                            return question;
                        }
                    }
                }
            }

            return GetRandomQuestionTryNotToGetOneWithAssignedPacket(random, eligibleQuestions, this.UnassignedBonuses, GetPacketFromPacketToMatch(packetToMatch));
        }

        public Question GetUnassignedBonus(string[] categories, string[] excludeCategories = null, bool isParentSearch = false, string packetToMatch = null)
        {
            HashSet<string> categorySet = new HashSet<string>();
            foreach (string category in categories)
            {
                categorySet.Add(category);
            }

            HashSet<string> excludeCategorySet = new HashSet<string>();
            if (excludeCategories != null)
            {
                foreach (string category in excludeCategories)
                {
                    excludeCategorySet.Add(category);
                }
            }

            List<Question> eligibleQuestions = new List<Question>();
            foreach (Question question in this.UnassignedBonuses)
            {
                if (categorySet.Contains(question.Category.Name) || categorySet.Contains(question.Category.ParentCategoryName))
                {
                    if (excludeCategories == null || (!excludeCategorySet.Contains(question.Category.Name) && !excludeCategorySet.Contains(question.Category.ParentCategoryName)))
                    {
                        eligibleQuestions.Add(question);
                    }
                }
            }

            if (eligibleQuestions.Count == 0)
            {
                if (excludeCategories != null)
                {
                    return GetUnassignedBonus(categories, packetToMatch: packetToMatch);
                }
                else if (!isParentSearch)
                {
                    List<string> parentCategories = new List<string>();
                    foreach (string category in categories)
                    {
                        parentCategories.Add(this.Categories[category].ParentCategoryName);
                    }

                    return GetUnassignedBonus(parentCategories.ToArray(), null, false, packetToMatch: packetToMatch);
                }


                // TODO: Add this exception back. This is due to a bug in calculating how many CE bonuses we need vs. science bonuses in the NSC
                // For now just grab any question
                //Question randomQuestion = this.UnassignedBonuses[random.Next(this.UnassignedBonuses.Count)];
                //this.UnassignedBonuses.Remove(randomQuestion);
                //return randomQuestion;

                throw new Exception("Unassigned bonus: " + categories);
            }

            if (packetToMatch != null)
            {
                foreach (var question in eligibleQuestions)
                {
                    if (question.QemsPacketTag == packetToMatch)
                    {
                        this.UnassignedBonuses.Remove(question);
                        return question;
                    }
                }

                // Nothing matched--see if there is anything +- 1 packets away
                int roundToMatch = GetPacketFromPacketToMatch(packetToMatch);
                foreach (var question in eligibleQuestions)
                {
                    if (question.QemsPacketTagInt.HasValue)
                    {
                        if (roundToMatch - question.QemsPacketTagInt.Value >= 0 && roundToMatch - question.QemsPacketTagInt.Value <= MaxPacketDistance)
                        {
                            this.UnassignedBonuses.Remove(question);
                            return question;
                        }
                    }
                }
            }

            return GetRandomQuestionTryNotToGetOneWithAssignedPacket(random, eligibleQuestions, this.UnassignedBonuses, GetPacketFromPacketToMatch(packetToMatch));
        }
    }
}