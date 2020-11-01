using Xamarin.Essentials;
using Xunit;

namespace Droid.IntegrationTests
{
    public class EssentialsTests
    {
        [Fact]
        public void DeviceInfoShouldReturnsAndroidPlatform()
        {
            Assert.Equal(
                DevicePlatform.Android,
                DeviceInfo.Platform);
        }

        [Fact]
        public void AppInfoShouldReturnsCorrectVersion()
        {
            Assert.Equal(
                "1.0",
                AppInfo.VersionString);
        }

        [Fact]
        public void DeviceInfoShouldReturnsCorrectPackageName()
        {
            Assert.Equal(
                "com.companyname.droid_integrationtests",
                AppInfo.PackageName);
        }
    }
}
