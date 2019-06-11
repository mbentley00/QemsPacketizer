using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Docs.v1;
using Google.Apis.Docs.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using System.Text.RegularExpressions;

namespace QemsPacketizer
{
    /// <summary>
    /// Class for generating stats on the packets for upload to Google Docs and for reading directly
    /// from Google Docs to determine problems with unresolved comments, back-to-back categories, etc.
    /// </summary>
    public class PacketChecks
    {
        public static string[] Scopes = { DocsService.Scope.DocumentsReadonly, DriveService.Scope.Drive };
        public static string[] DriveScopes = { DriveService.Scope.DriveReadonly, DriveService.Scope.Drive };
        public static string ApplicationName = "PACE Affiliation App";
        public static string ApplicationName2 = "Other Client 2";

        public static void GetCategoryTotals()
        {
            string inputFile = @"C:\Users\mbent\Downloads\categories.txt";
            string output = inputFile.Replace(".txt", "_formatted.txt");
            bool isFirstLine = true;

            List<string> allCategories = new List<string>();
            allCategories.Add("RMP");
            allCategories.Add("Social Science");
            allCategories.Add("Other");

            using (StreamWriter writer = new StreamWriter(output))
            {
                foreach (var line in System.IO.File.ReadAllLines(inputFile))
                {
                    if (isFirstLine)
                    {
                        writer.Write(line + "\t");
                        writer.Write("RMP Tossups" + "\t");
                        writer.Write("RMP Bonuses" + "\t");
                        writer.Write("Social Science Tossups" + "\t");
                        writer.Write("Social Science Bonuses" + "\t");
                        writer.Write("Other Tossups" + "\t");
                        writer.WriteLine("Other Bonuses");
                        isFirstLine = false;
                    }
                    else
                    {
                        Dictionary<string, int> tossupMap = new Dictionary<string, int>();
                        Dictionary<string, int> bonusMap = new Dictionary<string, int>();
                        Dictionary<string, int> overallMap = new Dictionary<string, int>();

                        List<string> outputCategories = new List<string>();

                        string[] questions = line.Split('\t');
                        int index = 1;
                        foreach (var question in questions)
                        {
                            if (index != 22 && index != 43 && index != 1)
                            {
                                string category = question.Split('-')[0].Trim();
                                if (category == "Geography" || category == "Current Events")
                                {
                                    category = "Other";
                                }

                                if (index < 22)
                                {
                                    if (!tossupMap.ContainsKey(category))
                                    {
                                        tossupMap.Add(category, 1);
                                    }
                                    else
                                    {
                                        tossupMap[category]++;
                                    }
                                }
                                else
                                {
                                    if (!bonusMap.ContainsKey(category))
                                    {
                                        bonusMap.Add(category, 1);
                                    }
                                    else
                                    {
                                        bonusMap[category]++;
                                    }
                                }

                                if (!overallMap.ContainsKey(category))
                                {
                                    overallMap.Add(category, 1);
                                }
                                else
                                {
                                    overallMap[category]++;
                                }
                            }

                            index++;
                            outputCategories.Add(question);
                        }

                        foreach (var category in allCategories)
                        {
                            int tossups = 0;
                            int bonuses = 0;
                            if (tossupMap.ContainsKey(category))
                            {
                                tossups = tossupMap[category];
                            }

                            if (bonusMap.ContainsKey(category))
                            {
                                bonuses = bonusMap[category];
                            }

                            outputCategories.Add(tossups.ToString());
                            outputCategories.Add(bonuses.ToString());
                        }

                        writer.WriteLine(string.Join("\t", outputCategories));
                    }
                }
            }
        }

