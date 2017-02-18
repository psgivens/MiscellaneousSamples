using NumbersOfBabel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NumbersTest {
    class Program {
        static void Main(string[] args) {
            Random random = new Random(DateTime.Now.Millisecond);
            string value = null;
            var translator = new NumbersTranslators.IndonesianNumberTranslator() as INumberTranslator;            
            bool isRunning = true;
            Console.WriteLine("Press any key to begin translating numbers.");
            Console.ReadKey();
            while (isRunning) {
                int counter = 0;
                var timer = new Timer(_ => isRunning = false, null, TimeSpan.FromMinutes(1), Timeout.InfiniteTimeSpan);
                while (isRunning) {
                    int generatedNumber = random.Next(999);
                    value = translator.TranslateNumber(generatedNumber);
                    while (true) {
                        if (!isRunning) break;
                        Console.WriteLine("Number: " + value);
                        Console.Write("Answer: ");
                        var answerLine = Console.ReadLine();
                        if (!isRunning) break;
                        int result;
                        if (int.TryParse(answerLine, out result) && result == generatedNumber) {
                            if (!isRunning) break;
                            counter++;
                            break;
                        }
                    }
                }
                Console.WriteLine("You translated {0} numbers from {1}.", counter, translator.Language);
                while (true) {
                    Console.WriteLine("Press 'a' to go again. Press 'q' to exit."); 
                    var key = Console.ReadKey().Key;
                    if (key == ConsoleKey.A) {
                        isRunning = true;
                        break;
                    }
                    if (key == ConsoleKey.Q) break;                    
                }
            }
        }
    }
}
