using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Drill
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread thread1 = new Thread(CountTo100);
            thread1.Start();


            Thread thread2 = new Thread(CountTo100);
            thread2.Start();

            Console.ReadKey();


        }


        public static void CountTo100()
        {
            for (int index = 0; index < 100; index++)
            {
                Console.WriteLine(index + 1);
                Thread.Sleep(1000);
                /* Interessant:
                 * Wenn keine Verzögerung implementiert wird, kann man sehr schön sehen
                 * wie die Ausgabe sich beim jeden Neustart ändert. Dies liegt daran, dass
                 * immer anders entschieden wird welcher Thread ausgeführt werden soll.
                 * Doch entscheidet das Betriebssystem oder der Prozessor ?
                 */

            }
        }
    }
}