        public static void Google()
        {
            Dictionary<int, string> roundToIdMap = new Dictionary<int, string>();
            roundToIdMap.Add(1, "1ysKW8xwHVoJedOcYAm9JMH5ckdTO66JiOVy0jAJ7Pdo");
            roundToIdMap.Add(2, "1ddezVky3fMNPrtGfBAKgkyOgDNc7g3X6mNfnQafMnEI");
            roundToIdMap.Add(3, "1BO1ab7s_Ele8N8tL-vNcUFdylLG17n7YVWxB9txgNr0");
            roundToIdMap.Add(4, "1PPng2uPTorvaPP0Ao69Suu8Tb_eJ8P0RheoDk4cJdEs");
            roundToIdMap.Add(5, "1cGSTq8EeuitrhzAFkq7cImCuW7Luw7CTZDBKme-Ufeo");
            roundToIdMap.Add(6, "1kVmPNOatHbDfuyl1kjJz7Bs0N4Iu3UYNhBh0yjiWv1g");
            roundToIdMap.Add(7, "1SKAlsuY-6X392gw6o19qgjgzao7oDvC4up_YQi3lphs");
            roundToIdMap.Add(8, "16LNxPXUyXC1D_ns1XkFOPJ00wVDC-xASvKU3wI8ahHA");
            roundToIdMap.Add(9, "12sCIMeHZldZ4tpCbx7KziA28jnk4O_WDxkcAZvKVkw8");
            roundToIdMap.Add(10, "16KEEpe7VJSezv7F8QbAMLmeyHa-D6oCoHHb0ydDcKow");
            roundToIdMap.Add(11, "178fuSkdVGrEi_rjn7zKWiTWTkD1QhVrVH5hRU32E5Is");
            roundToIdMap.Add(12, "11ftt-fIhuSnBkfw2HDuk8kPD0MKLyBDPRpEa8xK28_Y");
            roundToIdMap.Add(13, "1LS1sAwCfJCZ9VAU8fySBQL3cYRTCaIzKi-_ZJKnRxnU");
            roundToIdMap.Add(14, "1_dqL46ZcLizVbIdllxshVH58b53IZEjmq3uAxQmZETo");
            roundToIdMap.Add(15, "1hljYA9mrdi-NLeMabXmQ-eZ1HV8N6zvlB3M0avqpI2U");
            roundToIdMap.Add(16, "1ntPRprG8WqvJqHiOf9QZPXs9hRsBatlQiyNxSmwMxcY");
            roundToIdMap.Add(17, "11tzNCnLpwb9vfEmFg9VGym2dmB0v6_f0Ooioh_9Uppc");
            roundToIdMap.Add(18, "1b9j-NFfCKgmwjNlOK1hXu798c0guAO9XVcq1CQj3tkE");
            roundToIdMap.Add(19, "17pcwSS-ReQfvh0uxcBu1C9VZbLZhk9se8vEFyrvw3h0");
            roundToIdMap.Add(20, "1bMrLEi9CoAcxbAZr99KIHY21tYsZ0iE2udvzDZmTHfE");
            roundToIdMap.Add(21, "1xC9YTqI976fCAEIloLowsLHPpQW4sqoUOa2K0xMf0KE");
            roundToIdMap.Add(22, "18PTr4AN8W8bPCd11O30NDFfNWescSXEbeREF5VM-wNs");
            roundToIdMap.Add(23, "1woIVdbgS082a9FAOkPPA-SXaYHicSilAA3ZAS9iEZd0");
            roundToIdMap.Add(24, "1keg-WKGm8NOD4RJKeesXUKsCDkHBRj90Na8nJuD_IV8");

            UserCredential docsCredential;
            UserCredential driveCredential;

            string credentialsFile = @"C:\Users\mbentley\Downloads\credentials.json";
            if (!System.IO.File.Exists(credentialsFile))
            {
                credentialsFile = @"F:\OneDrive\credentials.json";
                if (!System.IO.File.Exists(credentialsFile))
                {
                    credentialsFile = @"C:\Users\mbentley\OneDrive\credentials.json";
                }
            }

            string driveCredentialsFile = @"C:\Users\mbentley\OneDrive\Code\CategoryTotals\credentials_drive.json";

            using (var stream = new FileStream(credentialsFile, FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                docsCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            using (var stream = new FileStream(driveCredentialsFile, FileMode.Open, FileAccess.Read))
            {
                string credPath = "token_drive.json";
                driveCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    DriveScopes,
                    "mbentleypace@gmail.com",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = driveCredential,
                ApplicationName = ApplicationName2,
            });

            // Create Google Sheets API service.
            var docsService = new DocsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = docsCredential,
                ApplicationName = ApplicationName,
            });

            Regex categoryRegex = new Regex(@"(<(.+))?, ((.+)\-(.+))>");
            Regex innerCategory = new Regex(@"<(.+), ((.+)\-(.+))");

