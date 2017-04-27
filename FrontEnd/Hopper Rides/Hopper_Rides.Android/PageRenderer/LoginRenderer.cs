using Android.App;
using Xamarin.Forms.Platform.Android;
using Hopper_Rides;
using Xamarin.Forms;
using Hopper_Rides.Droid;
using Xamarin.Auth;
using System;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Xamarin.Utilities;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;
using System.Diagnostics;

[assembly: ExportRenderer(typeof(ProviderPage), typeof(LoginRenderer))]
namespace Hopper_Rides.Droid
{
	public class LoginRenderer : PageRenderer
	{
		bool showLogin = true;

		protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
		{

			base.OnElementChanged(e);

			//Get and Assign ProviderName from ProviderLoginPage
			var loginPage = Element as ProviderPage;
			string providername = loginPage.ProviderName;
			string user_PhoneNumber = loginPage.UserPhoneNumber;

			var activity = this.Context as Activity;
			if (showLogin && OAuthConfig.User == null)
			{
				showLogin = false;

				System.Console.WriteLine("calling LoginWithProvider (" + providername + ")");
				OAuth2Authenticator auth = null;

				if (providername == "FaceBook")
				{
					auth = new OAuth2Authenticator(
										 clientId: "129563854251270",  // For Facebook login, for configure refer http://www.c-sharpcorner.com/article/register-identity-provider-for-new-oauth-application/
										 scope: "user_about_me",
										 authorizeUrl: new Uri("https://www.facebook.com/v2.8/dialog/oauth/"), // These values do not need changing
										 redirectUrl: new Uri("http://www.facebook.com/connect/login_success.html")// These values do not need changin
						);
				}
				if (providername == "Google")
				{
					auth = new OAuth2Authenticator(
								// For Google login, for configure refer http://www.c-sharpcorner.com/article/register-identity-provider-for-new-oauth-application/
								"156308704452-p65lu6ce1lvs6a5nl69eeeui7j5okmdf.apps.googleusercontent.com",
							   "blL3bpK2UPG0u3KGcx42kRu5",
								// Below values do not need changing
								"https://www.googleapis.com/auth/userinfo.email",
								new Uri("https://accounts.google.com/o/oauth2/auth"),
								new Uri("http://www.devenvexe.com"),// Set this property to the location the user will be redirected too after successfully authenticating
								new Uri("https://accounts.google.com/o/oauth2/token")
								);
				}

				// After facebook,google and all identity provider login completed 
				auth.Completed += async (sender, eventArgs) =>
			  {
				  if (eventArgs.IsAuthenticated)
				  {
					  OAuthConfig.User = new Models.Rider();
					  // Get and Save User Details 


					  // Now that we're logged in, make a OAuth2 request to get the user's id.

					  var request = new OAuth2Request("GET", new Uri("https://graph.facebook.com/me?fields=id,email,first_name,last_name,gender,birthday"), null, eventArgs.Account);
					  var response = request.GetResponseAsync();

					  var obj = JObject.Parse(response.Result.GetResponseText());


					  //OAuthConfig.User.Email = eventArgs.Account.Properties["access_token"];
					  //OAuthConfig.User.Email = eventArgs.Account.Properties["email"];
					  OAuthConfig.User.ID = 20;
					  OAuthConfig.User.PhoneNumber = user_PhoneNumber;
					  OAuthConfig.User.FirstName = obj["first_name"].ToString().Replace("\"", "");
					  OAuthConfig.User.LastName = obj["last_name"].ToString().Replace("\"", "");
					  //var EmailAddress = obj["email"].ToString();
					  System.Console.WriteLine("Hello  " + OAuthConfig.User.FirstName + " " + OAuthConfig.User.LastName + " !");
					  System.Console.WriteLine("Your token: " + OAuthConfig.User.Email + " was assigned in our records"); ;

					  using (var client = new HttpClient())
					  {
                          client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
                          string ser_obj = JsonConvert.SerializeObject(OAuthConfig.User);
						  var content_post = new StringContent(ser_obj, Encoding.UTF8, "text/json");
						  //post it to the proper table
							var response_post = await client.PostAsync("http://thehopper.azurewebsites.net/api/riders", content_post);
                          var responseString_post = await response_post.Content.ReadAsStringAsync();
                          System.Diagnostics.Debug.WriteLine(responseString_post);
                      }
					  //OAuthConfig.PostRider();
					  //OAuthConfig.SuccessfulLoginAction.Invoke();
					  await ((ProviderPage)Element).SuccessfulLogin(new MapPage());

				  }
				  else
				  {
					  // The user cancelled
				  }
			  };

				activity.StartActivity(auth.GetUI(activity));

			}


		
		}
	}
}