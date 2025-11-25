using System;
using PatientService.Models;
using PatientService.Validators;
using Xunit;

namespace PatientService.Tests;

public class PatientDtoValidatorTests
{
    private readonly PatientDtoValidator _validator = new();

    [Fact]
    public void Should_Fail_When_Email_Is_Invalid()
    {
        var dto = new PatientDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "not-an-email",
            Phone = "123456789",
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            Gender = "Male"
        };

        var result = _validator.Validate(dto);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(PatientDto.Email));
    }

    [Fact]
    public void Should_Pass_For_Valid_Dto()
    {
        var dto = new PatientDto
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane@demo.com",
            Phone = "123456789",
            DateOfBirth = DateTime.UtcNow.AddYears(-28),
            Gender = "Female"
        };

        var result = _validator.Validate(dto);
        Assert.True(result.IsValid);
    }
}