            List<string> menteeQuestions = new List<string>();

            List<string> comments = new List<string>();

            using (StreamWriter writer = new StreamWriter(@"new_categories.tsv"))
            {
                string header = "Packet	Tossup 1	Tossup 2	Tossup 3	Tossup 4	Tossup 5	Tossup 6	Tossup 7	Tossup 8	Tossup 9	Tossup 10	Tossup 11	Tossup 12	Tossup 13	Tossup 14	Tossup 15	Tossup 16	Tossup 17	Tossup 18	Tossup 19	Tossup 20	Tossup 21	Bonus 1	Bonus 2	Bonus 3	Bonus 4	Bonus 5	Bonus 6	Bonus 7	Bonus 8	Bonus 9	Bonus 10	Bonus 11	Bonus 12	Bonus 13	Bonus 14	Bonus 15	Bonus 16	Bonus 17	Bonus 18	Bonus 19	Bonus 20	Bonus 21";
                writer.WriteLine(header);

                foreach (var kvp in roundToIdMap)
                {
                    Console.WriteLine("Round " + kvp.Key);
                    writer.Write(kvp.Key + "\t");

                    // Read the affiliation data
                    string docId = kvp.Value;

                    // Get the comments info
                    var commentsRequest = service.Comments.List(docId);
                    commentsRequest.Fields = "comments";
                    var commentsResult = commentsRequest.Execute();
                    if (commentsResult.Comments != null)
                    {
                        foreach (var comment in commentsResult.Comments)
                        {
                            if ((comment.Deleted == null || comment.Deleted.Value == false) && (comment.Resolved == null || comment.Resolved.Value == false))
                            {
                                if (comment.QuotedFileContent != null && comment.QuotedFileContent.Value != null && comment.QuotedFileContent.Value.Contains("@@"))
                                {
                                    continue;
                                }

                                List<string> replyContents = new List<string>();
                                var repliesRequest = service.Replies.List(docId, comment.Id);
                                repliesRequest.Fields = "*";
                                var repliesResult = repliesRequest.Execute();
                                if (repliesResult.Replies != null)
                                {
                                    foreach (var reply in repliesResult.Replies)
                                    {
                                        if (reply.Content != null)
                                        {
                                            replyContents.Add(reply.Content);
                                        }
                                    }
                                }

                                comments.Add($"Packet {kvp.Key}, Unresolved Comment: '{comment.Content}', Replies: '{string.Join("\t", replyContents)}', on '{comment.QuotedFileContent.Value}'");
                            }
                        }
                    }

                    var request = docsService.Documents.Get(docId);
                    Document doc = request.Execute();

                    var allBody = doc.Body.ToString();

                    List<string> categories = new List<string>();

                    int questionIndex = 1;
                    foreach (var contentItem in doc.Body.Content)
                    {
                        if (contentItem.Paragraph != null && contentItem.Paragraph.Elements != null)
                        {
                            foreach (var element in contentItem.Paragraph.Elements)
                            {
                                if (element.TextRun != null)
                                {
                                    var text = element.TextRun.Content;

                                    if (text.Contains("@@"))
                                    {
                                        comments.Add($"Packet {kvp.Key}, Question {questionIndex}");
                                    }

                                    if (categoryRegex.IsMatch(text))
                                    {
                                        var match = categoryRegex.Match(text);
                                        var author = match.Groups[1].Value;
                                        if (author.Contains("Mentee"))
                                        {
                                            menteeQuestions.Add($"Packet {kvp.Key}, Question {questionIndex}");
                                        }

                                        var category = match.Groups[3].Value;
                                        if (innerCategory.IsMatch(category))
                                        {
                                            var match2 = innerCategory.Match(category);

                                            category = match2.Groups[2].Value;
                                            Console.WriteLine();
                                        }


                                        categories.Add(category);

                                        questionIndex++;
                                    }

                                }
                            }
                        }
                    }

                    Console.WriteLine(categories.Count);

                    writer.WriteLine(string.Join("\t", categories));
                }
            }

            System.IO.File.WriteAllLines(@"mentees.txt", menteeQuestions);
            System.IO.File.WriteAllLines(@"comments.txt", comments);
        }

