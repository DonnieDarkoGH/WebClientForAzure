using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebClientForAzure;


namespace WebClientSample {

    class Program 
    {
        static void Main(string[] args) {

            //Manager.MakeRequest("8842b380852146a48496b709e0fbad2a");
            Manager.GetAllProfiles("8842b380852146a48496b709e0fbad2a");

            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
        }
    }
}
