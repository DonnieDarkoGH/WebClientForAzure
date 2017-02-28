using System;
using WebClientForAzure;


namespace WebClientSample {

    class Program 
    {
        const string SubscriptionKey = "8842b380852146a48496b709e0fbad2a";

        static void Main(string[] args) {

            ProfileManager.SetSubscriptionKey(SubscriptionKey);
            ProfileManager.OnResponse += DisplayResponse;

            SpeakerManager.SetSubscriptionKey(SubscriptionKey);
            SpeakerManager.OnResponse += DisplayResponse;

            //ProfileManager.MakeRequest("8842b380852146a48496b709e0fbad2a");
            //ProfileManager.CreateProfile();
            ProfileManager.GetAllProfiles();
            //ProfileManager.GetProfile("a0ea0921-6ef8-4792-b000-95cf6338b0ab");
            //ProfileManager.DeleteProfile("45422ecc-3ba6-4a6e-a47c-36f48cf6bd75");

            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
        }

        static void DisplayResponse(string content) {

            Console.WriteLine("Here is the response : ");
            Console.WriteLine(content);


        }
    }
}