        public static void CheckPackets()
        {
            // Figure out categories that are skewed to TB/Emergency

            // Is this category skewed toward the front/back of the tournament?
            Dictionary<string, List<int>> categoryToPacketTossupMap = new Dictionary<string, List<int>>();
            Dictionary<string, List<int>> categoryToPacketBonusMap = new Dictionary<string, List<int>>();

            Dictionary<string, int> categoryToRegularTossups = new Dictionary<string, int>();
            Dictionary<string, int> categoryToRegularBonuses = new Dictionary<string, int>();
            Dictionary<string, int> categoryToTiebreakerTossups = new Dictionary<string, int>();
            Dictionary<string, int> categoryToTiebreakerBonuses = new Dictionary<string, int>();

            Dictionary<int, int> packetToRoundMap = new Dictionary<int, int>();
            packetToRoundMap.Add(1, 1);
            packetToRoundMap.Add(2, 2);
            packetToRoundMap.Add(3, 3);
            packetToRoundMap.Add(4, 4);
            packetToRoundMap.Add(5, 5);
            packetToRoundMap.Add(6, 6);
            packetToRoundMap.Add(7, 7);
            packetToRoundMap.Add(19, 8);
            packetToRoundMap.Add(8, 9);
            packetToRoundMap.Add(9, 10);
            packetToRoundMap.Add(10, 11);
            packetToRoundMap.Add(11, 12);
            packetToRoundMap.Add(12, 13);
            packetToRoundMap.Add(20, 14);
            packetToRoundMap.Add(13, 15);
            packetToRoundMap.Add(14, 16);
            packetToRoundMap.Add(15, 17);
            packetToRoundMap.Add(16, 18);
            packetToRoundMap.Add(17, 19);
            packetToRoundMap.Add(18, 20);
            packetToRoundMap.Add(21, 21);
            packetToRoundMap.Add(22, 22);
            packetToRoundMap.Add(23, 23);
            packetToRoundMap.Add(24, 24);

            HashSet<int> tiebreakers = new HashSet<int>();
            tiebreakers.Add(19);
            tiebreakers.Add(20);
            tiebreakers.Add(21);
            tiebreakers.Add(24);

            string inputFile = @"new_categories.tsv";
            string outputFile = inputFile.Replace(".tsv", "_check.txt");
            int packet = 1;
            using (StreamWriter writer = new StreamWriter(outputFile))
            {
                foreach (var line in System.IO.File.ReadAllLines(inputFile).Skip(1))
                {
                    writer.WriteLine();
                    writer.WriteLine("Packet " + packet);

                    int tossups_artsFirstHalf = 0;
                    int tossups_artsSecondHalf = 0;
                    int tossups_rmpFirstHalf = 0;
                    int tossups_rmpSecondHalf = 0;

                    int bonuses_artsFirstHalf = 0;
                    int bonuses_artsSecondHalf = 0;
                    int bonuses_rmpFirstHalf = 0;
                    int bonuses_rmpSecondHalf = 0;

                    int paintingCount = 0;
                    int musicCount = 0;
                    int otherVisual = 0;
                    int otherAudio = 0;

                    if (!string.IsNullOrEmpty(line))
                    {
                        string[] columns = line.Split('\t');
                        List<string> tossups = new List<string>();
                        for (int i = 1; i < 22; i++)
                        {
                            tossups.Add(columns[i]);
                        }

                        List<string> bonuses = new List<string>();
                        for (int i = 22; i < 43; i++)
                        {
                            bonuses.Add(columns[i]);
                        }

                        for (int i = 0; i <= 20; i++)
                        {
                            if (!categoryToPacketTossupMap.ContainsKey(tossups[i]))
                            {
                                categoryToPacketTossupMap.Add(tossups[i], new List<int>());
                            }

                            if (!categoryToPacketBonusMap.ContainsKey(bonuses[i]))
                            {
                                categoryToPacketBonusMap.Add(bonuses[i], new List<int>());
                            }

                            int placementPacket = packetToRoundMap[packet];

                            categoryToPacketTossupMap[tossups[i]].Add(placementPacket);
                            categoryToPacketBonusMap[bonuses[i]].Add(placementPacket);

                            if (tiebreakers.Contains(packet) || i == 20)
                            {
                                if (!categoryToTiebreakerTossups.ContainsKey(tossups[i]))
                                {
                                    categoryToTiebreakerTossups.Add(tossups[i], 1);
                                }
                                else
                                {
                                    categoryToTiebreakerTossups[tossups[i]]++;
                                }

                                if (!categoryToTiebreakerBonuses.ContainsKey(bonuses[i]))
                                {
                                    categoryToTiebreakerBonuses.Add(bonuses[i], 1);
                                }
                                else
                                {
                                    categoryToTiebreakerBonuses[bonuses[i]]++;
                                }
                            }
                            else
                            {
                                if (!categoryToRegularTossups.ContainsKey(tossups[i]))
                                {
                                    categoryToRegularTossups.Add(tossups[i], 1);
                                }
                                else
                                {
                                    categoryToRegularTossups[tossups[i]]++;
                                }

                                if (!categoryToRegularBonuses.ContainsKey(bonuses[i]))
                                {
                                    categoryToRegularBonuses.Add(bonuses[i], 1);
                                }
                                else
                                {
                                    categoryToRegularBonuses[bonuses[i]]++;
                                }
                            }

                            int index = i + 1;
                            string tossupCategory = GetParentCategory(tossups[i]);
                            string bonusCategory = GetParentCategory(bonuses[i]);
                            if (tossupCategory == bonusCategory && index <= 5)
                            {
                                writer.WriteLine($"Tossup and Bonus {index} are both {tossupCategory}");
                            }

                            if (tossupCategory == "RMP")
                            {
                                if (index <= 10)
                                {
                                    tossups_rmpFirstHalf++;
                                }
                                else if (index <= 20)
                                {
                                    tossups_rmpSecondHalf++;
                                }
                            }

                            if (tossupCategory == "Fine Arts")
                            {
                                if (index <= 10)
                                {
                                    tossups_artsFirstHalf++;
                                }
                                else if (index <= 20)
                                {
                                    tossups_artsSecondHalf++;
                                }
                            }

                            if (bonusCategory == "RMP")
                            {
                                if (index <= 10)
                                {
                                    bonuses_rmpFirstHalf++;
                                }
                                else if (index <= 20)
                                {
                                    bonuses_rmpSecondHalf++;
                                }
                            }

                            if (bonusCategory == "Fine Arts")
                            {
                                if (index <= 10)
                                {
                                    bonuses_artsFirstHalf++;
                                }
                                else if (index <= 20)
                                {
                                    bonuses_artsSecondHalf++;
                                }
                            }

                            if (index <= 20)
                            {
                                if (tossups[i] == "Fine Arts - Painting")
                                {
                                    paintingCount++;
                                }
                                else if (tossups[i] == "Fine Arts - Music")
                                {
                                    musicCount++;
                                }
                                else if (tossups[i] == "Fine Arts - Architecture" || tossups[i] == "Fine Arts - Architecture" || tossups[i] == "Fine Arts - Film" || tossups[i] == "Fine Arts - Miscellaneous" || tossups[i] == "Fine Arts - Photography" || tossups[i] == "Fine Arts - Sculpture")
                                {
                                    otherVisual++;
                                }
                                else if (tossups[i].Contains("Fine Arts"))
                                {
                                    otherAudio++;
                                }

                                if (bonuses[i] == "Fine Arts - Painting")
                                {
                                    paintingCount++;
                                }
                                else if (bonuses[i] == "Fine Arts - Music")
                                {
                                    musicCount++;
                                }
                                else if (bonuses[i] == "Fine Arts - Architecture" || bonuses[i] == "Fine Arts - Architecture" || bonuses[i] == "Fine Arts - Film" || bonuses[i] == "Fine Arts - Miscellaneous" || bonuses[i] == "Fine Arts - Photography" || bonuses[i] == "Fine Arts - Sculpture")
                                {
                                    otherVisual++;
                                }
                                else if (bonuses[i].Contains("Fine Arts"))
                                {
                                    otherAudio++;
                                }
                            }

                            if (i < 20)
                            {
                                string nextTossupCategory = GetParentCategory(tossups[i + 1]);
                                string nextBonusCategory = GetParentCategory(bonuses[i + 1]);
                                if (tossupCategory == nextTossupCategory)
                                {
                                    writer.WriteLine($"Tossup {index} and {index + 1} are both {tossupCategory}");
                                }

                                if (bonusCategory == nextBonusCategory)
                                {
                                    writer.WriteLine($"Bonus {index} and {index + 1} are both {bonusCategory}");
                                }
                            }
                        }
                    }

                    if (tossups_artsFirstHalf >= 3)
                    {
                        writer.WriteLine("3 arts tossups in first half");
                    }

                    if (tossups_artsSecondHalf >= 3)
                    {
                        writer.WriteLine("3 arts tossups in second half");
                    }

                    if (tossups_rmpFirstHalf >= 3)
                    {
                        writer.WriteLine("3 RMP tossups in first half");
                    }

                    if (tossups_rmpSecondHalf >= 3)
                    {
                        writer.WriteLine("3 RMP tossups in second half");
                    }

                    if (bonuses_artsFirstHalf >= 3)
                    {
                        writer.WriteLine("3 arts bonuses in first half");
                    }

                    if (bonuses_artsSecondHalf >= 3)
                    {
                        writer.WriteLine("3 arts bonuses in second half");
                    }

                    if (bonuses_rmpFirstHalf >= 3)
                    {
                        writer.WriteLine("3 RMP bonuses in first half");
                    }

                    if (bonuses_rmpSecondHalf >= 3)
                    {
                        writer.WriteLine("3 RMP bonuses in second half");
                    }

                    if (paintingCount != 2)
                    {
                        writer.WriteLine("Painting count error: " + paintingCount);
                    }

                    if (musicCount != 2)
                    {
                        writer.WriteLine("Music count error: " + musicCount);
                    }

                    if (otherVisual != 1)
                    {
                        writer.WriteLine("Other visual count error: " + otherVisual);
                    }

                    if (otherAudio != 1)
                    {
                        writer.WriteLine("Other audio count error: " + otherAudio);
                    }

                    packet++;
                }

                writer.WriteLine();
                writer.WriteLine("Average tossup / bonus placement");

                foreach (var kvp in categoryToPacketTossupMap.OrderBy(x => x.Key))
                {
                    double averageTossup = Math.Round(kvp.Value.Average(), 1);
                    double averageBonus = Math.Round(categoryToPacketBonusMap[kvp.Key].Average(), 1);
                    writer.WriteLine($"{kvp.Key}\t{averageTossup}\t{averageBonus}");
                }

                writer.WriteLine();
                writer.WriteLine("Rate in regular tossup / tiebreaker round tossup / regular bonus / tiebreaker round bonus");
                foreach (var kvp in categoryToRegularTossups.OrderBy(x => x.Key))
                {
                    // Total regular questions is 20 * 20 = 400

                    // Total TB questions = (21 * 4) + (20 * 1) = 104

                    double regularTossup = Math.Round(kvp.Value / 400.0, 4);
                    double regularBonus = 0;
                    if (categoryToRegularBonuses.ContainsKey(kvp.Key))
                    {
                        regularBonus = Math.Round(categoryToRegularBonuses[kvp.Key] / 400.0, 4);
                    }

                    double tiebreakerTossup = 0;
                    double tiebreakerBonus = 0;
                    if (categoryToTiebreakerTossups.ContainsKey(kvp.Key))
                    {
                        tiebreakerTossup = Math.Round(categoryToTiebreakerTossups[kvp.Key] / 104.0, 4);
                    }

                    if (categoryToTiebreakerBonuses.ContainsKey(kvp.Key))
                    {
                        tiebreakerBonus = Math.Round(categoryToTiebreakerBonuses[kvp.Key] / 104.0, 4);
                    }

                    double tossupDiff = regularTossup - tiebreakerTossup;
                    double bonusDiff = regularBonus - tiebreakerBonus;

                    writer.WriteLine($"{kvp.Key}\t{regularTossup}\t{tiebreakerTossup}\t{tossupDiff}\t{regularBonus}\t{tiebreakerBonus}\t{bonusDiff}");
                }
            }
        }

        public static string GetParentCategory(string fullCategory)
        {
            return fullCategory.Split('-')[0].Trim();
        }
    }
}
