namespace sample.api.tests.nunit;

[SetUpFixture]
public class AllTestsFixtures
{
    [OneTimeSetUp]
    public void Init()
    {
        Diverse.Fuzzer.Log = TestContext.WriteLine;
    }
}