using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace TestAndro
{
    public partial class WelcomePage : ContentPage
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        async void User1Handle_Clicked(object sender, System.EventArgs e)
        {
            indi.IsVisible = true;
            try
            {
                var result = await Login("nirmalsubedi@outlook.com");
                string authToken = result.Token; //Login authentication token
                App.DisplayName = "User1";
                var videoSession = await CreateVideoSession("User1", authToken);  //Creating the Session
                var token = await JoinVideoSession(videoSession, authToken);  //Passsing Session to Get New Token to authenticate the Vidyo;

                App.VidyoToken = token;  //Storing the Vidyo Token
                App.ResourceName = "DEBUGROOM"; //Room name both will connect;

                App.JoinVidyoSession();
            }
            catch(Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
            finally{
                indi.IsVisible = false;
            }
        }

        async void User2Handle_Clicked(object sender, System.EventArgs e)
        {
            indi.IsVisible = true;
            try
            {
                var result = await Login("creativemutu@hotmail.com");
                string authToken = result.Token; //Login authentication token
                App.DisplayName = "User2";
                var videoSession = await CreateVideoSession("User2", authToken);  //Creating the Session
                var token = await JoinVideoSession(videoSession, authToken);  //Passsing Session to Get New Token to authenticate the Vidyo;

                App.VidyoToken = token;  //Storing the Vidyo Token
                App.ResourceName = "DEBUGROOM"; //Room name both will connect;

                App.JoinVidyoSession();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                indi.IsVisible = false;
            }
        }

        private async Task<LoginResponse>Login(string userName)
        {
            string PASS = "ABC123";

            LoginModel loginModel = new LoginModel(){
                EmailId=userName, Password= PASS
            };

            using (HttpClient client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(loginModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = null;
                response = await client.PostAsync("https://ytimeappservice.azurewebsites.net/api/user/login", content);

                var serializer = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore };
                response.EnsureSuccessStatusCode();
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        using (var respjson = new JsonTextReader(reader))
                        {
                            return serializer.Deserialize<LoginResponse>(respjson);
                        }
                    }
                }
            }
        }


        private async Task<VideoSession> CreateVideoSession(string userName, string authToken)
        {
            VideoSession loginModel = new VideoSession()
            {
                EventDescription = string.Format("{0} at {1}", userName, DateTime.Now.ToLongTimeString())
            };

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer "+authToken);

                var json = JsonConvert.SerializeObject(loginModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = null;
                response = await client.PostAsync("https://ytimeappservice.azurewebsites.net/api/live/go", content);

                var serializer = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore };
                response.EnsureSuccessStatusCode();
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        using (var respjson = new JsonTextReader(reader))
                        {
                            return serializer.Deserialize<VideoSession>(respjson);
                        }
                    }
                }
            }
        }

        private async Task<string> JoinVideoSession(VideoSession session, string authToken)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + authToken);

                HttpResponseMessage response = null;
                response = await client.GetAsync("https://ytimeappservice.azurewebsites.net/api/live/join?eventid="+session.Id.ToString());
                return await response.Content.ReadAsStringAsync();
            }
        }

    }

   public class LoginModel
    {
        public string EmailId { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string UserId { get; set; }
        public string Token { get; set; }
    }

    public class VideoSession
    {
        public Guid Id { get; set; }
        public string EventDescription { get; set; }
    }
}
