﻿namespace Persistence
{
    internal static class DbConstants
    {
        public const string PublicSchema = "public";

        internal static class ErrorCodesAndMessages
        {
            public const string UniqueViolation = "23505";
            public const string UniqueViolationMessage = "Entity is not unique.";
        }
    }
}
