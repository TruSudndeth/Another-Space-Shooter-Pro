using System.ComponentModel;

public static class DifficultiesEnums
{
    public enum Modes {
        test = 0, [Description("Easy ranges 0-2")]easy = 1, [Description("Medium ranges 1-3")]medium = 2, [Description("Hard Ranges 2-4")]hard = 3
    }
}
