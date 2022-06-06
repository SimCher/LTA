using System.Runtime.CompilerServices;
using LTA.API.Domain.Models;

namespace LTA.API.Infrastructure.Data.Helpers.Static;

public static class CodeGeneratorService
{
    private static Random Randomizer { get; }
    private const int MinSeed = 100000;
    private const int MaxSeed = 1000000;

    static CodeGeneratorService()
    {
        Randomizer = new Random();
    }

    public static string GenerateCode(IEnumerable<Topic>? topics)
    {
        var generateNumber = Randomizer.Next(MinSeed, MaxSeed);

        if (topics is null)
        {
            return generateNumber.ToString();
        }

        short i = 0;

        while (i < 100)
        {
            if (topics.Any(topic => int.Parse(topic.InviteCode) == generateNumber))
            {
                generateNumber = Randomizer.Next(MinSeed, MaxSeed);
                i++;
            }
            else
            {
                return generateNumber.ToString();
            }
        }

        throw new StackOverflowException("Invite code generation was taking too long.");
    }
}