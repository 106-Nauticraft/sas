using NFluent;
using NSubstitute;
using NSubstitute.ClearExtensions;

namespace sas.simulators.nsubstitute.tests;

/// <summary>
/// Characterisation of the NSubstitute behaviour <see cref="BaseSimulator{T}"/> leans on to reset in place.
/// If any of these stops holding, resetting a substitute between scenarios has to be rebuilt another way.
/// A dropped stub then reads as NSubstitute's auto-value for the return type: string.Empty here, not null.
/// </summary>
public class ClearSubstituteShould
{
    [Fact]
    public void Drop_the_configured_returns()
    {
        var substitute = Substitute.For<IGreet>();
        substitute.Greet("Ada").Returns("Hello Ada");

        substitute.ClearSubstitute();

        Check.That(substitute.Greet("Ada")).IsEmpty();
    }

    [Fact]
    public void Drop_the_recorded_calls()
    {
        var substitute = Substitute.For<IGreet>();
        substitute.Greet("Ada").Returns("Hello Ada");
        substitute.Greet("Ada");

        substitute.ClearSubstitute();

        Check.That(substitute.ReceivedCalls()).IsEmpty();
    }

    [Fact]
    public void Clear_the_substitute_in_place_rather_than_replace_it()
    {
        var substitute = Substitute.For<IGreet>();
        var referenceTakenBeforeTheClear = substitute;
        substitute.Greet("Ada").Returns("Hello Ada");

        substitute.ClearSubstitute();

        Check.That(referenceTakenBeforeTheClear).IsSameReferenceAs(substitute);
        Check.That(referenceTakenBeforeTheClear.Greet("Ada")).IsEmpty();
    }

    [Fact]
    public void Let_the_substitute_be_stubbed_again()
    {
        var substitute = Substitute.For<IGreet>();
        substitute.Greet("Ada").Returns("Hello Ada");

        substitute.ClearSubstitute();
        substitute.Greet("Grace").Returns("Hi Grace");

        Check.That(substitute.Greet("Grace")).IsEqualTo("Hi Grace");
        Check.That(substitute.Greet("Ada")).IsEmpty();
    }

    public interface IGreet
    {
        string Greet(string name);
    }
}