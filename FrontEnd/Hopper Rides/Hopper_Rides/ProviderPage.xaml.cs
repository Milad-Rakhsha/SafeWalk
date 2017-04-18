using Xamarin.Forms;
namespace Hopper_Rides
{
	public partial class ProviderPage : ContentPage
	{
		//we will refer providename from renderer page  
		public string ProviderName
		{
			get;
			set;
		}
		public string UserPhoneNumber
		{
			get;
			set;
		}
		public ProviderPage(string _providername, string _UserPhoneNumber)
		{
			InitializeComponent();
			ProviderName = _providername;
			UserPhoneNumber = _UserPhoneNumber;

		}
	}
}