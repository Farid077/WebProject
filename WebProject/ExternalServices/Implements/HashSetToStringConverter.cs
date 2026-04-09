using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WebProject.ExternalServices.Implements;

public class HashSetToStringConverter : ValueConverter<HashSet<string>, string>
{
    private const char Separator = '|';

    public HashSetToStringConverter() : base(
        // HashSet → DB string
        set => string.Join(Separator, set),

        // DB string → HashSet
        str => str.Split(Separator, StringSplitOptions.RemoveEmptyEntries).ToHashSet()
    )
    { }
}
