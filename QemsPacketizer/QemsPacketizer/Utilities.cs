namespace QemsPacketizer
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Class for writing compiled packets to RTFs and Word documents
    /// </summary>
    public class Utilities
    {
        /// <summary>
        /// Takes all of the RTF files in a directory and converts them to docx
        /// </summary>
        /// <param name="directory">Directory to convert files from</param>
        public static void ConvertToDocx(string directory)
        {
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            foreach (string file in Directory.GetFiles(directory, "*.rtf"))
            {
                string outputFile = file.Replace("rtf", "docx");
                Microsoft.Office.Interop.Word.Document wordDoc = wordApp.Documents.Open(file);

                object _DocxFileName = outputFile;
                Object FileFormat = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatXMLDocument;

                wordDoc.SaveAs2(ref _DocxFileName, ref FileFormat);
                wordDoc.Close();
                wordDoc = null;
                GC.Collect();
            }

            wordApp = null;
            GC.Collect();
        }

        /// <summary>
        /// For the given line of text, format it for RTF. Does things like convert
        /// QEMS italics (~) into RTF italics. Escapes necessary RTF characters.
        /// </summary>
        /// <param name="line">Line of text to convert to RTF</param>
        /// <param name="ignoreUnderscores">Whether to ignore underlining/prompting or not</param>
        /// <returns>The line of text converted to RTF</returns>
        public static string GetFormattedText(string line, bool ignoreUnderscores)
        {
            // Convert any instance of multiple spaces into just one space
            Regex spaceRegex = new Regex("( )+");
            line = spaceRegex.Replace(line, " ").Trim();

            line = GetRtfUnicodeEscapedString(line);
            StringBuilder builder = new StringBuilder();
            builder.Append(@"\f99998 ");

            // Track QEMS formatting
            bool isInParen = false;
            bool isInAnswer = false;
            bool isInPrompt = false;
            bool isInItalics = false;
            bool isInSubscript = false;
            bool isInSuperscript = false;

            string nextChar = "";
            string nextChar2 = "";

            if (ignoreUnderscores)
            {
                // We're in a tossup, see if we need to bold everything before power
                if (line.Contains("(*)"))
                {
                    line = "▲" + line;
                    line = line.Replace("(*)", "◊");
                }
            }

            var charArray = line.ToCharArray();
            for (int i = 0; i < charArray.Length; i++)
            {
                var c = charArray[i];

                if (i < line.Length - 1)
                {
                    nextChar = line.Substring(i + 1, 1);
                }
                else
                {
                    nextChar = "";
                }

                if (i < line.Length - 2)
                {
                    nextChar2 = line.Substring(i + 2, 1);
                }
                else
                {
                    nextChar2 = "";
                }

                if (c == '_' && !ignoreUnderscores)
                {
                    if (nextChar == "_")
                    {
                        // Double underlines mean prompt
                        if (isInPrompt)
                        {
                            builder.Append(@"\ul0 ");
                            isInPrompt = false;
                            i++; // Skip processing the second underline
                        }
                        else
                        {
                            builder.Append(@"\ul ");
                            isInPrompt = true;
                            i++; // Skip processing the second underline
                        }
                    }
                    else
                    {
                        if (isInAnswer)
                        {
                            builder.Append(@"\b0 \ul0 ");
                            isInAnswer = false;
                        }
                        else
                        {
                            builder.Append(@"\b \ul ");
                            isInAnswer = true;
                        }
                    }
                }
                else if (c == '~')
                {
                    if (isInItalics)
                    {
                        builder.Append(@"\i0 ");
                        isInItalics = false;
                    }
                    else
                    {
                        builder.Append(@"\i ");
                        isInItalics = true;
                    }
                }
                else if (c == '\\')
                {
                    // Rules for backslashes:
                    // Subscript: \\sText\\s
                    // Superscript: \\SText\\S
                    // Literal parentheses: \\(text\\)
                    // Literal backslash: test \\ test2
                    // The literal backslash is the default if we don't find an s,S,(,) after the double backslash

                    if (nextChar == "\\")
                    {
                        if (nextChar2 == "s")
                        {
                            // Start or stop subscripts
                            if (isInSubscript)
                            {
                                builder.Append(@"\nosupersub ");
                                isInSubscript = false;
                            }
                            else
                            {
                                builder.Append(@"\sub ");
                                isInSubscript = true;
                            }

                            i += 2;
                        }
                        else if (nextChar2 == "S")
                        {
                            // Start or stop superscripts
                            if (isInSuperscript)
                            {
                                builder.Append(@"\nosupersub ");
                                isInSuperscript = false;
                            }
                            else
                            {
                                builder.Append(@"\super ");
                                isInSuperscript = true;
                            }

                            i += 2;
                        }
                        else if (nextChar2 == "(")
                        {
                            builder.Append("(");
                            i += 2;
                        }
                        else if (nextChar2 == ")")
                        {
                            builder.Append(")");
                            i += 2;
                        }
                        else
                        {
                            // In the default case, print a double backslash so that RTF escapes properly
                            builder.Append("\\\\");
                            i++;
                        }
                    }
                    else if (nextChar == "u")
                    {
                        // Print the literal \u for unicode character output
                        builder.Append("\\u");
                        i++;
                    }
                    else
                    {
                        // Don't write the single backslash for now, it should never be the case that this happens
                        Console.WriteLine("Unexpected single backslash");
                    }
                }
                else if (c == '(')
                {
                    if (!isInParen)
                    {
                        isInParen = true;
                        builder.Append(@"\f99999\fs20(");
                    }
                    else
                    {
                        builder.Append(c);
                    }
                }
                else if (c == ')')
                {
                    if (isInParen)
                    {
                        isInParen = false;
                        builder.Append(@")\f99998\fs24 ");
                    }
                    else
                    {
                        builder.Append(c);
                    }
                }
                else if (c == '▲')
                {
                    builder.Append(@"\b ");
                }
                else if (c == '◊')
                {
                    builder.Append(@"(*)\b0 ");
                }
                else
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// For the given text, escapes it for RTF unicode
        /// </summary>
        /// <param name="inputString">Unescaped text</param>
        /// <returns>Escaped text for RTF</returns>
        public static string GetRtfUnicodeEscapedString(string inputString)
        {
            var sb = new StringBuilder();
            foreach (var c in inputString)
            {
                if (c <= 0x7f)
                    sb.Append(c);
                else
                    sb.Append("\\u" + Convert.ToUInt32(c) + "?");
            }
            return sb.ToString();
        }

        /// <summary>
        /// For the given line, escapes it for RTF
        /// </summary>
        /// <param name="line">Unescaped text</param>
        /// <returns>Escaped text for RTF</returns>
        public static string EscapeRTF(string line)
        {
            return line.Replace("\\", "\\\\").Replace("{", "\\{").Replace("}", "\\}");
        }

        /// <summary>
        /// For the given CSV of the packets, generates the RTF documents
        /// </summary>
        /// <param name="file">CSV of the packets. e.g. a file that is named something like 131384202599968092.csv</param>
        /// <param name="outputDir">Output directory to place the RTF files</param>
        /// <param name="includeScoresheet">If true, add a scoresheet to the first page</param>
        /// <param name="convertToDocx">If true, convert the RTF files to .docx files</param>
        /// <param name="setType">What question set type to produce</param>
        /// <param name="logo">Logo file to add to top of documents</param>
        /// <param name="includeComments">If true, include comments below questions</param>
        /// <param name="commentFilters">If non-empty, only include comments that match at least one of these terms</param>
        /// <param name="includeWriterNames">If true, include writer names under questions</param>
        /// <param name="setName">Name of the question set</param>
        /// <param name="font">Name of the font to use</param>
        public static void GenerateRtf(
            string file,
            string outputDir,
            bool includeScoresheet,
            bool convertToDocx,
            QuestionSet.SetType setType,
            string logo,
            bool includeComments,
            List<string> commentFilters,
            bool includeWriterNames,
            bool includeCategories,
            string setName,
            string font)
        {
            Directory.CreateDirectory(outputDir);

            string bitmapText = string.Empty;
            if (!string.IsNullOrEmpty(logo))
            {
                Bitmap bitmap = LoadImage(logo);
                bitmapText = GetEmbedImageString(bitmap);
            }

            int round = 1;
            foreach (string line in File.ReadAllLines(file).Skip(1))
            {
                string outputRtf = Path.Combine(outputDir, string.Format("Round {0}.rtf", round.ToString("D2")));

                string rtfLine = EscapeRTF(line);
                List<string> cols = QuestionSet.ReadCSVLine(rtfLine);

                StringBuilder builder = new StringBuilder();
                if (includeScoresheet)
                {
                    // Load the scoresheet rtf to place at the top of the document
                    string scoresheetRtf = File.ReadAllText(@"2017NSCScoresheet.rtf");

                    // Set the round number to the proper value
                    scoresheetRtf = scoresheetRtf.Replace("CHANGE ME", round.ToString("D2"));
                    scoresheetRtf = AddFontToScoresheet(scoresheetRtf, font);
                    builder.Append(scoresheetRtf);
                }
                else
                {
                    builder.Append(@"{\rtf1\ansi\ansicpg1252\deff0\deflang1033\margl720\margr720\margt720\margb720");
                }

                string formattedRound = round.ToString("D2");

                if (setType == QuestionSet.SetType.NSC || setType == QuestionSet.SetType.Nasat)
                {
                    builder.Append(GetNscOrNasatText(cols, includeWriterNames, includeCategories, includeComments, commentFilters, setName, formattedRound, bitmapText, font));
                }
                else if (setType == QuestionSet.SetType.VHSL)
                {
                    builder.Append(GetVhslText(cols, includeWriterNames, includeCategories, includeComments, commentFilters, setName, formattedRound, bitmapText, font));
                }

                builder.Append("}");
                File.WriteAllText(outputRtf, builder.ToString());
                round++;
            }

            ConvertToDocx(outputDir);
        }

        /// <summary>
        /// Adds the font information to the scoresheet RTF document
        /// </summary>
        /// <param name="scoresheetRtf">Scoresheet rtf</param>
        /// <param name="font">Font name</param>
        /// <returns>Updated rtf</returns>
        public static string AddFontToScoresheet(string scoresheetRtf, string font)
        {

            // Source Sans Pro Semibold
            string replaceText = string.Format(@"{{\fonttbl{{\f99998\fnil\fcharset0 {0};}}{{\f99999\fbidi \fswiss\fcharset0\fprq2{{\*\panose 00000000000000000000}}Source Sans Pro SemiBold;}}", font);
            // string replaceText = string.Format(@"{{\fonttbl{{\f99998\fnil\fcharset0 {0};}}{{\f99999\fbidi \fswiss\fcharset0\fprq2{{\*\panose 00000000000000000000}}Source Sans Pro SemiBold;}}", font);
            return scoresheetRtf.Replace(@"{\fonttbl", replaceText);
        }

        /// <summary>
        /// For a given packet in raw format, generate an RTF according to how it should be constructed
        /// for the NSC or NASAT
        /// </summary>
        /// <param name="columns">Columns parsed from the CSV with question information. Each column is a tossup or bonus.</param>
        /// <param name="includeWriterNames">If true, include writer names</param>
        /// <param name="includeCategories">If true, include category names</param>
        /// <param name="includeComments">If true, include comments under the questions</param>
        /// <param name="commentFilters">If non-empty, filter comments to only those which contain these words</param>
        /// <param name="setName">Name of the question set</param>
        /// <param name="round">Round number</param>
        /// <param name="bitmapText">The binary representation of the logo bitmap</param>
        /// <param name="font">Font name</param>
        /// <returns>RTF rendering of an NSC or NASAT packet</returns>
        public static string GetNscOrNasatText(List<string> columns, bool includeWriterNames, bool includeCategories, bool includeComments, List<string> commentFilters, string setName, string round, string bitmapText, string font)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(@"{\colortbl;\red0\green0\blue0;\red0\green0\blue255;}");
            builder.Append(string.Format(@"{{\footer\pard\qr {0} - Round {1} - Page \chpgn  of {{\field{{\*\fldinst  NUMPAGES }}}}\par}} \keep \keepn", setName, round));
            builder.Append(string.Format(@"\keep \keepn {{\fonttbl{{\f99998\fnil\fcharset0 {0};}}{{\f99999\fbidi \fswiss\fcharset0\fprq2{{\*\panose 00000000000000000000}}Source Sans Pro SemiBold;}}}}\viewkind4\uc1\par\f99998\fs24\qc\b", font));
            string tossupHeaderText = string.Format(@"{0} - Round {1} - Tossups", setName, round);
            builder.Append(bitmapText + @"\line " + tossupHeaderText);
            builder.Append(@"\par\sb0\sa0\par\sb0\sa0\keep\keepn\ql\b0");

            builder.Append(GetTossupText(columns, 0, 21, 1, includeWriterNames, includeCategories, includeComments, commentFilters, true));

            builder.Append(@"\page ");
            builder.Append(string.Format(@"\keep \keepn {{\fonttbl{{\f99998\fnil\fcharset0 {0};}}{{\f99999\fbidi \fswiss\fcharset0\fprq2{{\*\panose 00000000000000000000}}Source Sans Pro SemiBold;}}}}\viewkind4\uc1\par\sb0\sa0\f99998\fs24\qc\b", font));
            string bonusHeaderText = string.Format(@"{0} - Round {1} - Bonuses", setName, round);
            builder.Append(bitmapText + @"\line " + bonusHeaderText);
            builder.Append(@"\par\sb0\sa0\par\sb0\sa0\keep\keepn\ql\b0");

            builder.Append(GetAcfBonusText(columns, 21, 21, 1, includeWriterNames, includeCategories, includeComments, commentFilters, true));

            return builder.ToString();
        }

        /// <summary>
        /// For a given packet in raw format, generate an RTF according to how it should be constructed
        /// for VHSL
        /// </summary>
        /// <param name="columns">Columns parsed from the CSV with question information. Each column is a tossup or bonus.</param>
        /// <param name="includeWriterNames">If true, include writer names</param>
        /// <param name="includeComments">If true, include comments under the questions</param>
        /// <param name="commentFilters">If non-empty, filter comments to only those which contain these words</param>
        /// <param name="setName">Name of the question set</param>
        /// <param name="round">Round number</param>
        /// <param name="bitmapText">The binary representation of the logo bitmap</param>
        /// <param name="font">Font name</param>
        /// <returns>RTF rendering of a VHSL packet</returns>
        public static string GetVhslText(List<string> columns, bool includeWriterNames, bool includeCategoryNames, bool includeComments, List<string> commentFilters, string setName, string round, string bitmapText, string font)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(@"{\colortbl;\red0\green0\blue0;\red0\green0\blue255;}");
            builder.Append(string.Format(@"{{\footer\pard\qr {0} - Round {1} - Page \chpgn  of {{\field{{\*\fldinst  NUMPAGES }}}}\par}} \keep \keepn", setName, round));
            builder.Append(string.Format(@"\keep \keepn {{\fonttbl{{\f99998\fnil\fcharset0 {0};}}{{\f99999\fbidi \fswiss\fcharset0\fprq2{{\*\panose 00000000000000000000}}Source Sans Pro SemiBold;}}}}\viewkind4\uc1\par\f99998\fs24\qc\b", font));
            string period1HeaderText = string.Format(@"{0} - Round {1} - First Period, Fifteen Tossups", setName, round);
            builder.Append(bitmapText + @"\line " + period1HeaderText);
            builder.Append(@"\par\sb0\sa0\par\sb0\sa0\keep\keepn\ql\b0");

            builder.Append(GetTossupText(columns, 0, 15, 1, includeWriterNames, includeCategoryNames, includeComments, commentFilters, true));

            builder.Append(@"\page ");
            builder.Append(string.Format(@"\keep \keepn {{\fonttbl{{\f99998\fnil\fcharset0 {0};}}{{\f99999\fbidi \fswiss\fcharset0\fprq2{{\*\panose 00000000000000000000}}Source Sans Pro SemiBold;}}}}\viewkind4\uc1\par\sb0\sa0\f0\fs24\qc\b", font));
            string period2HeaderText = string.Format(@"{0} - Round {1} - Directed Period", setName, round);
            builder.Append(bitmapText + @"\line " + period2HeaderText);
            builder.Append(@"\par\sb0\sa0\par\sb0\sa0\keep\keepn\ql\b0");

            builder.Append(GetVhslBonusText(columns, 35, 20, 1, includeWriterNames, includeCategoryNames, includeComments, true));

            builder.Append(@"\page ");
            builder.Append(string.Format(@"\keep \keepn {{\fonttbl{{\f99998\fnil\fcharset0 {0};}}{{\f99999\fbidi \fswiss\fcharset0\fprq2{{\*\panose 00000000000000000000}}Source Sans Pro SemiBold;}}}}\viewkind4\uc1\par\sb0\sa0\f0\fs24\qc\b", font));
            string period3HeaderText = string.Format(@"{0} - Round {1} - Third Period, Fifteen Tossups", setName, round);
            builder.Append(bitmapText + @"\line " + period3HeaderText);
            builder.Append(@"\par\sb0\sa0\par\sb0\sa0\keep\keepn\ql\b0");

            builder.Append(GetTossupText(columns, 15, 15, 1, includeWriterNames, includeCategoryNames, includeComments, commentFilters, true));

            builder.Append(@"\page ");
            builder.Append(string.Format(@"\keep \keepn {{\fonttbl{{\f99998\fnil\fcharset0 {0};}}{{\f99999\fbidi \fswiss\fcharset0\fprq2{{\*\panose 00000000000000000000}}Source Sans Pro SemiBold;}}}}\viewkind4\uc1\par\sb0\sa0\f0\fs24\qc\b", font));
            string tbHeaderText = string.Format(@"{0} - Round {1} - Tiebreaker Questions", setName, round);
            builder.Append(bitmapText + @"\line " + tbHeaderText);
            builder.Append(@"\par\sb0\sa0\par\sb0\sa0\keep\keepn\ql\b0");

            builder.Append(GetTossupText(columns, 30, 5, 1, includeWriterNames, includeCategoryNames, includeComments, commentFilters, true));
            builder.Append(GetVhslBonusText(columns, 55, 2, 1, includeWriterNames, includeCategoryNames, includeComments, false));

            return builder.ToString();
        }

        /// <summary>
        /// Gets the rtf text for the tossups in a packet or part of a packet
        /// </summary>
        /// <param name="columns">Columns from the CSV input of this packet. Each column is a different question.</param>
        /// <param name="columnStartIndex">Where to start reading tossups from</param>
        /// <param name="tossupCount">How many tossups to read</param>
        /// <param name="tossupStartIndex">Tossup number to start from</param>
        /// <param name="includeWriterNames">If true, include writer names</param>
        /// <param name="includeCategories">If true, include the question category</param>
        /// <param name="includeComments">If true, include comments</param>
        /// <param name="commentFilters">If non-empty, only include comments which match at least one of these terms</param>
        /// <param name="showNumber">If true, include the question number before the question</param>
        /// <returns>Rtf representation of the tossups in this packet</returns>
        public static string GetTossupText(List<string> columns, int columnStartIndex, int tossupCount, int tossupStartIndex, bool includeWriterNames, bool includeCategories, bool includeComments, List<string> commentFilters, bool showNumber)
        {
            StringBuilder builder = new StringBuilder();
            int tossupIndex = tossupStartIndex;
            for (int i = columnStartIndex; i < (columnStartIndex + tossupCount); i++)
            {
                string formattedIndex = string.Format("{0}. ", tossupIndex);
                if (!showNumber)
                {
                    formattedIndex = string.Empty;
                }

                if (columns[i].Contains("||"))
                {
                    // The raw form of each tossup is split into different components separated by ||
                    string[] tossupParts = columns[i].Split(new string[] { "||" }, StringSplitOptions.None);

                    // Get the tossup contents
                    string formattedLine = GetFormattedText($"{formattedIndex}{tossupParts[0]}", true);

                    builder.Append(string.Format(@"\keep \keepn {0}\line ", formattedLine));

                    // Get the tossup answer line
                    string writerName = string.Empty;
                    if (includeWriterNames && includeCategories)
                    {
                        writerName = string.Format(" <{0}, {1}>", tossupParts[2], tossupParts[4]);
                    }
                    else if (includeWriterNames)
                    {
                        writerName = string.Format(" <{0}>", tossupParts[2]);
                    }
                    else if (includeCategories)
                    {
                        writerName = string.Format(" <{0}>", tossupParts[4]);
                    }

                    builder.Append(GetFormattedText(string.Format(@"ANSWER: {0}", tossupParts[1]), false));
                    if (!string.IsNullOrEmpty(writerName))
                    {
                        builder.Append($@"\line{GetFormattedText(writerName, true)} ");
                    }

                    // Add any comments if necessary
                    if (includeComments)
                    {
                        if (!string.IsNullOrEmpty(tossupParts[3]))
                        {
                            List<string> filteredComments = GetFilteredComments(tossupParts[3].Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries), commentFilters);
                            if (filteredComments.Count > 0)
                            {
                                builder.Append(@"\cf2 "); // Color comments red

                                foreach (string comment in filteredComments)
                                {
                                    builder.Append(string.Format(@"\line {0}", comment));
                                }

                                builder.Append(@"\cf1 "); // Revert to black
                            }
                        }
                    }

                    builder.Append(@"\par\sb0\sa0\par\sb0\sa0 ");
                }
                else
                {
                    builder.Append(string.Format(@"{0}{1}\par\sb0\sa0\par\sb0\sa0 ", formattedIndex, columns[i]));
                }

                tossupIndex++;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Gets the rtf text for ACF-style bonuses in a packet or part of a packet
        /// </summary>
        /// <param name="columns">Columns from CSV with raw question input</param>
        /// <param name="columnStartIndex">Start index in the columns to read from</param>
        /// <param name="bonusCount">How many bonuses to read</param>
        /// <param name="bonusStartIndex">Bonus number to start with</param>
        /// <param name="includeWriterNames">If true, include writer names</param>
        /// <param name="includeCategories">If true, include category name</param>
        /// <param name="includeComments">If true, include comments</param>
        /// <param name="commentFilters">If non-empty, filter comments to just these terms</param>
        /// <param name="showNumber">If true, show question number</param>
        /// <returns>Rtf text for ACF-style bonuses</returns>
        public static string GetAcfBonusText(List<string> columns, int columnStartIndex, int bonusCount, int bonusStartIndex, bool includeWriterNames, bool includeCategories, bool includeComments, List<string> commentFilters, bool showNumber)
        {
            StringBuilder builder = new StringBuilder();
            int bonusIndex = bonusStartIndex;
            for (int i = columnStartIndex; i < (columnStartIndex + bonusCount); i++)
            {
                string formattedIndex = string.Format("{0}. ", bonusIndex);
                if (!showNumber)
                {
                    formattedIndex = string.Empty;
                }

                if (columns[i].Contains("||"))
                {
                    // Each bonus is split into different parts by ||
                    string[] bonusParts = columns[i].Split(new string[] { "||" }, StringSplitOptions.None);

                    if (bonusParts.Length >= 7)
                    {
                        string writerName = string.Empty;
                        if (includeWriterNames && includeCategories)
                        {
                            writerName = string.Format(" <{0}, {1}>", bonusParts[7], bonusParts[9]);
                        }
                        else if (includeWriterNames)
                        {
                            writerName = string.Format(" <{0}>", bonusParts[7]);
                        }
                        else if (includeCategories)
                        {
                            writerName = string.Format(" <{0}>", bonusParts[9]);
                        }

                        // Get the different parts of the bonus
                        builder.Append(@"\keep \keepn ");
                        builder.Append(GetFormattedText(string.Format(@"{0}{1} ", formattedIndex, bonusParts[0]), true));
                        builder.Append(@"\line ");
                        builder.Append(GetFormattedText(string.Format(@"[10] {0} ", bonusParts[1]), true));
                        builder.Append(@"\line ");
                        builder.Append(GetFormattedText(string.Format(@"ANSWER: {0}", bonusParts[2]), false));
                        builder.Append(@"\line ");
                        builder.Append(GetFormattedText(string.Format(@"[10] {0} ", bonusParts[3]), true));
                        builder.Append(@"\line ");
                        builder.Append(GetFormattedText(string.Format(@"ANSWER: {0} ", bonusParts[4]), false));
                        builder.Append(@"\line ");
                        builder.Append(GetFormattedText(string.Format(@"[10] {0} ", bonusParts[5]), true));
                        builder.Append(@"\line ");
                        builder.Append(GetFormattedText(string.Format(@"ANSWER: {0} ", bonusParts[6]), false));
                        if (!string.IsNullOrEmpty(writerName))
                        {
                            builder.Append($@"\line{GetFormattedText(writerName, true)} ");
                        }

                        if (includeComments)
                        {
                            if (!string.IsNullOrEmpty(bonusParts[8]))
                            {
                                List<string> filteredComments = GetFilteredComments(bonusParts[8].Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries), commentFilters);
                                if (filteredComments.Count > 0)
                                {
                                    builder.Append(@"\cf2 "); // Color comments red

                                    foreach (string comment in filteredComments)
                                    {
                                        builder.Append(string.Format(@"\line {0}", comment));
                                    }

                                    builder.Append(@"\cf1 "); // Revert to black
                                }
                            }
                        }
                    }

                    builder.Append(@"\par\sb0\sa0\par\sb0\sa0 ");
                }
                else
                {
                    builder.Append(@"\keep \keepn ");
                    builder.Append(string.Format(@"{0}{1}\par\sb0\sa0\par\sb0\sa0 ", formattedIndex, columns[i]));
                }

                bonusIndex++;
            }

            return builder.ToString();
        }

        /// <summary>
        /// For the given set of comments, return any that contain a substring from the list of
        /// commentFilters. If commentFilters is empty, return everything.
        /// </summary>
        /// <param name="comments">List of comments for this question</param>
        /// <param name="commentFilters">If any of these terms match, include the comment</param>
        /// <returns></returns>
        public static List<string> GetFilteredComments(string[] comments, List<string> commentFilters)
        {
            List<string> output = new List<string>();
            foreach (string comment in comments)
            {
                string lowerComment = comment.ToLowerInvariant();
                foreach (string filter in commentFilters)
                {
                    if (lowerComment.Contains(filter))
                    {
                        output.Add(comment);
                        break;
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// Gets the rtf text for VHSL bonuses for a packet or part of a packet
        /// </summary>
        /// <param name="columns">Columns from raw CSV</param>
        /// <param name="columnStartIndex">Start index in the columns for the bonuses</param>
        /// <param name="bonusCount">How many bonuses to read</param>
        /// <param name="bonusStartIndex">Bonus start number</param>
        /// <param name="includeWriterNames">If true, include writer names</param>
        /// <param name="includeCategoryNames">If true, include category names</param>
        /// <param name="includeComments">If true, include comments</param>
        /// <param name="showNumber">If true, show the question number</param>
        /// <returns>Rtf text for VHSL bonuses</returns>
        public static string GetVhslBonusText(List<string> columns, int columnStartIndex, int bonusCount, int bonusStartIndex, bool includeWriterNames, bool includeCategoryNames, bool includeComments, bool showNumber)
        {
            StringBuilder builder = new StringBuilder();
            int bonusIndex = bonusStartIndex;
            for (int i = columnStartIndex; i < (columnStartIndex + bonusCount); i++)
            {
                string formattedIndex = ((bonusIndex + 1) / 2) + "A. ";
                if (bonusIndex % 2 == 0)
                {
                    formattedIndex = (bonusIndex / 2) + "B. ";
                }

                if (!showNumber)
                {
                    formattedIndex = string.Empty;
                }

                if (columns[i].Contains("||"))
                {
                    string[] bonusParts = columns[i].Split(new string[] { "||" }, StringSplitOptions.None);

                    string writerName = string.Empty;
                    if (includeWriterNames)
                    {
                        writerName = string.Format(" <{0}>", bonusParts[7]);
                    }

                    builder.Append(@"\keep \keepn ");
                    builder.Append(GetFormattedText(string.Format(@"{0}{1}", formattedIndex, bonusParts[1]), true));
                    builder.Append(@"\line ");
                    builder.Append(GetFormattedText(string.Format(@"ANSWER: {0}{1} ", bonusParts[2], writerName), false));

                    if (includeComments)
                    {
                        if (!string.IsNullOrEmpty(bonusParts[8]))
                        {
                            string[] comments = bonusParts[8].Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries);
                            if (comments.Length > 0)
                            {
                                builder.Append(@"\cf2 "); // Color comments red

                                foreach (string comment in comments)
                                {
                                    builder.Append(string.Format(@"\line {0}", comment));
                                }

                                builder.Append(@"\cf1 "); // Revert to black
                            }
                        }
                    }

                    builder.Append(@"\par\sb0\sa0\par\sb0\sa0 ");
                }
                else
                {
                    builder.Append(string.Format(@"{0}{1}\par\sb0\sa0\par\sb0\sa0 ", formattedIndex, columns[i]));
                }

                bonusIndex++;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Loads an image file into a bitmap object
        /// </summary>
        /// <param name="file">Input image file</param>
        /// <returns>Bitmap version of it</returns>
        public static Bitmap LoadImage(string file)
        {
            Bitmap b = new Bitmap(file);
            return b;
        }

        #region RtfImageCode

        // RTF Image Format
        // {\pict\wmetafile8\picw[A]\pich[B]\picwgoal[C]\pichgoal[D]
        //  
        // A    = (Image Width in Pixels / Graphics.DpiX) * 2540 
        //  
        // B    = (Image Height in Pixels / Graphics.DpiX) * 2540 
        //  
        // C    = (Image Width in Pixels / Graphics.DpiX) * 1440 
        //  
        // D    = (Image Height in Pixels / Graphics.DpiX) * 1440 
        [Flags]
        enum EmfToWmfBitsFlags
        {
            EmfToWmfBitsFlagsDefault = 0x00000000,
            EmfToWmfBitsFlagsEmbedEmf = 0x00000001,
            EmfToWmfBitsFlagsIncludePlaceable = 0x00000002,
            EmfToWmfBitsFlagsNoXORClip = 0x00000004
        }

        const int MM_ISOTROPIC = 7;
        const int MM_ANISOTROPIC = 8;

        [DllImport("gdiplus.dll")]
        private static extern uint GdipEmfToWmfBits(IntPtr _hEmf, uint _bufferSize,
            byte[] _buffer, int _mappingMode, EmfToWmfBitsFlags _flags);
        [DllImport("gdi32.dll")]
        private static extern IntPtr SetMetaFileBitsEx(uint _bufferSize,
            byte[] _buffer);
        [DllImport("gdi32.dll")]
        private static extern IntPtr CopyMetaFile(IntPtr hWmf,
            string filename);
        [DllImport("gdi32.dll")]
        private static extern bool DeleteMetaFile(IntPtr hWmf);
        [DllImport("gdi32.dll")]
        private static extern bool DeleteEnhMetaFile(IntPtr hEmf);

        public static string GetEmbedImageString(Bitmap image)
        {
            Metafile metafile = null;
            float dpiX; float dpiY;

            using (Graphics g = Graphics.FromImage(image))
            {
                IntPtr hDC = g.GetHdc();
                metafile = new Metafile(hDC, EmfType.EmfOnly);
                g.ReleaseHdc(hDC);
            }

            using (Graphics g = Graphics.FromImage(metafile))
            {
                g.DrawImage(image, 0, 0);
                dpiX = g.DpiX;
                dpiY = g.DpiY;
            }

            IntPtr _hEmf = metafile.GetHenhmetafile();
            uint _bufferSize = GdipEmfToWmfBits(_hEmf, 0, null, MM_ANISOTROPIC,
            EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);
            byte[] _buffer = new byte[_bufferSize];
            GdipEmfToWmfBits(_hEmf, _bufferSize, _buffer, MM_ANISOTROPIC,
                                        EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);
            IntPtr hmf = SetMetaFileBitsEx(_bufferSize, _buffer);
            string tempfile = Path.GetTempFileName();
            CopyMetaFile(hmf, tempfile);
            DeleteMetaFile(hmf);
            DeleteEnhMetaFile(_hEmf);

            var stream = new MemoryStream();
            byte[] data = File.ReadAllBytes(tempfile);
            //File.Delete (tempfile);
            int count = data.Length;
            stream.Write(data, 0, count);

            string proto = @"{\rtf1{\pict\wmetafile8\picw" + (int)(((float)image.Width / dpiX) * 2540)
                              + @"\pich" + (int)(((float)image.Height / dpiY) * 2540)
                              + @"\picwgoal" + (int)(((float)image.Width / dpiX) * 1440)
                              + @"\pichgoal" + (int)(((float)image.Height / dpiY) * 1440)
                              + " "
                  + BitConverter.ToString(stream.ToArray()).Replace("-", "")
                              + "}}";
            return proto;
        }

        #endregion
    }
}
