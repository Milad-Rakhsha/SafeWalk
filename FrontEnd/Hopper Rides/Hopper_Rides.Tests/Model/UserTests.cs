using Hopper_Rides.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hopper_Rides.Tests.Model
{
	[TestClass]
	public class UserTests
	{
		private User user;


		[TestInitialize]
		public void TestInitialize()
		{
			user = new User();
		}

		//[TestCleanup]
		//public void TestCleanup() {}

		[TestMethod]
		public void TestCreateUser()
		{
			Assert.IsTrue(User.CreateUser());
		}
	}
}