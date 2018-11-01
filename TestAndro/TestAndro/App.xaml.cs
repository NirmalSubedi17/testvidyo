using System;
using VidyoConnector;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace TestAndro
{
    public partial class App : Application
    {
        public static string DisplayName = string.Empty;
        public static string ResourceName = string.Empty;
        public static string VidyoToken = string.Empty;

        private static IVidyoController _vidyoController = null;
        private static VidyoChatPage VidyoChatPage = null;
        // Need this in order to see preview in App.xaml interface builder
        public App()
        {
            InitializeComponent();
            MainPage = new WelcomePage();
        }

        public App(IVidyoController vidyoController)
        {
            InitializeComponent();
            _vidyoController = vidyoController;
            MainPage = new WelcomePage();
        }

        public static void JoinVidyoSession()
        {
            if (VidyoChatPage == null)
                VidyoChatPage = new VidyoChatPage();
            VidyoChatPage.Initialize(_vidyoController);
            Application.Current.MainPage = VidyoChatPage;
            ViewModel.GetInstance().ClientVersion = "v " + _vidyoController.OnAppStart();
            Console.WriteLine("Vidyo Version : " + ViewModel.GetInstance().ClientVersion);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
