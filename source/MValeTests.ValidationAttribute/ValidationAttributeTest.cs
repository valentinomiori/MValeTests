using System;
using System.ComponentModel.DataAnnotations;

namespace MVale.Tests.ValidationAttribute;

public class Tests
{
    class NonOverriddenSubject : global::System.ComponentModel.DataAnnotations.ValidationAttribute
    {
    }

    class OverriddenSubject : global::System.ComponentModel.DataAnnotations.ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            return true;
        }
    }

    // [SetUp]
    // public void Setup()
    // {
    // }

    [Test]
    public void Test()
    {
        global::System.ComponentModel.DataAnnotations.ValidationAttribute subject;

        subject = new NonOverriddenSubject();
        Assert.Throws<NotImplementedException>(() => subject.GetValidationResult(null, new ValidationContext(new object())));

        subject = new OverriddenSubject();
        Assert.DoesNotThrow(() => subject.GetValidationResult(null, new ValidationContext(new object())));
    }

    [Test]
    public void TestEmailAddress()
    {
        var subject = new EmailAddressAttribute();
        var value = "test@m@lformed.it";
        var result = subject.GetValidationResult(value, new ValidationContext(value) { DisplayName = nameof(value) });

        if (result == null)
            throw new InvalidOperationException();

        Assert.That(result.ErrorMessage, Is.EqualTo(subject.FormatErrorMessage(nameof(value))));
    }
}