namespace DevDefined.OAuth.Core
{
    public static class OAuthProblems
    {
        public const string ConsumerKeyRefused = "consumer_key_refused";
        public const string ConsumerKeyRejected = "consumer_key_rejected";
        public const string ConsumerKeyUnknown = "consumer_key_unknown";
        public const string NonceUsed = "nonce_used";
        public const string ParameterAbset = "parameter_absent";
        public const string ParameterRejected = "parameter_rejected";
        public const string PermissionDenied = "permission_denied";
        public const string PermissionUnknown = "permission_unknown";
        public const string SignatureInvalid = "signature_invalid";
        public const string SignatureMethodRejected = "signature_method_rejected";
        public const string TimestampRefused = "timestamp_refused";
        public const string TokenExpired = "token_expired";
        public const string TokenRejected = "token_rejected";
        public const string TokenRevoked = "token_revoked";
        public const string TokenUsed = "token_used";
        public const string VersionRejected = "version_rejected";
    }
}