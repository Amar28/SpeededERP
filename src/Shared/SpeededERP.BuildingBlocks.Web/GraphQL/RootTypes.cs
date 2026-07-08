namespace SpeededERP.BuildingBlocks.Web.GraphQL;

/// <summary>
/// Empty root types — every module contributes fields via
/// <c>[ExtendObjectType(OperationTypeNames.Query)]</c> / <c>...Mutation</c> on its own
/// GraphQL classes, discovered by <see cref="GraphQlModuleExtensions"/>. REST and
/// GraphQL are two adapters over the same Application layer (architecture section 19) —
/// this file intentionally carries no fields of its own.
/// </summary>
public sealed class Query;

public sealed class Mutation;
