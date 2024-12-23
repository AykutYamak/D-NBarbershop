namespace DNBarbershop.Common
{
    public static class ErrorMessages
    {
        public const string RequiredErrorMessage = "This field is required!";
        public const string InvalidEmailErrorMessage = "Please enter a valid email address.";
        private const string MaxLengthExceededTemplate = "The maximum allowed length is {0} characters.";
        public const string NonNegativeNumberErrorMessage = "Must be a non-negative number.";
        public const string MustBeWholeNumberErrorMessage = "Must be a whole number.";
        public static string MaxLengthExceededErrorMessage(int maxLength) => string.Format(MaxLengthExceededTemplate, maxLength);
        
        public const string MinimumLength = "Input must be at least 8 characters long.";
        public static string MinimumLengthErrorMessage(int minLength) => string.Format(MinimumLength, minLength);
    }
}
