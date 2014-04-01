using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pushbullet.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            string Nexus4 = "xxx";
            
            PushbulletClient client = new PushbulletClient("xxx");

            client.GetDevices(list => 
            {
                Console.WriteLine(list.Devices.Count);
                
                foreach (Device d in list.Devices)
                {
                    if(d.Identifier == Nexus4) {
                        //client.PushNote(d, "Test from C#", "This is a pushed message from the C# client.", response => { Console.WriteLine(response.Identifier); });
                        //client.PushLink(d, "Pushbullet", "www.pushbullet.com", response => { Console.WriteLine(response.Identifier); });
                        //client.PushList(d, "My Family", new List<string>() { "A", "B" }, response => { Console.WriteLine(response.Identifier); });
                        //client.PushAddress(d, "Home", "Somewhere xx, xxxx Someplace, Belgium", response => { Console.WriteLine(response.Identifier); });
                        client.PushFile(d, "C:\\Temp\\Test.txt", response => { Console.WriteLine(response.Identifier); });
                    }
                }
            });

            

            Console.ReadLine();
        }
    }
}
