using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace Test
{
    public class TestClass
    {
        enum Event
        {
            BeforeTestEvent,   
            CreateTest,
            TestEvent,  
            DisplayResults,    
            AskToRepeatTest,     
            ExitProgram,        
        }
        static int state;
        static Stopwatch? stopwatch;
        static String? UserInput;
        static string[]? lines;
        static List<string>? wordlist;
        static Random? rand;
        static string? testsentence;

        static int testwordamount;
        static void Init()
        {

            try
            {
                var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: true, reloadOnChange: true);
                IConfigurationRoot configuration = builder.Build();
                if (!configuration.GetSection("path-to-word-list-txt").Exists() || !configuration.GetSection("test-word-amount").Exists())
                {
                    throw new Exception("Failed to load configuration from file config.json.");
                }
                else
                {
                    lines = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\" + configuration.GetSection("path-to-word-list-txt").Value);
                    testwordamount = Int32.Parse(configuration.GetSection("test-word-amount").Value);
                    state = (int)Event.BeforeTestEvent;
                    stopwatch = new Stopwatch();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(-1);
            }

        }

        static void BeforeTestEvent()
        {

            Console.WriteLine("Typing test starting soon. Press spacebar to start. \r\nTest sentence will be displayed and you can start typing. \r\nPress enter when complete.\r\n");
            ConsoleKeyInfo keyinfo;
            do
            {
                keyinfo = Console.ReadKey();

            }
            while (keyinfo.Key != ConsoleKey.Spacebar);
            Console.WriteLine("\r\n\r\n\r\n");
            state = (int)Event.CreateTest;
        }
        static void CreateTest()
        {
            rand = new Random();
            wordlist = new List<string>();
            do
            {
                var word = lines[rand.Next(0, lines.Count())];
                if (!wordlist.Contains(word))
                {
                    wordlist.Add(word);
                }


            } while (wordlist.Count() < testwordamount);
            testsentence = string.Join(" ", wordlist).Trim();
            state = (int)Event.TestEvent;
        }
        static void TestEvent()
        {

            Console.WriteLine(testsentence);
            stopwatch.Start();
            UserInput = CaptureUserTestInput();
            stopwatch.Stop();

            state = (int)Event.DisplayResults;
        }
        static String CaptureUserTestInput()
        {
            String input = Console.ReadLine();
            return input;
        }

        static void CalculateAndDisplayResults()
        {
            if (UserInput.Length > testsentence.Length)
            {
                UserInput = UserInput.Substring(0, testsentence.Length);
            }
            else if (UserInput.Length < testsentence.Length)
            {
                testsentence = testsentence.Substring(0, UserInput.Length);
            }
            int numberoferrors = findNumberOfErrorsBetweenStrings(testsentence, UserInput);
            Console.WriteLine("\r\ntype test completed.");
            Console.WriteLine("Elapsed Time is {0} ms", stopwatch.ElapsedMilliseconds);
            double wpm = ((UserInput.Length / 5.0) - numberoferrors) / (stopwatch.ElapsedMilliseconds / 60000.0);
            Console.WriteLine(UserInput.Length);
            Console.WriteLine("Errors found:{0}.", numberoferrors);
            Console.Write("Your WPM is: ");
            Console.WriteLine(string.Format("{0:0.##}", wpm));
            state = (int)Event.AskToRepeatTest;
        }
        static int findNumberOfErrorsBetweenStrings(string testStr, string inputStr)
        {
            int numberoferrors = 0;
            for (int i = 0; i < inputStr.Length; i++)
            {
                if (inputStr[i] != testStr[i])
                {
                    numberoferrors++;
                }
            }
            return numberoferrors;
        }
        static void AskToRepeatTest()
        {
            while (true)
            {
                Console.WriteLine("Do you want to try again? (y/n)");
                string? input = Console.ReadLine();
                if (input == "y")
                {
                    Console.Clear();
                    state = (int)Event.BeforeTestEvent;
                    break;
                }
                else if (input == "n")
                {
                    state = (int)Event.ExitProgram;
                    break;
                }
            }


        }
        static void ExitProgram()
        {
            Console.WriteLine("Goodbye");
            Environment.Exit(0);

        }
        static void EventDispatcher()

        {
            while (true)
            {
                //switch block to call appropriate state
                switch (state)
                {
                    case 0:
                        BeforeTestEvent();
                        break;
                    case 1:
                        CreateTest();
                        break;
                    case 2:
                        TestEvent();
                        break;
                    case 3:
                        CalculateAndDisplayResults();
                        break;
                    case 4:
                        AskToRepeatTest();
                        break;
                    case 5:
                        ExitProgram();
                        break;

                }
            }
        }


        static void Main(string[] args)
        {
            Init();
            EventDispatcher();

        }
    }
}
