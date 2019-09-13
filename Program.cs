using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Net;
using System.IO;

namespace TheCodeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string remoteUri = "https://ringba-test-html.s3-us-west-1.amazonaws.com/TestQuestions/output.txt";
            string path = @"c:\temp\myfile.txt";

            var wc = new WebClient();
            wc.DownloadFile(remoteUri, path);

            string[] contents = File.ReadAllLines(path);

            var upperChar = Regex.Matches(contents[0], "[A-Z]")
                                 .OfType<Match>()
                                 .Select(match => match.Value)
                                 .ToList();

            var words = Regex.Matches(contents[0], @"([A-Z][a-z]+)")
                             .OfType<Match>()
                             .Select(match => match.Value)
                             .OrderBy(match => match)
                             .ToList();

            var commonWords = words.GroupBy(x => x)
                                   .Where(g => g.Count() > 1)
                                   .Select(y => y.Key)
                                   .OrderBy(y => y)
                                   .ToList();
            
            var commonPrefixes = words.Where(p => p.Length > 2)
                                      .Select(x => x.Substring(0,2).ToLowerInvariant())
                                      .OrderBy(x => x)
                                      .Distinct()
                                      .ToList();

            var wordFound = 0;
            string commonWord = string.Empty;

            commonWords.ForEach(i => {
                var matchQuery = (from w in words
                                  where w == i
                                  select w).FirstOrDefault();
                var matchCount = (from w in words
                                  where w == i
                                  select w).Count();

                if (matchCount > wordFound) {
                    wordFound = matchCount;
                    commonWord = matchQuery;
                                        
                    Console.WriteLine("\n# times found: {0:N0}\n" + "Common Words: {1} = {2}\n", wordFound, commonWord, i);
                }

                Console.WriteLine("Count: {0:N0} - Word: {1} = {2}" , matchCount, matchQuery, i);
            });

            var prefixFound = 0;
            string commonPrefix = string.Empty;

            commonPrefixes.ForEach(i => {
                var matchPrefix = (from w in words
                                   where w.Substring(0,2).ToLowerInvariant() == i
                                   select w.Substring(0,2).ToLowerInvariant()).FirstOrDefault();
                var matchCount = (from w in words
                                  where w.Substring(0,2).ToLowerInvariant() == i
                                  select w.Substring(0,2).ToLowerInvariant()).Count();

                if (matchCount > prefixFound) {
                    prefixFound = matchCount;
                    commonPrefix = matchPrefix;
                                        
                    Console.WriteLine("\n# prefix found: {0:N0}\n" + "Common Prefix: {1} = {2}\n", prefixFound, commonPrefix, i);
                }

                Console.WriteLine("Count: {0:N0} - Prefix: {1} = {2}" , matchCount, matchPrefix, i);
            });

            // char[] charArray = contents.SelectMany(x=>x.ToCharArray()).ToArray();
            // int charCapitalized = charArray.Where(c=>char.IsUpper(c)).Count();
            // string input = String.Join(string.Empty, contents.Select(c=>c.ToString()).ToArray());
            // var uniqueWords = (from w in words select w).Distinct();
            // int i = 0;
            // foreach(string uw in uniqueWords) {
            //     var matchQuery = from w in words  
            //                       where w == uw  
            //                       select w;                
            //     var count = matchQuery.Count();
            //     if (count > timesFound) {
            //         timesFound = count;
            //         commonWord = matchQuery.First();                                        
            //         Console.WriteLine("# times found: {0}\n" + "Common Words: {1}\n", timesFound, commonWord);
            //     }
            //     i++;
            //     Console.WriteLine("Count: {0} - Word: {1} => {2} of {3}" , count, matchQuery.First(), i, uniqueWords.Count());
            // }

            Console.WriteLine("\n" +
                              "Output Text File has {0:N0} letters.\n" + 
                              "Output Text File has {1:N0} capitalized letters.\n" +
                              "Most common word is {2} and number of times seen {3:N0}.\n" +
                              "Most common 2 character prefix is '{4}' and the number of occurrences is {5:N0}.\n", 
                              contents[0].ToString().Length, upperChar.Count(), commonWord, wordFound, commonPrefix, prefixFound);
            
            Console.WriteLine("Item count in words is {0:N0}\n" +
                              "Item count in commonWords is {1:N0}\n" +
                              "Item count in commonPrefix is {2:N0}\n", 
                              words.Count(), commonWords.Count(), commonPrefixes.Count());
        }
    }
}
