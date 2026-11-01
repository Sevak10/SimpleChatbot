

namespace StructuredOutput;

public record Employee(
    string Name,
    string FirstName,
    DateOnly BirthDate,
    decimal Salary,
    string FunctionTitle);
